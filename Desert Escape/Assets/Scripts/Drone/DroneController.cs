using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class DroneController : MonoBehaviour
{

    private PlayerController1 playerController; // Reference to the PlayerController script

    public Rigidbody target;
    public float timePrediction;
    public float angle;
    public float radius;
    public LayerMask maskObs;
    FSM<StatesEnum> _fsm;
    ISteering _steering;
    Drone _drone;
    ObstacleAvoidanceV2 _obstacleAvoidance;

    public float detectionRadius = 10f; // Radius of the cone
    public float detectionAngle = 45f;  // Angle of the cone
    public LayerMask detectionLayer;    // LayerMask to filter the raycasting to specific layers (e.g., Player layer)

    private void Awake()
    {
        _drone = GetComponent<Drone>();
        InitializeSteerings();
        InitializeFSM();
    }

    private void Start()
    {
        // Find and store the PlayerController component
        playerController = FindObjectOfType<PlayerController1>();
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
        DetectPlayer();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 forward = transform.forward;
        Vector3 down = Vector3.down;

        for (float angle = -detectionAngle; angle <= detectionAngle; angle += 5f)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, transform.right);
            Vector3 direction = rotation * down;
            Gizmos.DrawRay(transform.position, direction * detectionRadius);
        }
    }

    void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            Vector3 directionToTarget = hitCollider.transform.position - transform.position;
            float angleToTarget = Vector3.Angle(Vector3.down, directionToTarget);

            if (angleToTarget < detectionAngle)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    // Player detected within the cone
                    PlayerDetected();
                }
            }
        }
    }

    void PlayerDetected()
    {
        // Load the "GameOver" scene
        SceneManager.LoadScene("GameOver");

        // Activate cursor using PlayerController
        playerController.ActivateCursor();
    }

}