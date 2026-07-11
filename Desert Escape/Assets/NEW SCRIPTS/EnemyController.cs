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

    [SerializeField] float loseSightMultiplier = 1.3f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float searchDuration = 4f;
    IState<StatesEnum> _chaseState;
    IState<StatesEnum> _attackState;
    EnemySearchState<StatesEnum> _searchState;

    EnemyBlackboard _blackboard;
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
        InitializeBlackboard();
        InitializedTree();
        InitializeFSM();

        if (_agentController != null)
        {
            IncreaseWaypontIndex();
            _agentController.RunAStar();
        }
    }

    // Everything the states and tree questions need to read/write lives in one shared
    // object instead of each of them getting a hand-picked subset of references.
    void InitializeBlackboard()
    {
        _blackboard = new EnemyBlackboard
        {
            Model = _model,
            Target = target.transform,
            LineOfSight = _los,
            Pursuit = _pursuit,
            ObstacleAvoidance = _obstacleAvoidance
        };
    }

    void InitializeFSM()
    {
        var idle = new EnemyIdleState<StatesEnum>();
        var chase = new EnemyChaseState<StatesEnum>(_blackboard);
        var attack = new EnemyAttackState<StatesEnum>(_blackboard);
        var search = new EnemySearchState<StatesEnum>(_blackboard, searchDuration);
        _stateFollowPoints = new EnemyPatrolState<StatesEnum>(_blackboard);
        _chaseState = chase;
        _attackState = attack;
        _searchState = search;

        idle.AddTransition(StatesEnum.Chase, chase);    
        idle.AddTransition(StatesEnum.Patrol, _stateFollowPoints);
        idle.AddTransition(StatesEnum.Attack, attack);

        chase.AddTransition(StatesEnum.Idle, idle);                     
        chase.AddTransition(StatesEnum.Patrol, _stateFollowPoints);    
        chase.AddTransition(StatesEnum.Attack, attack);
        chase.AddTransition(StatesEnum.Search, search);

        attack.AddTransition(StatesEnum.Idle, idle);
        attack.AddTransition(StatesEnum.Chase, chase);
        attack.AddTransition(StatesEnum.Patrol, _stateFollowPoints);
        attack.AddTransition(StatesEnum.Search, search);

        search.AddTransition(StatesEnum.Chase, chase);
        search.AddTransition(StatesEnum.Attack, attack);
        search.AddTransition(StatesEnum.Patrol, _stateFollowPoints);

        _stateFollowPoints.AddTransition(StatesEnum.Idle, idle);
        _stateFollowPoints.AddTransition(StatesEnum.Chase, chase);
        _stateFollowPoints.AddTransition(StatesEnum.Attack, attack);

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
        var patrol = new ActionNode(() => _fsm.Transition(StatesEnum.Patrol));
        var chase = new ActionNode(() => _fsm.Transition(StatesEnum.Chase));
        var attack = new ActionNode(() => _fsm.Transition(StatesEnum.Attack));
        var search = new ActionNode(() =>
        {
            if (_fsm.CurrentState != _searchState) _fsm.Transition(StatesEnum.Search);
        });

        // Questions
        var qSearchExpired = new QuestionNode(QuestionSearchExpired, patrol, search);
        var qWasAlert = new QuestionNode(QuestionWasAlert, qSearchExpired, patrol);
        var qAttackRange = new QuestionNode(QuestionAttackRange, attack, chase);
        var qLoS = new QuestionNode(QuestionLoS, qAttackRange, qWasAlert);

        _root = qLoS;
    }

    bool QuestionLoS()
    {
        bool isAlert = _fsm.CurrentState == _chaseState || _fsm.CurrentState == _attackState || _fsm.CurrentState == _searchState;
        float rangeMultiplier = isAlert ? loseSightMultiplier : 1f;
        float angleMultiplier = isAlert ? loseSightMultiplier : 1f;

        _blackboard.HasLineOfSight = _blackboard.LineOfSight.CheckRange(_blackboard.Target, _blackboard.LineOfSight.Range * rangeMultiplier)
                && _blackboard.LineOfSight.CheckAngle(_blackboard.Target, _blackboard.LineOfSight.Angle * angleMultiplier)
                && _blackboard.LineOfSight.CheckView(_blackboard.Target);

        return _blackboard.HasLineOfSight;
    }

    bool QuestionAttackRange()
    {
        return Vector3.Distance(transform.position, _blackboard.Target.position) <= attackRange;
    }

    bool QuestionWasAlert()
    {
        return _fsm.CurrentState == _chaseState || _fsm.CurrentState == _attackState || _fsm.CurrentState == _searchState;
    }

    bool QuestionSearchExpired()
    {
        return _fsm.CurrentState == _searchState && _searchState.TimedOut;
    }

    private void IncreaseWaypontIndex()
    {
        _model.currentWaypointIndex = PickWeightedWaypointIndex();
        _model.currentWayPoint = _model.waypoints[_model.currentWaypointIndex];
        _agentController.target = _model.currentWayPoint;
        _agentController.RunAStar();
    }

    private int PickWeightedWaypointIndex()
    {
        var distances = new float[_model.waypoints.Length];
        float totalDistance = 0f;

        for (int i = 0; i < _model.waypoints.Length; i++)
        {
            if (i == _model.index) continue; 
            distances[i] = Vector3.Distance(transform.position, _model.waypoints[i].transform.position);
            totalDistance += distances[i];
        }

        var weights = new Dictionary<int, int>();
        for (int i = 0; i < _model.waypoints.Length; i++)
        {
            if (i == _model.index) continue;
            float probability = totalDistance > 0f ? 1f - (distances[i] / totalDistance) : 1f;
            weights[i] = Mathf.Max(1, Mathf.RoundToInt(probability * 100));
        }

        if (weights.Count == 0) return 0;
        return MyRandoms.Roulette(weights);
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
        else if (other.CompareTag("Player"))
        {
            GameOver.Trigger();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameOver.Trigger();
        }
    }


    public IPoints GetStateWaypoints => _stateFollowPoints;
}
