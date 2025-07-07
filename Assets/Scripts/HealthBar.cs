using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отвечает за отображение здоровья игрока через UI-элемент Slider.
/// Следит за событием изменения здоровья и обновляет индикатор.
/// </summary>
public class HealthBar : MonoBehaviour
{
    private Slider healthSlider;
    private Player player;

    /// <summary>
    /// Инициализация ссылок на компоненты.
    /// </summary>
    private void Awake()
    {
        healthSlider = GetComponent<Slider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    /// <summary>
    /// Подписка на событие изменения здоровья игрока.
    /// </summary>
    private void OnEnable()
    {
        player.OnHealthChanged.AddListener(UpdateHealthBar);
        UpdateHealthBar();
    }

    /// <summary>
    /// Отписка от события изменения здоровья при деактивации объекта.
    /// </summary>
    private void OnDisable()
    {
        player.OnHealthChanged.RemoveListener(UpdateHealthBar);
    }

    /// <summary>
    /// Обновляет значение полосы жизни в зависимости от текущего процента здоровья игрока.
    /// </summary>
    private void UpdateHealthBar()
    {
        healthSlider.value = player.GetHealthPercentage();
    }
}