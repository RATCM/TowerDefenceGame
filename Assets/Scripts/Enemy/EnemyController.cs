using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // TODO convert this to a JSON file instead of defining the enemies for each round in the inspector
    [System.Serializable]
    internal class EnemyClass
    {
        [SerializeField] internal GameObject Enemy;
        [SerializeField] internal float WaitTime = 1;
    }
    [System.Serializable]
    internal class EnemyList
    {
        [SerializeField] internal List<EnemyClass> Enemies; // This should function as a queue
    }
    [SerializeField] private List<EnemyList> NextEnemies = new List<EnemyList>();


    [HideInInspector] private float timePassed = 0;

    private void Awake()
    {
        Global.MaxRounds = NextEnemies.Count;
    }

    // TODO refactor
    void FixedUpdate()
    {
        if (NextEnemies[(PlayerInfo.CurrentRound-1) % (Global.MaxRounds)].Enemies.Count == 0 && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            Global.RoundInProgress = false;
            PlayerInfo.CurrentRound++;
        }

        if (NextEnemies[(PlayerInfo.CurrentRound - 1) % (Global.MaxRounds)].Enemies.Count == 0)
            return;

        if (Global.RoundInProgress)
        {
            timePassed += Time.fixedDeltaTime;

            if(timePassed >= NextEnemies[PlayerInfo.CurrentRound - 1].Enemies[0].WaitTime)
            {
                var enemy = Instantiate(NextEnemies[PlayerInfo.CurrentRound - 1].Enemies[0].Enemy);

                enemy.transform.position = Global.SpawnLocations[0];
                NextEnemies[PlayerInfo.CurrentRound - 1].Enemies.RemoveAt(0);

                timePassed = 0;
            }
        }
    }
}