using System.Collections.Generic;
using UnityEngine;

// Second agent of the assignment: unlike the Enemy (FSM + binary decision tree),
// the Drone is driven by a real Behaviour Tree - a root Selector trying plans in
// priority order (kill > hunt spotted player > rest near leader > follow > pathfind
// > idle), where each Sequence gates its action behind a condition and leaves report
// back Success/Failure/Running.
public class DroneController : MonoBehaviour
{
    IBTNode _root;
    [SerializeField] DroneModel _model;

    [SerializeField] float closeToLeader;
    [SerializeField] float closeToLeaderMargin = 1f;
    [SerializeField] float killRayDistance = 1.5f;
    [SerializeField] LayerMask killRayMask = ~0;
    [SerializeField] float pathRecalcInterval = 1f;

    public LeaderBehaviour leaderBehaviour;

    ISteering _steering;
    ObstacleAvoidanceV2 _obstacleAvoidance;
    DronePathfinder _pathfinder;
    ILineOfSight _los;

    #region Steering
    public float timePrediction;
    public float angle;
    public float radius;
    public float personalArea;
    public LayerMask maskObs;
    #endregion

    // Tree working data
    bool _isIdling;
    List<Vector3> _path;
    int _pathIndex;
    float _pathTimer;
    Transform _player;

    private void Awake()
    {
        _model = GetComponent<DroneModel>();
        _pathfinder = GetComponent<DronePathfinder>();
        _los = GetComponent<ILineOfSight>();
    }

    void Start()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) _player = playerGO.transform;

        InitializeSteerings();
        InitializeTree();
    }

    void InitializeSteerings()
    {
        _steering = GetComponent<FlockingManager>();
        _obstacleAvoidance = new ObstacleAvoidanceV2(_model.transform, angle, radius, maskObs, personalArea);
    }

    void InitializeTree()
    {
        // Plan 1: target is right under us -> kill it.
        var killSequence = new BTSequence(
            new BTCondition(IsPlayerUnderKillRay),
            new BTAction(KillPlayer));

        // Plan 2: the player is inside our vision cone (full sensor: overlap + dot +
        // raycast via LineOfSight) -> break off the flock and hunt them.
        var huntSequence = new BTSequence(
            new BTCondition(SeesPlayer),
            new BTAction(DoHuntPlayer));

        // Plan 3: already close enough to the leader -> hover in place.
        var restSequence = new BTSequence(
            new BTCondition(IsCloseToLeader),
            new BTAction(DoIdle));

        // Plan 4: straight line to the leader is clear -> flock toward it.
        var followSequence = new BTSequence(
            new BTCondition(HasClearPathToLeader),
            new BTAction(DoFollow));

        // Plan 5: leader out of straight-line reach -> A* through the node grid.
        var pathfindSequence = new BTSequence(
            new BTCondition(CanPathfind),
            new BTAction(DoFollowPath));

        // Last resort: nothing applies (e.g. no leader assigned) -> stay put.
        var idleFallback = new BTAction(DoIdle);

        _root = new BTSelector(killSequence, huntSequence, restSequence, followSequence, pathfindSequence, idleFallback);
    }

    void Update()
    {
        _root.Tick();
    }

    Transform Leader => leaderBehaviour != null ? leaderBehaviour.target : null;

    // ---- Conditions ----

    bool IsPlayerUnderKillRay()
    {
        // The drone hovers above the player instead of physically colliding with them,
        // so "catching" the player is a downward raycast, not a trigger/collision check.
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, killRayDistance, killRayMask)
            && hit.collider.CompareTag("Player");
    }

    bool IsCloseToLeader()
    {
        if (Leader == null) return false;
        // Hysteresis: wider "stay idle" threshold, narrower "stay following" one, so
        // hovering right at closeToLeader doesn't flip the decision every frame.
        float threshold = _isIdling ? closeToLeader + closeToLeaderMargin : closeToLeader - closeToLeaderMargin;
        return Vector3.Distance(transform.position, Leader.position) < threshold;
    }

    bool SeesPlayer()
    {
        if (_player == null || _los == null) return false;
        // Full vision sensor (overlap + dot + raycast via LineOfSight), same as the
        // Enemy's FSM uses - this BT node is where the Drone's decisions consume it.
        return _los.CheckRange(_player)
            && _los.CheckAngle(_player)
            && _los.CheckView(_player);
    }

    bool HasClearPathToLeader()
    {
        if (Leader == null) return false;
        // Flock-following is proximity-based, not vision-based: a follower shouldn't
        // need its leader inside a forward cone (it's usually beside or behind it) -
        // only an unobstructed straight line matters here.
        return !Physics.Linecast(transform.position, Leader.position, maskObs);
    }

    bool CanPathfind()
    {
        return _pathfinder != null && Leader != null;
    }

    // ---- Actions ----

    BTStatus KillPlayer()
    {
        GameOver.Trigger();
        return BTStatus.Success;
    }

    BTStatus DoIdle()
    {
        _isIdling = true;
        _model.Move(Vector3.zero);
        _model.LevelOut();
        return BTStatus.Running;
    }

    BTStatus DoHuntPlayer()
    {
        _isIdling = false;
        _path = null;
        Vector3 dir = _player.position - transform.position;
        var moveDir = _obstacleAvoidance.GetDir(dir.normalized, false);
        _model.Move(moveDir);
        _model.LookDir(moveDir);
        return BTStatus.Running;
    }

    BTStatus DoFollow()
    {
        _isIdling = false;
        _path = null; // direct route available again, drop any stale A* path
        var dir = _obstacleAvoidance.GetDir(_steering.GetDir(), false);
        _model.Move(dir);
        _model.LookDir(dir);
        return BTStatus.Running;
    }

    BTStatus DoFollowPath()
    {
        _isIdling = false;
        _pathTimer -= Time.deltaTime;
        if (_path == null || _pathTimer <= 0f)
        {
            _path = _pathfinder.GetPath(transform.position, Leader.position);
            _pathIndex = 0;
            _pathTimer = pathRecalcInterval;
        }
        if (_path == null || _path.Count == 0)
        {
            _path = null;
            return BTStatus.Failure; // no route found, let the selector fall through to idle
        }

        var point = _path[_pathIndex];
        point.y = transform.position.y; // node grid sits on the ground, the drone keeps its own height
        Vector3 dir = point - transform.position;
        if (dir.magnitude < 0.3f)
        {
            _pathIndex++;
            if (_pathIndex >= _path.Count)
            {
                _path = null;
                return BTStatus.Success;
            }
            return BTStatus.Running;
        }

        var moveDir = _obstacleAvoidance.GetDir(dir.normalized, false);
        _model.Move(moveDir);
        _model.LookDir(moveDir);
        return BTStatus.Running;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector3.down * killRayDistance);
    }
}
