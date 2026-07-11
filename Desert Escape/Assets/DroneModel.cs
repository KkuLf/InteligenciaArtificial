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
        dir.y = 0;
        if (dir.x == 0 && dir.z == 0) return;
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedRot);
    }

    public void LevelOut()
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude < 0.0001f) return;
        Quaternion targetRotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedRot);
    }
}
