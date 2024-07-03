using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IPoints
{
    public GameObject obj;
    public void SetWayPoints(List<Node> newPoints)
    {
        if (newPoints.Count == 0) return;
        obj.SetActive(true);
        var pos = newPoints[newPoints.Count - 1].transform.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }
    public void SetWayPoints(List<Vector3> newPoints)
    {
        if (newPoints.Count == 0) return;
        obj.SetActive(true);
        var pos = newPoints[newPoints.Count - 1];
        pos.y = transform.position.y;
        transform.position = pos;
    }
}
