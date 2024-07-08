using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class DroneController : MonoBehaviour
{
    FSM<StatesEnum> _fsm;
    ITreeNode _root;
    [SerializeField] DroneModel _model;
    //[SerializeField] GameObject target;
    private Rigidbody _targetRigidbody;

    [SerializeField] bool isDrone;
    [SerializeField] float closeToLeader;
    [SerializeField] GameObject target;


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
        var attack = new ActionNode(() => EndGame());


        // Questions
        QuestionNode distanceToLead = new QuestionNode(QuestionTooClose, idle, follow);

        var qLoS = new QuestionNode(QuestionLoS, attack, follow);

        _root = distanceToLead;
    }

    bool QuestionLoS()
    {
        var seen = _los.CheckRange(target.transform)
                && _los.CheckAngle(target.transform)
                && _los.CheckView(target.transform);

        if (seen == true && isDrone)
        {
            EndGame();
            return seen;
        }
        return seen;

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

    private void EndGame()
    {
        Debug.Log("PLAYER SPOTTED BY DRONE!");
        SceneManager.LoadScene("GameOver");
    }
}