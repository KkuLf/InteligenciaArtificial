using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPatrolState<T> : State<T>, IPoints
{
    EnemyBlackboard _bb;
    List<Vector3> _waypoints;
    int _nextPoint = 0;
    bool _isFinishPath = true;

    public EnemyPatrolState(EnemyBlackboard blackboard)
    {
        _bb = blackboard;
    }

    public override void Execute()
    {
        base.Execute();
        Run();
    }

    public override void Sleep()
    {
        base.Sleep();
    }

    public void SetWayPoints(List<Node> newPoints)
    {
        var list = new List<Vector3>();
        for (int i = 0; i < newPoints.Count; i++)
        {
            list.Add(newPoints[i].transform.position);
        }
        SetWayPoints(list);
    }

    public void SetWayPoints(List<Vector3> newPoints)
    {
        _nextPoint = 0;
        if (newPoints.Count == 0) return;
        //_anim.Play("CIA_Idle");
        _waypoints = newPoints;
        var pos = _waypoints[_nextPoint];
        pos.y = _bb.Model.transform.position.y;
        _bb.Model.SetPosition(pos);
        _isFinishPath = false;
    }

    void Run()
    {
        if (IsFinishPath) return;
        var point = _waypoints[_nextPoint];
        var posPoint = point;
        posPoint.y = _bb.Model.transform.position.y;
        Vector3 dir = posPoint - _bb.Model.transform.position;
        if (dir.magnitude < 0.2f)
        {
            if (_nextPoint + 1 < _waypoints.Count)
            {
                _nextPoint++;
            }
            else
            {
                _isFinishPath = true;
                return;
            }
        }
        Vector3 avoidDir = _bb.ObstacleAvoidance != null ? _bb.ObstacleAvoidance.GetDir(dir.normalized, false) : dir.normalized;
        _bb.Model.Move(avoidDir);
        _bb.Model.LookDir(avoidDir);
    }

    public bool IsFinishPath => _isFinishPath;

    private int SelectNextWaypoint()
    {
        List<float> probabilities = CalculateProbabilities();
        return RouletteWheelSelection(probabilities);
    }

    private List<float> CalculateProbabilities()
    {
        List<float> distances = new List<float>();
        foreach (Vector3 waypoint in _waypoints)
        {
            float distance = Vector3.Distance(_bb.Model.transform.position, waypoint);
            distances.Add(distance);
        }
        List<float> probabilities = new List<float>();
        float totalDistance = distances.Sum();

        foreach (float distance in distances)
        {
            float probability = 1f - (distance / totalDistance);
            probabilities.Add(probability);
        }

        return probabilities;
    }

    private int RouletteWheelSelection(List<float> probabilities)
    {
        float randomValue = UnityEngine.Random.value;
        float cumulativeProbability = 0;

        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return i;
            }
        }
        Debug.LogError("No se pudo seleccionar un waypoint.");
        return -1;
    }
}
