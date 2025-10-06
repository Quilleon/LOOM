using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    private Transform[] spawnPoints;
    private Transform[] spawnedEnemies;

    private bool _hasSpawnedEnemies;
    void Start()
    {
        // Array of all children
        var childNum = transform.childCount;
        spawnPoints = new Transform[childNum-1];
        for (int i = 1; i < childNum; i++) // Ignore door
            spawnPoints[i-1] = transform.GetChild(i);
        
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        if (AllEnemiesKilled())
        {
            print("All Enemies Killed, opening the gate");
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
    }

    public void SpawnEnemies()
    {
        foreach (var point in spawnPoints)
        {
            int random = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[random], point);
        }

        _hasSpawnedEnemies = true;
    }

    private bool AllEnemiesKilled()
    {
        if (!_hasSpawnedEnemies)
            return false;
        
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

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
