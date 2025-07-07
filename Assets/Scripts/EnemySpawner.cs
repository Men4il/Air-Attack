using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Отвечает за спавн врагов вокруг игрока. Управляет количеством, интервалом и положением врагов.
/// Также отслеживает количество живых и убитых врагов на уровне.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    private float minRadius = 25f; // Минимальный радиус от игрока для спавна врагов
    private float maxRadius = 55f; // Максимальный радиус от игрока для спавна врагов
    private float minHeight = 5f; // Минимальная высота от игрока для спавна врагов
    private float maxHeight = 15f; // Максимальная высота от игрока для спавна врагов
    private float spawnInterval = 5f; // Интервал между спавнами врагов
    private int enemiesToSpawn = 1; // Количество врагов для спавна на данном уровне
    private int spawnedEnemiesCounter = 0;

    private List<GameObject> enemyPrefabs; // Префабы врагов
    private GameObject enemiesParent; // GameObject для хранения врагов
    private GameManager gameManager;

    private void Awake()
    {
        enemyPrefabs = new List<GameObject>
        {
            Resources.Load<GameObject>("Prefabs/Su57")
        };

        enemiesParent = GameObject.Find("EnemiesPool");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Корутина, которая последовательно спавнит врагов с заданным интервалом.
    /// </summary>
    private IEnumerator SpawnEnemies()
    {
        while (spawnedEnemiesCounter < enemiesToSpawn)
        {
            SpawnRandomEnemy();
            spawnedEnemiesCounter++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Спавнит случайного врага в случайном положении вокруг игрока.
    /// </summary>
    private void SpawnRandomEnemy()
    {
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count - 1)];

        GameObject enemyGO = Instantiate(enemyPrefab, enemiesParent.transform);
        enemyGO.GetComponent<EnemyLogic>().SetFlightHeight(Random.Range(minHeight, maxHeight));
        enemyGO.GetComponent<EnemyLogic>().SetOrbitRadius(Random.Range(minRadius, maxRadius));
    }

    /// <summary>
    /// Возвращает общее количество врагов, которые должны быть заспавнены на текущем уровне.
    /// </summary>
    /// <returns>Число врагов.</returns>
    public int GetEnemiesToSpawn()
    {
        return enemiesToSpawn;
    }

    /// <summary>
    /// Устанавливает новое количество врагов для спавна на текущем уровне.
    /// </summary>
    /// <param name="amount">Количество врагов.</param>
    public void SetEnemiesToSpawn(int amount)
    {
        enemiesToSpawn = amount;
    }

    /// <summary>
    /// Возвращает количество уже заспавненных врагов.
    /// </summary>
    /// <returns>Количество спавненных врагов.</returns>
    public int GetSpawnedEnemies()
    {
        return spawnedEnemiesCounter;
    }

    /// <summary>
    /// Устанавливает количество заспавненных врагов.
    /// </summary>
    /// <param name="amount">Новое значение.</param>
    public void SetSpawnedEnemies(int amount)
    {
        spawnedEnemiesCounter = amount;
    }

    /// <summary>
    /// Сбрасывает счётчик заспавненных врагов и удаляет всех существующих врагов.
    /// Затем начинает спавн заново.
    /// </summary>
    public void ResetSpawnedEnemies()
    {
        spawnedEnemiesCounter = 0;

        for (int i = 0; i < enemiesParent.transform.childCount; i++)
        {
            Destroy(enemiesParent.transform.GetChild(i).gameObject);
        }

        StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Возвращает количество убитых врагов на текущем уровне.
    /// </summary>
    /// <returns>Количество убитых врагов.</returns>
    public int GetKilledEnemies()
    {
        return spawnedEnemiesCounter - (enemiesParent.transform.childCount - 1);
    }

    /// <summary>
    /// Проверяет, все ли враги мертвы, и если да — вызывает экран победы.
    /// </summary>
    public void CheckDeadEnemies()
    {
        if (enemiesParent.transform.childCount == 1 && enemiesToSpawn == spawnedEnemiesCounter)
        {
            gameManager.SetLevelPassed(true);
            gameManager.ShowVictoryScreen();
        }
    }
}