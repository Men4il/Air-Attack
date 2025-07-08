Проект представляет собой игру, где игрок управляет турелью, которая стреляет по летающим врагам. В игре реализована система уровней, пули, пул объектов, UI-индикатор здоровья, а также система управления жизненным циклом объектов.

---

## **Основные скрипты и их логика**

### 1. **Player** (Представляет игрока)
**Логика:**
  - Хранит здоровье (`currentHealth`, `maxHealth`).
  - Реагирует на урон методом `TakeDamage()`.
  - Испускает событие `OnPlayerDeath`, если здоровье равно нулю.
  - Подписывается на события из других систем через UnityEvents.

### 2. **TurretAndCameraController** (Контроллер турели и камеры)
**Логика:**
  - Обрабатывает поворот турели с помощью InputSystem.
  - Ограничивает вертикальный угол поворота пушки.
  - Управляет стрельбой с заданным интервалом.
  - Вызывает метод `BulletSpawner.GetPooledBullet()` для создания пули.

### 3. **BulletSpawner** (Singleton-класс, предоставляющий пулю из пула)
**Логика:**
  - Делегирует вызов `GetPooledObject()` классу `BulletPool`.

### 4. **BulletPool** (Управление пулями через пул объектов)
**Логика:**
  - Создаёт начальные пули при старте.
  - Возвращает пулю из пула или создаёт новую, если все активны.
  - Метод `ReturnToPool()` возвращяет пулю обратно после столкновения.

### 5. **Bullet** (Сама пуля — физический объект)
**Логика:**
  - Направляется с помощью `ResetPositionAndDirection()`.
  - Активна в течение `lifeTime`, после чего возвращается в пул.
  - При столкновении с врагом наносит урон через `EnemyLogic.TakeDamage()`.

### 6. **EnemyLogic** (Логика поведения врага)
**Логика:**
  - Вращается вокруг игрока по орбите.
  - Поворачивается в направлении движения.
  - Атакует игрока с интервалом.
  - При получении урона вызывает `Die()`, который запускает корутину падения и эффекты.

### 7. **EnemySpawner** (Спавнит врагов вокруг игрока)
**Логика:**
  - Спавнит случайных врагов с разными параметрами (радиус, высота).
  - Отслеживает количество спавненных и убитых врагов.
  - Вызывает победный экран через `GameManager`, когда все враги мертвы.

### 8. **GameManager** (Управляет игровым процессом)
**Логика:**
  - Отслеживает уровень, количество убитых врагов.
  - Отображает экран победы/поражения.
  - Сбрасывает уровень при необходимости.
  - Связывает Player, EnemySpawner и UI.

### 9. **HealthBar** (Отображает здоровье игрока в виде полоски)
**Логика:**
  - Подписывается на событие изменения здоровья игрока.
  - Обновляет значение `Slider.value` на основе `player.GetHealthPercentage()`.

---

### Использованные ассеты:
**1. [Handpainted Grass & Ground Textures | 2D Nature | Unity Asset Store](https://assetstore.unity.com/packages/2d/textures-materials/nature/handpainted-grass-ground-textures-187634)**

**2. [Skybox Series Free | 2D Sky | Unity Asset Store](https://assetstore.unity.com/packages/2d/textures-materials/sky/skybox-series-free-103633)**

**3. [VFX URP - Fire Package | Fire & Explosions | Unity Asset Store](https://assetstore.unity.com/packages/vfx/particles/fire-explosions/vfx-urp-fire-package-305098)**

**4. [Sci-fi Turrets (cannon) | Unity Asset Store](https://assetstore.unity.com/packages/3d/props/weapons/sci-fi-turrets-cannon-69615)**

**5. [Low Poly Military Vehicles Package | Unity Asset Store](https://assetstore.unity.com/packages/3d/vehicles/low-poly-military-vehicles-package-276939)**
