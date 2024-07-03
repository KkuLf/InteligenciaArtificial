using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{

    [SerializeField] private float speed;
    Rigidbody _rb;

    public Node[] waypoints; // Array to hold the patrol waypoints
    public Node currentWayPoint;
    public int currentWaypointIndex = 0; // Index of the current waypoint
    public int index;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 dir)
    {
        dir *= speed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }

    public void LookDir(Vector3 dir)
    {
        if (dir.x == 0 && dir.z == 0) return;
        transform.forward = dir;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Attack()
    {
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        // Add bullet logic
        yield return true;
    }

}
