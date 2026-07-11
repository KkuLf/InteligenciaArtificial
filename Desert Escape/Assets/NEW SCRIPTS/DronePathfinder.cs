using System.Collections.Generic;
using UnityEngine;

// Runs the generic AStar over the same Node grid the Enemy patrols on, but for the
// Drone: used when the drone's straight line to its target is blocked and it needs
// an actual route instead of pushing against walls.
public class DronePathfinder : MonoBehaviour
{
    public float radius = 3;
    public LayerMask maskNodes;
    public LayerMask maskObs;

    List<Vector3> _lastPath; // kept only so the gizmo can draw the current route

    public List<Vector3> GetPath(Vector3 from, Vector3 to)
    {
        var start = GetNearNode(from);
        var goal = GetNearNode(to);
        if (start == null || goal == null) return null;

        List<Node> path = AStar.Run(start,
            current => current.neighbours,
            current => current == goal,
            (parent, child) => Vector3.Distance(parent.transform.position, child.transform.position),
            current => Vector3.Distance(current.transform.position, goal.transform.position));

        if (path == null || path.Count == 0) return null;

        var result = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            result.Add(path[i].transform.position);
        }
        _lastPath = result;
        return result;
    }

    Node GetNearNode(Vector3 pos)
    {
        var nodes = Physics.OverlapSphere(pos, radius, maskNodes);
        Node nearNode = null;
        float nearDistance = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            var currentNode = nodes[i];
            var dir = currentNode.transform.position - pos;
            float currentDistance = dir.magnitude;
            if (nearNode == null || currentDistance < nearDistance)
            {
                if (!Physics.Raycast(pos, dir.normalized, currentDistance, maskObs))
                {
                    nearNode = currentNode.GetComponent<Node>();
                    nearDistance = currentDistance;
                }
            }
        }
        return nearNode;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        if (_lastPath == null) return;
        for (int i = 0; i < _lastPath.Count - 1; i++)
        {
            Gizmos.DrawLine(_lastPath[i], _lastPath[i + 1]);
        }
    }
}
