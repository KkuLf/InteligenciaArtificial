using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroneController : MonoBehaviour
{
    public Rigidbody target;
    public float timePrediction;
    public float angle;
    public float radius;
    public LayerMask maskObs;
    FSM<StatesEnum> _fsm;
    ISteering _steering;
    Drone _drone;
    ObstacleAvoidanceV2 _obstacleAvoidance;
    private void Awake()
    {
        _drone = GetComponent<Drone>();
        InitializeSteerings();
        InitializeFSM();
    }

    void InitializeSteerings()
    {
        //var seek = new Seek(_drone.transform, target.transform);
        //var flee = new Flee(_drone.transform, target.transform);
        //var pursuit = new Pursuit(_drone.transform, target, timePrediction);
        //var evade = new Evade(_drone.transform, target, timePrediction);
        _steering = GetComponent<FlockingManager>();
        _obstacleAvoidance = new ObstacleAvoidanceV2(_drone.transform, angle, radius, maskObs);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();

        var idle = new DroneStateIdle<StatesEnum>();
        var steering = new DroneStateSteering<StatesEnum>(_drone, _steering, _obstacleAvoidance);

        idle.AddTransition(StatesEnum.Walk, steering);
        steering.AddTransition(StatesEnum.Idle, idle);

        _fsm.SetInit(steering);
    }
    void Update()
    {
        _fsm.OnUpdate();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);
    }
}