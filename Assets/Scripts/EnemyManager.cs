using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    private Transform[] spawnPoints;
    private Transform[] spawnedEnemies;
    void Start()
    {
        // Array of all children
        var childNum = transform.childCount;
        spawnPoints = new Transform[childNum];
        for (int i = 0; i < childNum; i++)
            spawnPoints[i] = transform.GetChild(i);
        
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        if (AllEnemiesKilled())
        {
            print("All Enemies Killed, opening the gate");
        }
    }

    public void SpawnEnemies()
    {
        foreach (var point in spawnPoints)
        {
            int random = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[random], point);
        }
    }

    private bool AllEnemiesKilled()
    {
        bool allDead = true;
        foreach (var spawn in spawnPoints)
        {
            if (spawn.childCount > 0)
            {
                allDead = false;
            }
        }
        
        return allDead;
    }
}
