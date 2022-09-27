using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;
internal enum PathArgs
{
    [Description("Weather the object should follow the layer")]
    Follow,
    [Description("Weather the object should avoid the layer")]
    Avoid,
}
public class PathCreation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject Path;
    [SerializeField] private GameObject EndWaypoint;
    [SerializeField] private bool UseRotation = false;
    [SerializeField] private PathArgs pathArgs;
    [SerializeField] private LayerMask Layers;
    

    private List<Vector2> _waypoints = new List<Vector2>();
    private List<Vector2> totalPath;

    void Awake()
    {
        var waypoints = Path.GetComponentsInChildren<Transform>().Where(x => x != Path.transform).ToList();
        waypoints.Remove(EndWaypoint.transform);

        waypoints.ForEach(x => _waypoints.Add(x.position));

        // Sort list by distance to player
        _waypoints.Sort(delegate (Vector2 x, Vector2 y)
        {
            return Dist(transform.position, x).CompareTo(Dist(transform.position, y));
        });

    }

    private void Start()
    {
        totalPath = CalcTotalPath();
    }
    private bool IsAboveLayers(Vector2 pos)
    {
        //foreach (var layer in Layers)
        //{
        var hit = Physics2D.BoxCast(pos, EndWaypoint.transform.lossyScale, 0, Vector2.zero, 0, Layers);

        if (hit)
        {
            Debug.Log("Hit detected!");
            return hit && pathArgs == PathArgs.Avoid;
        }
        //}
        return pathArgs == PathArgs.Follow;
    }
    private bool IsIntersectingLayers(Vector2 start, Vector2 end)
    {
        //foreach (var layer in Layers)
        //{
        var hit = Physics2D.Linecast(start, end, Layers);

        if (hit)
        {
            Debug.Log("Hit detected!");
            return hit && pathArgs == PathArgs.Avoid;
        }
        //}
        return pathArgs == PathArgs.Follow;
    }

    //private Vector2? NextPath(List<Vector2> list)
    //{
    //    Debug.Log("Calculating next waypoint...");
    //    foreach (var vec in list)
    //    {
    //        if (!IsIntersectingLayers(transform.position, vec))
    //        {
    //            return vec;
    //        }

    //    }
    //    return null;
    //}
    private Vector2? NextPath(List<Vector2> list)
    {
        foreach(var vec in list)
        {
            if(!IsAboveLayers(vec))
            {
                return vec;
            }
        }
        return null;
    }
    private float Dist(Vector2 vec1, Vector2 vec2) =>
        Mathf.Sqrt(Mathf.Pow(vec1.x - vec2.x, 2) + Mathf.Pow(vec1.y - vec2.y, 2));

    private RaycastHit2D? GoingToHitObstacle(Vector2 dir)
    {
        var hit = Physics2D.CircleCast(transform.position, transform.lossyScale.x / 2, dir, dir.magnitude, Layers);

        if ((!hit && pathArgs == PathArgs.Follow) || (hit && pathArgs == PathArgs.Avoid))
            return hit;

        return null;
    }

    private List<Vector2> CalcTotalPath()
    {
        List<Vector2> path = new List<Vector2>();

        Vector2? pos = transform.position;

        while (!_waypoints.IsNullOrEmpty())
        {
            pos = NextPath(_waypoints);
            if (pos == null) break;

            _waypoints.Remove(pos.Value);
            path.Add(pos.Value);
        }

        if (!path.IsNullOrEmpty())
        {
            var end = NextPath(new List<Vector2>() { EndWaypoint.transform.position });
            if(end != null)
            {
                path.Add(end.Value);
            }
        }
        return path;
    }
    void FixedUpdate()
    {
        const float speedMultiplier = 0.01f;

        if(!totalPath.IsNullOrEmpty())
        {
            Vector2 dir = (totalPath[0] - (Vector2)transform.position).normalized * speed * speedMultiplier;

            if (UseRotation)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);

            var hit = GoingToHitObstacle(dir);

            if (hit.Detected())
            {
                dir += hit.Value.normal * speed * speedMultiplier;
                Debug.Log("Hit detected!");
            }

            transform.position += (Vector3)dir;

            if (Dist(totalPath[0], (Vector2)transform.position) < 0.1f)
            {
                totalPath.RemoveAt(0);
            }
        }
    }
}
