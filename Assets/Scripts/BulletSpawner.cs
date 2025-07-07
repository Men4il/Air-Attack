using UnityEngine;

/// <summary>
/// Класс для управления получением пули из пула объектов.
/// </summary>
public class BulletSpawner : MonoBehaviour
{
    /// <summary>
    /// Синглтон экземпляр класса BulletSpawner.
    /// </summary>
    public static BulletSpawner Instance { get; private set; }

    /// <summary>
    /// Инициализация синглтона. Если уже существует инстанс — текущий объект уничтожается.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Возвращает пулю из пула объектов для дальнейшего использования.
    /// </summary>
    /// <returns>Объект пули.</returns>
    public GameObject GetPooledBullet()
    {
        return BulletPool.Instance.GetPooledObject();
    }
}