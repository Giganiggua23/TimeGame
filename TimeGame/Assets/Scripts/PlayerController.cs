using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;
    private Animator animator;

    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float rotationSpeed = 200f;

    private float currentSpeed;

    void Start()
    {
        // Получаем компоненты Rigidbody, Camera и Animator
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        MovePlayer();
        HandleSprint();
        Jump();
        UpdateAnimation();
    }

    private void MovePlayer()
    {
        // Получаем направление движения (WASD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Преобразуем направление в мировые координаты с учетом вращения камеры
        Vector3 direction = mainCamera.transform.forward * vertical + mainCamera.transform.right * horizontal;
        direction.y = 0f; // Оставляем только движение по плоскости XZ

        // Нормализуем направление, чтобы скорость не зависела от угла наклона камеры
        direction.Normalize();

        // Двигаем персонажа
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);

        // Вращаем персонажа в сторону направления движения с меньшей скоростью
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Обновление анимации
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.1f) // Проверка на приземление
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("JumpTrigger"); // Используем Trigger для активации анимации прыжка
        }
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift)) // Ускорение
        {
            currentSpeed = sprintSpeed;
            animator.SetBool("IsRunning", true); // Анимация быстрого бега
        }
        else
        {
            currentSpeed = moveSpeed;
            animator.SetBool("IsRunning", false); // Обычная анимация
        }
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("MovementSpeed", currentSpeed); // Обновление скорости для анимаций
    }
}


