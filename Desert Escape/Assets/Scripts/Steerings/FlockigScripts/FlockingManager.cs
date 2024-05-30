using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour, ISteering
{
    public float radius;
    public int maxBoids = 1;
    public LayerMask boidMask;
    IFlockingBehaviour[] _flockings;
    IBoid _self;
    Collider[] _colliders;
    List<IBoid> _boids;
    private void Awake()
    {
        _self = GetComponent<IBoid>();
        _flockings = GetComponents<IFlockingBehaviour>();
        _colliders = new Collider[maxBoids];
        _boids = new List<IBoid>();
    }
    public Vector3 GetDir()
    {
        _boids.Clear();
        int count = Physics.OverlapSphereNonAlloc(_self.Position, radius, _colliders, boidMask);
        for (int i = 0; i < count; i++)
        {
            var currBoid = _colliders[i].GetComponent<IBoid>();
            //print(currBoid);
            if (currBoid != null && currBoid != _self)
                _boids.Add(currBoid);
        }
        Vector3 dir = Vector3.zero;
        for (int i = 0; i < _flockings.Length; i++)
        {
            var flockBehaviour = _flockings[i];

            dir += flockBehaviour.GetDir(_boids, _self);
        }
        return dir.normalized;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
