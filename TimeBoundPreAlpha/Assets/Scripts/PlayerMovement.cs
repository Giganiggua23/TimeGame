using UnityEngine;
using UnityEngine.SceneManagement; // библиотека для переключения сцен, конкретно для смерти игрока тут

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    // - - - Настройки движения
    [Header("Movement Settings")]
    [Tooltip("Скорость движения персонажа.")]
    public float moveSpeed = 5f;                                       // потом переделать в метод который будет добавлять

    [Header("Audio Settings")]
    [Tooltip("Звук шагов персонажа.")]
    public AudioClip stepSound; // Аудиоклип для звука шагов
    private AudioSource audioSource; // Компонент для воспроизведения звука
    public float stepInterval = 0.55f; // Интервал между звуками шагов
    private float stepTimer; // Таймер для контроля интервала

    [Tooltip("Множитель скорости звука шагов при беге.")]
    public float sprintStepMultiplier = 0.6f; // Чем меньше, тем быстрее воспроизведение звука

    [Tooltip("Звук при получении урона.")]
    public AudioClip GGhitSound;

    [Tooltip("Звук смерти персонажа.")]
    public AudioClip deathSound;

    [Tooltip("Множитель скорости для спринта (Shift).")]
    [SerializeField] private float sprintMultiplier = 2f;

    // Настройки прыжка
    [Header("Jump Settings")]
    [Tooltip("Высота прыжка.")]
    [SerializeField] private float jumpHeight = 2f;

    // Настройки физики
    [Header("Physics Settings")]
    [Tooltip("Сила гравитации. Отрицательное значение.")]
    [SerializeField] private float gravity = -9.81f;

    // Очки здоровья
    [Header("Health Settings")]
    [Tooltip("Максимальное количество очков здоровья.")]
    public int maxHealth = 10;                                             // потом переделать в метод который будет добавлять
    private int currentHealth;


    // Приватные компоненты
    private Rigidbody rb;
    private Animator animator;

    // Переменные движения
    private Vector3 movementInput;



    // Для проверки, находится ли персонаж на земле
    [Header("Ground Check Settings")]
    [Tooltip("Точка для проверки нахождения на земле.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("Радиус проверки нахождения на земле.")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Tooltip("Маска слоёв, относящихся к земле.")]
    [SerializeField] private LayerMask groundMask;

    private bool isGrounded;


    // Для обработки анимаций прыжков и падений
    private bool isJumping = false;
    private bool isFalling = false;

    // Настройки приземления
    [Header("Landing Settings")]
    [Tooltip("Дистанция для активации Landing анимации.")]
    [SerializeField] private float landingDistance = 0.5f;






    //Игровые цености 

    public int ExpScore = 4;  // Очки экспирианса которые можно потратить
    public int Money = 0;














    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component не найден!");
        }

        // Устанавливаем начальное здоровье
        currentHealth = maxHealth;

        // Настройка AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f; // Пространственный 3D звук
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 10f;
        audioSource.loop = false;
        if (stepSound != null)
        {
            audioSource.clip = stepSound;
        }
    }

    void Update()
    {
        // Получение ввода от пользователя
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        movementInput = new Vector3(moveX, 0f, moveZ).normalized;
        HandleFootsteps(); // Обрабатывает звук шагов

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isRunning)
        {
            movementInput *= sprintMultiplier;
        }

        // Анимации
        animator.SetBool("IsRunning", isRunning);
        animator.SetFloat("Vertical", moveZ);
        animator.SetFloat("Horizontal", moveX);

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            Jump();
        }

        // Проверка на землю
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // Анимации прыжка и падения
        if (!isGrounded)
        {
            if (rb.velocity.y < 0 && !isFalling)
            {
                isFalling = true;
                animator.SetTrigger("FallingIdle");
            }
        }
        else
        {
            if (isFalling)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, landingDistance + groundCheckRadius, groundMask))
                {
                    animator.SetTrigger("Landing");
                    isFalling = false;
                }
            }

            if (isJumping)
            {
                isJumping = false;
            }
        }

        // Вызов метода для установки поворота игрока
        SetPlayerRotation();
    }

    void FixedUpdate()
    {
        // Обновление скорости
        Vector3 velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);
        rb.velocity = velocity;

        // Применение гравитации
        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
        else if (rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
    }

    void Jump()
    {
        float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        Vector3 newVelocity = rb.velocity;
        newVelocity.y = jumpVelocity;
        rb.velocity = newVelocity;

        isJumping = true;
        animator.SetTrigger("Jump");
    }

    // Метод для получения урона
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }

        // Воспроизведение звука попадания
        if (GGhitSound != null)
        {
            audioSource.PlayOneShot(GGhitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Смерть героя
    private void Die()
    {
        Debug.Log("Герой погиб!");

        // Воспроизведение звука смерти
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        // Переключение с текущей сцены на сцену 2
        SceneManager.LoadScene(2);

        // Здесь можно добавить логику перезапуска уровня, показа экрана смерти и т.д.
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyProjectile"))
        {
            TakeDamage(2); // Каждый снаряд наносит 2 урона
            Destroy(collision.gameObject); // Уничтожаем снаряд после попадания
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * landingDistance);
    }

    /// Метод для установки поворота игрока в зависимости от нажатых клавиш.
    void SetPlayerRotation()
    {
        float desiredAngle = transform.eulerAngles.y; // Текущий угол поворота

        bool isMoving = movementInput.x != 0 || movementInput.z != 0;

        if (!isMoving)
            return; // Не вращаем, если не движется

        // Определяем направление и устанавливаем нужный угол
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D))
            {
                desiredAngle = 45f;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                desiredAngle = -45f;
            }
            else
            {
                desiredAngle = 0f;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.D))
            {
                desiredAngle = 135f;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                desiredAngle = -135f;
            }
            else
            {
                desiredAngle = 180f;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            desiredAngle = 90f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            desiredAngle = -90f;
        }

        // Встановлення поворота
        Quaternion targetRotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
    void HandleFootsteps()
    {
        bool isMoving = movementInput.magnitude > 0; // Проверяем, движется ли персонаж
        if (isGrounded && isMoving)
        {
            // Проверяем, бежит ли персонаж
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float currentStepInterval = isRunning ? stepInterval * sprintStepMultiplier : stepInterval;

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                stepTimer = currentStepInterval; // Устанавливаем интервал шага
                audioSource.PlayOneShot(stepSound); // Воспроизведение звука шага
            }
        }
        else
        {
            stepTimer = 0f; // Сброс таймера при остановке
        }
    }
}