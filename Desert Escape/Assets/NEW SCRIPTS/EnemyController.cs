using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour
{
    FSM<StatesEnum> _fsm;
    ITreeNode _root;
    [SerializeField] EnemyModel _model;
    [SerializeField] GameObject target;
    [SerializeField] AgentController _agentController;
    private Rigidbody _targetRigidbody;

    ISteering _pursuit;
    ObstacleAvoidanceV2 _obstacleAvoidance;

    ILineOfSight _los;

    #region Steering
    public float timePrediction;
    public float angle;
    public float radius;
    public float personalArea;
    public LayerMask maskObs;
    #endregion

    EnemyPatrolState<StatesEnum> _stateFollowPoints;


    private void Awake()
    {
        _model = GetComponent<EnemyModel>();
        _los = GetComponent<ILineOfSight>();
        _targetRigidbody = target.GetComponent<Rigidbody>();

    }
    void Start()
    {
        InitializeSteerings();
        InitializedTree();
        InitializeFSM();

        if (_agentController != null)
        {
            IncreaseWaypontIndex();
            _agentController.RunAStar();
        }
    }

    void InitializeFSM()
    {
        var idle = new EnemyIdleState<StatesEnum>();
        var chase = new EnemyChaseState<StatesEnum>(_model, target.transform, _pursuit, _obstacleAvoidance);
        var attack = new EnemyAttackState<StatesEnum>(_model);
        _stateFollowPoints = new EnemyPatrolState<StatesEnum>(_model);

        idle.AddTransition(StatesEnum.Chase, chase);    // Transition from idle to chase
        idle.AddTransition(StatesEnum.Patrol, _stateFollowPoints);
        idle.AddTransition(StatesEnum.Attack, attack);

        chase.AddTransition(StatesEnum.Idle, idle);                     // Transition from chase to idle
        chase.AddTransition(StatesEnum.Patrol, _stateFollowPoints);     // Transition from 
        chase.AddTransition(StatesEnum.Attack, attack);

        attack.AddTransition(StatesEnum.Idle, idle);
        attack.AddTransition(StatesEnum.Chase, chase);
        attack.AddTransition(StatesEnum.Patrol, _stateFollowPoints);


        _stateFollowPoints.AddTransition(StatesEnum.Idle, idle);
        _stateFollowPoints.AddTransition(StatesEnum.Chase, chase);

        _fsm = new FSM<StatesEnum>(idle);
    }



    void InitializeSteerings()
    {
        var pursuit = new Pursuit(_model.transform, _targetRigidbody, timePrediction);
        _pursuit = pursuit;

        _obstacleAvoidance = new ObstacleAvoidanceV2(_model.transform, angle, radius, maskObs, personalArea);
    }

    void InitializedTree()
    {
        // Actions
        var idle = new ActionNode(() => _fsm.Transition(StatesEnum.Idle));
        var patrol = new ActionNode(() => _fsm.Transition(StatesEnum.Patrol));
        var chase = new ActionNode(() => _fsm.Transition(StatesEnum.Chase));
        var attack = new ActionNode(() => _fsm.Transition(StatesEnum.Attack));

        // Questions
        var qPatrol = new QuestionNode(QuestionPatrol, patrol, idle);
        var qLoS = new QuestionNode(QuestionLoS, chase, qPatrol);

        _root = qLoS;
    }

    bool QuestionLoS()
    {
        return _los.CheckRange(target.transform)
                && _los.CheckAngle(target.transform)
                && _los.CheckView(target.transform);

    }

    bool QuestionPatrol()
    {
        return true;
    }

    private void IncreaseWaypontIndex()
    {
        // Randomized currentWaypointIndex controlled
        _model.currentWaypointIndex = MyRandoms.RangeRandom(0, _model.waypoints.Length);
        if (_model.index == _model.currentWaypointIndex)
        {
            _model.currentWaypointIndex++;
        }
        if (_model.currentWaypointIndex >= _model.waypoints.Length)
        {
            _model.currentWaypointIndex = 0;
        }
        _model.currentWayPoint = _model.waypoints[_model.currentWaypointIndex];
        _agentController.target = _model.currentWayPoint;
        _agentController.RunAStar();
    }


    void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Box")
        {
            _model.index = _model.currentWaypointIndex;
            IncreaseWaypontIndex();
        }
    }


    public IPoints GetStateWaypoints => _stateFollowPoints;
}
