using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс пула для управления пулями. Позволяет переиспользовать объекты-пули вместо их постоянного создания и удаления.
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    private GameObject bulletPrefab; // Префаб пули
    private Transform bulletsParent; // GameObject для хранения пуль

    private Queue<GameObject> bulletPool; // Пул для хранения очереди пуль

    private void Awake()
    {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        bulletsParent = GameObject.Find("BulletPool")?.transform;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        bulletPool = new Queue<GameObject>();
        CreateAndAddToPool(1);
    }

    /// <summary>
    /// Создаёт указанное количество новых объектов пули и добавляет их в пул.
    /// </summary>
    /// <param name="count">Количество объектов, которые нужно создать.</param>
    private void CreateAndAddToPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletsParent);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    /// <summary>
    /// Возвращает первый неактивный объект из пула. Если таких нет — создаёт новый.
    /// </summary>
    /// <returns>Объект пули, готовый к использованию.</returns>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            GameObject bullet = bulletPool.Dequeue();
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
            else
            {
                bulletPool.Enqueue(bullet);
            }
        }
        CreateAndAddToPool(1);
        return bulletPool.Dequeue();
    }

    /// <summary>
    /// Возвращает пулю обратно в пул, деактивируя её.
    /// </summary>
    /// <param name="bullet">Пуля, которую нужно вернуть в пул.</param>
    public void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}