using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBehaviour : MonoBehaviour, IFlockingBehaviour
{
    public float multiplier;
    public float predatorRange = 2;
    public int predatorMax = 5;
    public LayerMask predatorMask;
    Collider[] _colliders;

    private void Awake()
    {
        _colliders = new Collider[predatorMax];
    }
    public Vector3 GetDir(List<IBoid> boids, IBoid self)
    {
        int count = Physics.OverlapSphereNonAlloc(self.Position, predatorRange, _colliders, predatorMask);
        Vector3 predatorDir = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            var diff = self.Position - _colliders[i].transform.position;
            predatorDir += diff.normalized * (predatorRange - diff.magnitude);
        }
        return predatorDir.normalized * multiplier;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, predatorRange);
    }
}
