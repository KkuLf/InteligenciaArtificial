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
        var chase = new EnemyChaseState<StatesEnum>(_model, _pursuit, _obstacleAvoidance, target.transform);
        var attack = new EnemyAttackState<StatesEnum>(_model, target.transform);
        var search = new EnemySearchState<StatesEnum>(_model, searchDuration);
        _stateFollowPoints = new EnemyPatrolState<StatesEnum>(_model, _obstacleAvoidance);
        _chaseState = chase;
        _attackState = attack;
        _searchState = search;

        idle.AddTransition(StatesEnum.Chase, chase);    // Transition from idle to chase
        idle.AddTransition(StatesEnum.Patrol, _stateFollowPoints);
        idle.AddTransition(StatesEnum.Attack, attack);

        chase.AddTransition(StatesEnum.Idle, idle);                     // Transition from chase to idle
        chase.AddTransition(StatesEnum.Patrol, _stateFollowPoints);     // Transition from
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
            // Only (re)transition on the frame it's actually entered, otherwise
            // re-triggering Enter() every frame would keep resetting its timer.
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
        // Widen the range/angle while already alert (chasing/attacking/searching) so
        // small jitter near the edge doesn't flip the result every frame (hysteresis).
        bool isAlert = _fsm.CurrentState == _chaseState || _fsm.CurrentState == _attackState || _fsm.CurrentState == _searchState;
        float rangeMultiplier = isAlert ? loseSightMultiplier : 1f;
        float angleMultiplier = isAlert ? loseSightMultiplier : 1f;

        return _los.CheckRange(target.transform, _los.Range * rangeMultiplier)
                && _los.CheckAngle(target.transform, _los.Angle * angleMultiplier)
                && _los.CheckView(target.transform);

    }

    bool QuestionAttackRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    // Only relevant once we've actually been alert (chasing/attacking/searching) - a
    // fresh Idle/Patrol enemy that never spotted the player has nothing to "give up" on.
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

    // Roulette-wheel waypoint pick: closer waypoints get a higher chance, mirroring the
    // old PatrolState.cs's distance weighting, but reusing the shared MyRandoms.Roulette<T>.
    private int PickWeightedWaypointIndex()
    {
        var distances = new float[_model.waypoints.Length];
        float totalDistance = 0f;

        for (int i = 0; i < _model.waypoints.Length; i++)
        {
            if (i == _model.index) continue; // don't immediately re-pick where we came from
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
