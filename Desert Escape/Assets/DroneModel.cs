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
        // Building the target rotation with Quaternion.LookRotation(dir, Vector3.up)
        // pins "up" to world-up every frame, so even if the drone ended up rolled onto
        // its side (e.g. from physics before rotation was frozen) it corrects itself -
        // just Lerp-ing transform.forward alone doesn't fix an existing roll.
        dir.y = 0;
        if (dir.x == 0 && dir.z == 0) return;
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedRot);
    }

    // Re-levels whatever the drone is currently facing (keeps yaw, strips any pitch/roll).
    // Needed for states like Idle that don't call LookDir with a real target direction -
    // without this, a drone that ended up tilted stays tilted forever while idling.
    public void LevelOut()
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude < 0.0001f) return;
        Quaternion targetRotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedRot);
    }
}
