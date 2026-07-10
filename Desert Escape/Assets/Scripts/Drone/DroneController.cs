using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneController : MonoBehaviour
{
    FSM<StatesEnum> _fsm;
    ITreeNode _root;
    [SerializeField] DroneModel _model;
    //[SerializeField] GameObject target;
    private Rigidbody _targetRigidbody;

    [SerializeField] float closeToLeader;
    [SerializeField] float closeToLeaderMargin = 1f;
    [SerializeField] float killRayDistance = 1.5f;
    [SerializeField] LayerMask killRayMask = ~0;
    IState<StatesEnum> _idleState;

    public LeaderBehaviour leaderBehaviour;

    ISteering _steering;
    ObstacleAvoidanceV2 _obstacleAvoidance;

    ILineOfSight _los;

    #region Steering
    public float timePrediction;
    public float angle;
    public float radius;
    public float personalArea;
    public LayerMask maskObs;
    #endregion

    private void Awake()
    {
        _model = GetComponent<DroneModel>();
        _los = GetComponent<ILineOfSight>();

    }
    void Start()
    {
        InitializeSteerings();
        InitializedTree();
        InitializeFSM();
    }

    void InitializeFSM()
    {
        var idle = new DroneIdleState<StatesEnum>(_model);
        var follow = new DroneFollowState<StatesEnum>(_model, _steering, _obstacleAvoidance);
        _idleState = idle;

        idle.AddTransition(StatesEnum.Follow, follow);
        follow.AddTransition(StatesEnum.Idle, idle);

        _fsm = new FSM<StatesEnum>(idle);
    }



    void InitializeSteerings()
    {
        _steering = GetComponent<FlockingManager>();

        _obstacleAvoidance = new ObstacleAvoidanceV2(_model.transform, angle, radius, maskObs, personalArea);
    }

    void InitializedTree()
    {
        // Actions
        var idle = new ActionNode(() => _fsm.Transition(StatesEnum.Idle));
        var follow = new ActionNode(() => _fsm.Transition(StatesEnum.Follow));

        // Questions
        QuestionNode distanceToLead = new QuestionNode(QuestionTooClose, idle, follow);

        _root = distanceToLead;
    }

    bool QuestionTooClose()
    {
        // Hysteresis: use a wider "stay idle" threshold and a narrower "stay following"
        // threshold so hovering right at closeToLeader doesn't flip state every frame.
        bool isIdle = _fsm.CurrentState == _idleState;
        float threshold = isIdle ? closeToLeader + closeToLeaderMargin : closeToLeader - closeToLeaderMargin;
        return Vector3.Distance(transform.position, leaderBehaviour.target.position) < threshold;
    }

    bool QuestionPatrol()
    {
        return true;
    }


    void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
        CheckKillTouch();
    }

    // The drone hovers above the player instead of physically colliding with them, so
    // "catching" the player is a downward raycast instead of a trigger/collision check.
    void CheckKillTouch()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, killRayDistance, killRayMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                GameOver.Trigger();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector3.down * killRayDistance);
    }

}