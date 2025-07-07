using UnityEngine;

/// <summary>
/// Класс пули. Обрабатывает движение, столкновения, нанесение урона и возврат пули в пул.
/// </summary>
public class Bullet : MonoBehaviour
{
    private float speed = 100f; // Скорость полёта пули
    private float lifeTime = 3f; // Время исчезновения пули
    private float damage = 5f; // Урон пули

    private Rigidbody rb;

    /// <summary>
    /// Получает компонент Rigidbody для управления физикой пули.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Активирует таймер на возврат пули в пул после заданного времени жизни.
    /// </summary>
    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    /// <summary>
    /// Обрабатывает столкновение пули с объектом. Наносит урон врагу, если это необходимо,
    /// и возвращает пулю обратно в пул.
    /// </summary>
    /// <param name="other">Коллайдер объекта, с которым произошло столкновение.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent?.CompareTag("Enemy") == true)
        {
            EnemyLogic enemy = other.GetComponentInParent<EnemyLogic>();

            if (enemy != null)
            {
                // Наносим урон
                enemy.TakeDamage(damage);
            }
        }

        BulletPool.Instance.ReturnToPool(gameObject);
    }

    /// <summary>
    /// Сбрасывает позицию и направление пули, активирует её и запускает новый таймер на возврат в пул.
    /// </summary>
    /// <param name="position">Новая позиция пули.</param>
    /// <param name="newDirection">Новое направление движения пули.</param>
    public void ResetPositionAndDirection(Vector3 position, Vector3 newDirection)
    {
        CancelInvoke();
        transform.position = position;

        // Сбрасываем скорости
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Устанавливаем скорость
        rb.linearVelocity = newDirection * speed;
        transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);

        gameObject.SetActive(true);
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    /// <summary>
    /// Деактивирует объект пули и возвращает его в пул.
    /// </summary>
    private void ReturnToPool()
    {
        CancelInvoke(); // Останавливаем таймер
        gameObject.SetActive(false);
        BulletPool.Instance.ReturnToPool(gameObject);
    }
}