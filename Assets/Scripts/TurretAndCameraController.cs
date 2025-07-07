using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Контроллер для управления турелью и камерой игрока.
/// Обрабатывает повороты турели, наведение пушки и стрельбу.
/// </summary>
public class TurretAndCameraController : MonoBehaviour
{
    [Header("Controller Settings")]
    private float lookSpeed = 5f; // Скорость вращения камерой
    private float fireRate = 0.005f; // Время между выстрелами

    [Header("Input Actions")]
    private InputAction lookAction;
    private InputAction attackAction;

    private BulletSpawner bulletSpawner;
    private Transform gun; // Дуло пушки
    private Transform shootPoint; // Точка вылета пули
    private bool isShooting = false;
    private float timeSinceLastShot = 0f;

    /// <summary>
    /// Инициализация ссылок на объекты и настройка действий ввода.
    /// </summary>
    private void Awake()
    {
        gun = transform.Find("CannonLever")?.transform;
        shootPoint = GameObject.Find("ShootPoint")?.transform;
        var inputs = Resources.Load<InputActionAsset>("InputSystem_Actions");
        lookAction = inputs.FindActionMap("Player")?.FindAction("Look");
        attackAction = inputs.FindActionMap("Player")?.FindAction("Attack");

        attackAction.performed += ctx => StartShooting();
        attackAction.canceled += ctx => StopShooting();
    }

    private void Start()
    {
        bulletSpawner = FindFirstObjectByType<BulletSpawner>();
    }

    private void OnEnable()
    {
        lookAction.Enable();
        attackAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
        attackAction.Disable();
    }

    private void Update()
    {
        HandleRotation();
        HandleShooting();
    }

    /// <summary>
    /// Обработка поворота турели по горизонтали (всё тело) и вертикали (пушка).
    /// </summary>
    private void HandleRotation()
    {
        Vector2 lookDelta = lookAction.ReadValue<Vector2>();

        // Горизонтальный поворот (всё тело)
        float horizontalLook = lookDelta.x * lookSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, horizontalLook);

        // Вертикальный поворот (только пушка)
        float verticalLook = -lookDelta.y * lookSpeed * Time.deltaTime;
        gun.Rotate(Vector3.right, verticalLook);

        LimitGunVerticalRotation();
    }

    /// <summary>
    /// Ограничение угла поворота пушки по вертикали.
    /// Угол не должен выходить за пределы [-40, 40] градусов.
    /// </summary>
    private void LimitGunVerticalRotation()
    {
        Vector3 currentEulerAngles = gun.localEulerAngles;
        float x = currentEulerAngles.x % 360;

        if (x > 180) x -= 360;

        // Ограничение диапазона
        x = Mathf.Clamp(x, -40f, 40f);

        gun.localEulerAngles = new Vector3(x, currentEulerAngles.y, currentEulerAngles.z);
    }

    /// <summary>
    /// Активирует режим стрельбы.
    /// </summary>
    private void StartShooting()
    {
        isShooting = true;
    }

    /// <summary>
    /// Деактивирует режим стрельбы.
    /// </summary>
    private void StopShooting()
    {
        isShooting = false;
    }

    /// <summary>
    /// Выполняет логику стрельбы, если разрешено.
    /// Проверяет прошедшее время и запускает выстрел, если достигнут интервал.
    /// </summary>
    private void HandleShooting()
    {
        if (!isShooting) return;

        timeSinceLastShot += Time.deltaTime;

        if (timeSinceLastShot >= fireRate)
        {
            Shoot();
            timeSinceLastShot = 0f;
        }
    }

    /// <summary>
    /// Выполняет одиночный выстрел из точки shootPoint.
    /// Создаёт пулю из пула и устанавливает её позицию и направление.
    /// </summary>
    private void Shoot()
    {
        Vector3 position = shootPoint.position;
        Vector3 direction = shootPoint.forward;

        GameObject bulletGO = bulletSpawner.GetPooledBullet();
        if (bulletGO != null)
        {
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            bullet.ResetPositionAndDirection(position, direction);
        }
    }
}