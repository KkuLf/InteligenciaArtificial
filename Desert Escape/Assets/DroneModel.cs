using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneModel : MonoBehaviour, IBoid
{
    [SerializeField] private float speed;
    [SerializeField] private float speedRot;
    Rigidbody _rb;

    public Vector3 Position => transform.position;
    public Vector3 Front => transform.forward;

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
        // Keep the drone level: only yaw to face the horizontal direction, don't let
        // vertical steering (climbing/avoiding obstacles) pitch or roll the body.
        dir.y = 0;
        if (dir.x == 0 && dir.z == 0) return;
        transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * speedRot);

    }
}
