using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Скорость передвижения
    [Header("Movement Settings")]
    [Tooltip("Базовая скорость передвижения персонажа.")]
    public float moveSpeed = 5f;

    [Tooltip("Множитель скорости при ускорении (Shift).")]
    public float sprintMultiplier = 2f;

    // Параметры прыжка
    [Header("Jump Settings")]
    [Tooltip("Высота прыжка.")]
    public float jumpHeight = 2f;

    // Гравитация (можно настроить в инспекторе или использовать стандартную)
    [Header("Physics Settings")]
    [Tooltip("Гравитационное ускорение. Отрицательное значение.")]
    public float gravity = -9.81f;

    // Компоненты
    private Rigidbody rb;

    // Направление движения
    private Vector3 movementInput;

    // Для проверки, на земле ли персонаж
    [Header("Ground Check Settings")]
    [Tooltip("Точка для проверки на земле.")]
    public Transform groundCheck;

    [Tooltip("Радиус проверки на земле.")]
    public float groundCheckRadius = 0.2f;

    [Tooltip("Слой земли.")]
    public LayerMask groundMask;

    private bool isGrounded;

    // Animator
    private Animator animator;

    // Для управления состоянием прыжка
    private bool isJumping = false;
    private bool isFalling = false;

    // Расстояние для срабатывания Landing анимации
    [Header("Landing Settings")]
    [Tooltip("Расстояние до земли для срабатывания Landing анимации.")]
    public float landingDistance = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Предотвращение вращения

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component не найден!");
        }
    }

    void Update()
    {
        // Получение ввода пользователя
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        movementInput = new Vector3(moveX, 0f, moveZ).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isRunning)
        {
            movementInput *= sprintMultiplier;
        }

        // Обновление Animator параметров
        animator.SetBool("IsRunning", isRunning);
        animator.SetFloat("Vertical", moveZ);
        animator.SetFloat("Horizontal", moveX);

        // Проверка на прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            Jump();
        }

        // Обновление проверки на земле
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // Управление анимациями падения и приземления
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
                // Проверка расстояния до земли
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
    }

    void FixedUpdate()
    {
        // Передвижение персонажа
        Vector3 velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);
        rb.velocity = velocity;

        // Применение гравитации вручную
        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
        else if (rb.velocity.y < 0)
        {
            // Плавное приземление
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
    }

    void Jump()
    {
        // Вычисление необходимой скорости для достижения желаемой высоты прыжка
        float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        Vector3 newVelocity = rb.velocity;
        newVelocity.y = jumpVelocity;
        rb.velocity = newVelocity;

        // Установка флага прыжка и запуск анимации Jump
        isJumping = true;
        animator.SetTrigger("Jump");
    }

    // Визуализация проверки земли в редакторе
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Визуализация расстояния для Landing
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * landingDistance);
    }
}