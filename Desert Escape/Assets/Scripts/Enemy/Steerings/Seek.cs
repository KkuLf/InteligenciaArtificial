// Seek.cs
using UnityEngine;

public class Seek : ISteering
{
    private Transform entity;
    private Transform target;

    public Seek(Transform entity, Transform target)
    {
        this.entity = entity;
        this.target = target;
    }

    public Vector3 GetDir()
    {
        return (target.position - entity.position).normalized;
    }
}
