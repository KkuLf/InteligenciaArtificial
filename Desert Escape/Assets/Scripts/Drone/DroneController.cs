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
        var idle = new EnemyIdleState<StatesEnum>();
        var follow = new DroneFollowState<StatesEnum>(_model, _steering, _obstacleAvoidance);
        
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
        return Vector3.Distance(transform.position, leaderBehaviour.target.position) < closeToLeader;
    }

    bool QuestionPatrol()
    {
        return true;
    }


    void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }

}