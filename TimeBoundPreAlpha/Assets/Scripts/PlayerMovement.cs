using UnityEngine;
using UnityEngine.SceneManagement; // ���������� ��� ������������ ����, ��������� ��� ������ ������ ���

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{

    // - - - ��������� ��������
    [Header("Movement Settings")]
    [Tooltip("�������� �������� ���������.")]
    public float moveSpeed = 5f;                                       // ����� ���������� � ����� ������� ����� ���������

    [Header("Audio Settings")]
    [Tooltip("���� ����� ���������.")]
    public AudioClip stepSound; // ��������� ��� ����� �����
    private AudioSource audioSource; // ��������� ��� ��������������� �����
    public float stepInterval = 0.55f; // �������� ����� ������� �����
    private float stepTimer; // ������ ��� �������� ���������

    [Tooltip("��������� �������� ����� ����� ��� ����.")]
    public float sprintStepMultiplier = 0.6f; // ��� ������, ��� ������� ��������������� �����

    [Tooltip("���� ��� ��������� �����.")]
    public AudioClip GGhitSound;

    [Tooltip("���� ������ ���������.")]
    public AudioClip deathSound;

    [Tooltip("��������� �������� ��� ������� (Shift).")]
    [SerializeField] private float sprintMultiplier = 2f;

    // ��������� ������
    [Header("Jump Settings")]
    [Tooltip("������ ������.")]
    [SerializeField] private float jumpHeight = 2f;

    // ��������� ������
    [Header("Physics Settings")]
    [Tooltip("���� ����������. ������������� ��������.")]
    [SerializeField] private float gravity = -9.81f;

    // ���� ��������
    [Header("Health Settings")]
    [Tooltip("������������ ���������� ����� ��������.")]
    public int maxHealth = 10;                                             // ����� ���������� � ����� ������� ����� ���������
    private int currentHealth;


    // ��������� ����������
    private Rigidbody rb;
    private Animator animator;

    // ���������� ��������
    private Vector3 movementInput;



    // ��� ��������, ��������� �� �������� �� �����
    [Header("Ground Check Settings")]
    [Tooltip("����� ��� �������� ���������� �� �����.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("������ �������� ���������� �� �����.")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Tooltip("����� ����, ����������� � �����.")]
    [SerializeField] private LayerMask groundMask;

    private bool isGrounded;


    // ��� ��������� �������� ������� � �������
    private bool isJumping = false;
    private bool isFalling = false;

    // ��������� �����������
    [Header("Landing Settings")]
    [Tooltip("��������� ��� ��������� Landing ��������.")]
    [SerializeField] private float landingDistance = 0.5f;






    //������� ������� 

    public int ExpScore = 4;  // ���� ����������� ������� ����� ���������
    public int Money = 0;














    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component �� ������!");
        }

        // ������������� ��������� ��������
        currentHealth = maxHealth;

        // ��������� AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1f; // ���������������� 3D ����
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
        // ��������� ����� �� ������������
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        movementInput = new Vector3(moveX, 0f, moveZ).normalized;
        HandleFootsteps(); // ������������ ���� �����

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isRunning)
        {
            movementInput *= sprintMultiplier;
        }

        // ��������
        animator.SetBool("IsRunning", isRunning);
        animator.SetFloat("Vertical", moveZ);
        animator.SetFloat("Horizontal", moveX);

        // ������
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            Jump();
        }

        // �������� �� �����
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // �������� ������ � �������
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

        // ����� ������ ��� ��������� �������� ������
        SetPlayerRotation();
    }

    void FixedUpdate()
    {
        // ���������� ��������
        Vector3 velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);
        rb.velocity = velocity;

        // ���������� ����������
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

    // ����� ��� ��������� �����
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }

        // ��������������� ����� ���������
        if (GGhitSound != null)
        {
            audioSource.PlayOneShot(GGhitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ������ �����
    private void Die()
    {
        Debug.Log("����� �����!");

        // ��������������� ����� ������
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        // ������������ � ������� ����� �� ����� 2
        SceneManager.LoadScene(2);

        // ����� ����� �������� ������ ����������� ������, ������ ������ ������ � �.�.
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyProjectile"))
        {
            TakeDamage(2); // ������ ������ ������� 2 �����
            Destroy(collision.gameObject); // ���������� ������ ����� ���������
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

    /// ����� ��� ��������� �������� ������ � ����������� �� ������� ������.
    void SetPlayerRotation()
    {
        float desiredAngle = transform.eulerAngles.y; // ������� ���� ��������

        bool isMoving = movementInput.x != 0 || movementInput.z != 0;

        if (!isMoving)
            return; // �� �������, ���� �� ��������

        // ���������� ����������� � ������������� ������ ����
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

        // ������������ ��������
        Quaternion targetRotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
    void HandleFootsteps()
    {
        bool isMoving = movementInput.magnitude > 0; // ���������, �������� �� ��������
        if (isGrounded && isMoving)
        {
            // ���������, ����� �� ��������
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float currentStepInterval = isRunning ? stepInterval * sprintStepMultiplier : stepInterval;

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                stepTimer = currentStepInterval; // ������������� �������� ����
                audioSource.PlayOneShot(stepSound); // ��������������� ����� ����
            }
        }
        else
        {
            stepTimer = 0f; // ����� ������� ��� ���������
        }
    }
}