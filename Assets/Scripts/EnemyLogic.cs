using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
/// <summary>
/// Логика поведения врага: орбитальное движение вокруг игрока, атака, смерть и падение.
/// </summary>
public class EnemyLogic : MonoBehaviour
{
    [Header("Enemy Settings")]
    private float health = 100f; // Здоровье врага
    private float damage = 10f; // Урон врага
    private float attackDelay = 5f; // Время между атаками

    [Header("Orbiting Settings")]
    private Transform target; // Цель, вокруг которой летит объект
    private float orbitRadius = 10f; // Радиус орбиты вокруг игрока
    private float orbitSpeed = 10f; // Скорость вращения
    private float minOrbitSpeed = 10f; // Минимальная скорость полёта
    private float maxOrbitSpeed = 60f; // Максимальная скорость полёта
    private float flightHeight = 5f; // Высота полёта над целью
    private float transitionDuration = 3f; // Время между сменами скоростей

    [Header("Turning Settings")]
    private float rotationLerpSpeed = 10f; // Скорость поворота

    [Header("Death Settings")]
    private float fallSpeed = 4f; // Скорость падения
    private float directionSpeed = 15.0f; // Вертикальная скорость при падении
    private GameObject fireEffectPrefab; // Префаб огня
    private float destroyDelay = 3f; // Через сколько исчезнет враг

    private float angle = 0f; // Переменная хранения текущего угла поворота
    private bool isDead = false;
    private Rigidbody rb;
    private Player player;

    private void Awake()
    {
        fireEffectPrefab = Resources.Load<GameObject>("Prefabs/VFX_Fire 1");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();

        if (player == null)
            Debug.LogError("Не найден объект с тегом Player");
    }

    private void Start()
    {
        UpdatePosition();
        StartCoroutine(AttackAfterDelay());
        StartCoroutine(ChangeOrbitSpeedPeriodically());
    }

    private void Update()
    {
        angle += orbitSpeed * Time.deltaTime;
        if (angle >= 360f) angle -= 360f;

        if (!isDead)
        {
            UpdatePosition();
            RotateTowardsMovement();
        }
    }

    /// <summary>
    /// Обновляет позицию врага на основе текущего угла и радиуса орбиты.
    /// </summary>
    private void UpdatePosition()
    {
        float x = Mathf.Cos(Mathf.Deg2Rad * angle) * orbitRadius;
        float z = Mathf.Sin(Mathf.Deg2Rad * angle) * orbitRadius;
        Vector3 position = new Vector3(x, flightHeight, z);
        transform.position = target.position + position;
    }

    /// <summary>
    /// Поворачивает врага в направлении его движения.
    /// </summary>
    private void RotateTowardsMovement()
    {
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0f, 0f, 1f);
        float currentZAngle = transform.localEulerAngles.z;

        if (Mathf.Abs(currentZAngle - 45f) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }

        float tangentAngle = angle + 90f;
        Vector3 direction = new Vector3(
            Mathf.Cos(Mathf.Deg2Rad * tangentAngle),
            0f,
            Mathf.Sin(Mathf.Deg2Rad * tangentAngle)
        );

        Quaternion desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
        float initialZRotation = transform.localEulerAngles.z;
        desiredRotation *= Quaternion.Euler(0f, 0f, initialZRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationLerpSpeed);
    }

    /// <summary>
    /// Корутина, нанесения урона игрока с периодиченостью.
    /// </summary>
    private IEnumerator AttackAfterDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackDelay);
            player.TakeDamage(damage);
        }
    }

    /// <summary>
    /// Корутина, имитирующая падение врага после смерти.
    /// </summary>
    private IEnumerator FallToGround()
    {
        Vector3 fallDirection = transform.forward;
        fallDirection.y = 0f;
        fallDirection.Normalize();

        rb.useGravity = false;
        rb.isKinematic = false;

        Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);
        transform.Rotate(0f, 0f, -45f);

        // Дистанция проверки на наличие земли.
        float raycastDistance = 0.1f;
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        while (transform.position.y > 0.1f)
        {
            // Проверка наличия земли под самолётом и преждевременная остановка его падения.
            if (Physics.Raycast(transform.position, Vector3.down, raycastDistance, groundLayer))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                break;
            }

            Vector3 forwardMovement = fallDirection * directionSpeed * Time.deltaTime;
            Vector3 downwardMovement = Vector3.down * fallSpeed * Time.deltaTime;
            Vector3 totalVelocity = (forwardMovement + downwardMovement) / Time.deltaTime;
            rb.linearVelocity = totalVelocity;
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(destroyDelay);

        // TODO: Реализовать функционал пула для врагов.
        Destroy(gameObject);

        // Проверка, последний ли мёртвый враг на уровне.
        GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>().CheckDeadEnemies();
    }

    /// <summary>
    /// Корутина, изменяющая скорость вращения врага в случайном диапазоне.
    /// </summary>
    private IEnumerator ChangeOrbitSpeedPeriodically()
    {
        float targetOrbitSpeed;

        while (true)
        {
            targetOrbitSpeed = Random.Range(minOrbitSpeed, maxOrbitSpeed);

            float elapsed = 0f;
            float startSpeed = orbitSpeed;

            while (elapsed < transitionDuration)
            {
                orbitSpeed = Mathf.Lerp(startSpeed, targetOrbitSpeed, elapsed / transitionDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            orbitSpeed = targetOrbitSpeed;

            yield return new WaitForSeconds(transitionDuration);
        }
    }

    /// <summary>
    /// Применяет урон к врагу. Если здоровье равно 0 — вызывает смерть.
    /// </summary>
    /// <param name="damage">Количество нанесённого урона.</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0); // Не позволяем здоровью стать отрицательным.

        if (health == 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Логика смерти врага: отключает коллайдер, добавляет эффект огня, запускает падение.
    /// </summary>
    private void Die()
    {
        isDead = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (fireEffectPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            Instantiate(fireEffectPrefab, spawnPosition, Quaternion.identity, transform);
        }

        StopAllCoroutines();

        StartCoroutine(FallToGround());
    }

    /// <summary>
    /// Устанавливает новый радиус орбиты.
    /// </summary>
    /// <param name="radius">Новый радиус.</param>
    public void SetOrbitRadius(float radius)
    {
        orbitRadius = radius;
    }

    /// <summary>
    /// Устанавливает новую высоту полёта.
    /// </summary>
    /// <param name="height">Новая высота.</param>
    public void SetFlightHeight(float height)
    {
        flightHeight = height;
    }

    /// <summary>
    /// Устанавливает новую скорость вращения.
    /// </summary>
    /// <param name="speed">Новая скорость.</param>
    public void OrbitSpeed(float speed)
    {
        orbitSpeed = speed;
    }
}