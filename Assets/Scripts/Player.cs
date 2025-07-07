using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Класс игрока. Управляет здоровьем, нанесением урона и событиями смерти.
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    private float maxHealth = 100f; // Максимальное здоровье игрока
    private float currentHealth; // Текущее здоровье игрока

    /// <summary>
    /// Событие, вызывающееся при изменении здоровья игрока.
    /// </summary>
    public UnityEvent OnHealthChanged = new UnityEvent();

    /// <summary>
    /// Событие, вызывающееся при смерти игрока.
    /// </summary>
    public UnityEvent OnPlayerDeath = new UnityEvent();

    private void Start()
    {
        SetHealth(maxHealth);
    }

    /// <summary>
    /// Наносит урон игроку, ограничивает здоровье до 0.
    /// При достижении 0 здоровья — вызывает событие смерти.
    /// </summary>
    /// <param name="damage">Количество урона.</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged.Invoke();

        if (currentHealth == 0)
        {
            OnPlayerDeath.Invoke();
        }
    }

    /// <summary>
    /// Возвращает текущий уровень здоровья в процентах от максимального.
    /// </summary>
    /// <returns>Значение здоровья от 0 до 1.</returns>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// Устанавливает новое значение текущего здоровья игрока.
    /// Ограничивает его диапазоном от 0 до максимального.
    /// </summary>
    /// <param name="value">Новое значение здоровья.</param>
    public void SetHealth(float value)
    {
        currentHealth = value;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged.Invoke();
    }

    /// <summary>
    /// Возвращает текущее значение здоровья игрока.
    /// </summary>
    /// <returns>Текущее здоровье.</returns>
    public float GetHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Возвращает максимальное значение здоровья игрока.
    /// </summary>
    /// <returns>Максимальное здоровье.</returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}