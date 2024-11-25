using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // �������� ������������
    [Header("Movement Settings")]
    [Tooltip("������� �������� ������������ ���������.")]
    public float moveSpeed = 5f;

    [Tooltip("��������� �������� ��� ��������� (Shift).")]
    public float sprintMultiplier = 2f;

    // ��������� ������
    [Header("Jump Settings")]
    [Tooltip("������ ������.")]
    public float jumpHeight = 2f;

    // ���������� (����� ��������� � ���������� ��� ������������ �����������)
    [Header("Physics Settings")]
    [Tooltip("�������������� ���������. ������������� ��������.")]
    public float gravity = -9.81f;

    // ����������
    private Rigidbody rb;

    // ����������� ��������
    private Vector3 movementInput;

    // ��� ��������, �� ����� �� ��������
    [Header("Ground Check Settings")]
    [Tooltip("����� ��� �������� �� �����.")]
    public Transform groundCheck;

    [Tooltip("������ �������� �� �����.")]
    public float groundCheckRadius = 0.2f;

    [Tooltip("���� �����.")]
    public LayerMask groundMask;

    private bool isGrounded;

    // Animator
    private Animator animator;

    // ��� ���������� ���������� ������
    private bool isJumping = false;
    private bool isFalling = false;

    // ���������� ��� ������������ Landing ��������
    [Header("Landing Settings")]
    [Tooltip("���������� �� ����� ��� ������������ Landing ��������.")]
    public float landingDistance = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // �������������� ��������

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component �� ������!");
        }
    }

    void Update()
    {
        // ��������� ����� ������������
        float moveX = Input.GetAxisRaw("Horizontal"); // A, D
        float moveZ = Input.GetAxisRaw("Vertical");   // W, S

        movementInput = new Vector3(moveX, 0f, moveZ).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isRunning)
        {
            movementInput *= sprintMultiplier;
        }

        // ���������� Animator ����������
        animator.SetBool("IsRunning", isRunning);
        animator.SetFloat("Vertical", moveZ);
        animator.SetFloat("Horizontal", moveX);

        // �������� �� ������
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            Jump();
        }

        // ���������� �������� �� �����
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // ���������� ���������� ������� � �����������
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
                // �������� ���������� �� �����
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
        // ������������ ���������
        Vector3 velocity = new Vector3(movementInput.x * moveSpeed, rb.velocity.y, movementInput.z * moveSpeed);
        rb.velocity = velocity;

        // ���������� ���������� �������
        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
        else if (rb.velocity.y < 0)
        {
            // ������� �����������
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
    }

    void Jump()
    {
        // ���������� ����������� �������� ��� ���������� �������� ������ ������
        float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        Vector3 newVelocity = rb.velocity;
        newVelocity.y = jumpVelocity;
        rb.velocity = newVelocity;

        // ��������� ����� ������ � ������ �������� Jump
        isJumping = true;
        animator.SetTrigger("Jump");
    }

    // ������������ �������� ����� � ���������
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // ������������ ���������� ��� Landing
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * landingDistance);
    }
}