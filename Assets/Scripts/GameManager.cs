using TMPro;
using UnityEngine;

/// <summary>
/// Управляет игровым процессом: отслеживает уровень, количество убитых врагов,
/// состояние победы/поражения и перезапуск уровня.
/// </summary>
public class GameManager : MonoBehaviour
{
    private Transform levelCounterText; // Текст счётчика уровней
    private Transform enemiesKilledCounter; // Текст счётчика убитых врагов
    private Transform levelPassedText; // Текст статуса пройден ли уровень
    private GameObject endScreen; // Экран конца уровня
    private EnemySpawner enemySpawner;
    private Player player;
    private bool isPassed = false; // Переменная для определения пройден ли уровень
    private int currentLevel = 1;
    private int enemiesKilled = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        endScreen = GameObject.Find("EndScreen");
        levelCounterText = endScreen.transform.Find("LevelText");
        levelPassedText = endScreen.transform.Find("LevelPassedText");
        enemiesKilledCounter = endScreen.transform.Find("EnemiesKilled");
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
    }

    private void OnEnable()
    {
        player.OnPlayerDeath.AddListener(ShowVictoryScreen);
    }

    private void OnDisable()
    {
        player.OnPlayerDeath.RemoveListener(ShowVictoryScreen);
    }

    private void Start()
    {
        endScreen.SetActive(false);
    }

    /// <summary>
    /// Отображает экран конца уровня с результатами.
    /// Обновляет тексты: номер уровня, количество убитых врагов, статус прохождения.
    /// </summary>
    public void ShowVictoryScreen()
    {
        enemiesKilled = enemySpawner.GetKilledEnemies();
        enemiesKilled = isPassed ? enemiesKilled : enemiesKilled - 1;

        endScreen.SetActive(true);

        levelCounterText.GetComponent<TMP_Text>().text = $"Level {currentLevel}";
        enemiesKilledCounter.GetComponent<TMP_Text>().text = $"Enemies Killed: \n{enemiesKilled}";
        levelPassedText.GetComponent<TMP_Text>().text = "Level Failed";

        if (isPassed)
        {
            levelPassedText.GetComponent<TMP_Text>().text = "Level Passed";
        }
    }

    /// <summary>
    /// Начинает новый уровень и задаёт параметры уровня.
    /// </summary>
    public void StartNewLevel()
    {
        currentLevel = isPassed ? currentLevel + 1 : 1;

        enemySpawner.SetEnemiesToSpawn(currentLevel);
        enemySpawner.ResetSpawnedEnemies();
        player.SetHealth(player.GetMaxHealth());

        isPassed = false;

        endScreen.SetActive(false);
    }

    /// <summary>
    /// Устанавливает флаг завершения уровня.
    /// </summary>
    /// <param name="value">Значение, указывающее, был ли уровень пройден.</param>
    public void SetLevelPassed(bool value)
    {
        isPassed = value;
    }

    /// <summary>
    /// Устанавливает количество убитых врагов.
    /// </summary>
    /// <param name="value">Число убитых врагов.</param>
    public void SetEnemiesKilled(int value)
    {
        enemiesKilled = value;
    }

    /// <summary>
    /// Возвращает текущее количество убитых врагов.
    /// </summary>
    /// <returns>Количество убитых врагов.</returns>
    public int GetEnemiesKilled()
    {
        return enemiesKilled;
    }
}