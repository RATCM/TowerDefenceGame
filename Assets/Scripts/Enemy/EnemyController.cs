using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // TODO convert this to a JSON file instead of defining the enemies for each round in the inspector
    [System.Serializable]
    internal class EnemyClass
    {
        [SerializeField] internal GameObject Enemy;
        [SerializeField] internal float SpawnInterval = 1;
        [SerializeField] internal long SpawnCount = 1;
    }
    [System.Serializable]
    internal class EnemyList
    {
        [SerializeField] internal List<EnemyClass> Enemies; // This should function as a queue
    }
    [SerializeField] private List<EnemyList> NextEnemies = new List<EnemyList>();
    private List<EnemyList> _nextEnemies;

    [HideInInspector] private float timePassed = 0;

    private void Awake()
    {
        Global.MaxRounds = NextEnemies.Count;
        _nextEnemies = NextEnemies;
    }

    void CheckIfRoundIsOver()
    {
        if (NoEnemiesLeftInQueue() && NoEnemiesInScene())
            Global.EndRound();
    }

    bool NoEnemiesLeftInQueue() =>
        _nextEnemies[(PlayerInfo.CurrentRound - 1) % (Global.MaxRounds)].Enemies.Count == 0;

    bool NoEnemiesInScene() =>
        GameObject.FindGameObjectsWithTag("Enemy").Length == 0;

    void SpawnEnemy()
    {
        var enemy = Instantiate(_nextEnemies[PlayerInfo.CurrentRound - 1].Enemies[0].Enemy);

        enemy.transform.position = Global.SpawnLocations[0];
        _nextEnemies[PlayerInfo.CurrentRound - 1].Enemies[0].SpawnCount--;
        if (_nextEnemies[PlayerInfo.CurrentRound - 1].Enemies[0].SpawnCount == 0)
        {
            _nextEnemies[PlayerInfo.CurrentRound - 1].Enemies.RemoveAt(0);
        }

        timePassed = 0;
    }

    void UpdateTimePassed() =>
        timePassed += Time.fixedDeltaTime;

    bool NextEnemyReadyToSpawn() =>
        timePassed >= _nextEnemies[PlayerInfo.CurrentRound - 1].Enemies[0].SpawnInterval;

    void FixedUpdate()
    {
        if(!Global.RoundInProgress)
            return;

        CheckIfRoundIsOver();

        if (NoEnemiesLeftInQueue()) // Dont spawn new enemies if there are none left in the queue
            return;

        if (Global.RoundInProgress)
        {
            UpdateTimePassed();

            if(NextEnemyReadyToSpawn())
                SpawnEnemy();
        }
    }
}
