using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private List<Vector2> waypoints;
    [HideInInspector] private List<Vector2> CurrentWaypoints;
    [HideInInspector] private float Speed;
    [HideInInspector] private EnemyScript enemyScript;
    [HideInInspector] public Vector2 CurrentDirection;

    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<EnemyScript>();

        CurrentWaypoints = waypoints.Select(x => x).ToList(); // This is used such we dont assign by reference 

        Debug.Log(CurrentWaypoints.Count);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Speed = enemyScript.CurrentSpeed;
        if (CurrentWaypoints.Count == 0) // Enemy has reached destination
        {
            PlayerInfo.Population -= enemyScript.Damage;
            Destroy(gameObject);
            return;
        }

        var dir = (CurrentWaypoints.First()-(Vector2)transform.position);

        CurrentDirection = dir.normalized;

        if(dir.magnitude <= 0.2f)
            CurrentWaypoints.RemoveAt(0);

        transform.Translate(CurrentDirection * Time.fixedDeltaTime * Speed);
    }
}
