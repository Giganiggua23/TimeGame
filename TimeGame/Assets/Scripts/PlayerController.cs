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
        // �������� ���������� Rigidbody, Camera � Animator
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
        // �������� ����������� �������� (WASD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // ����������� ����������� � ������� ���������� � ������ �������� ������
        Vector3 direction = mainCamera.transform.forward * vertical + mainCamera.transform.right * horizontal;
        direction.y = 0f; // ��������� ������ �������� �� ��������� XZ

        // ����������� �����������, ����� �������� �� �������� �� ���� ������� ������
        direction.Normalize();

        // ������� ���������
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);

        // ������� ��������� � ������� ����������� �������� � ������� ���������
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // ���������� ��������
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.1f) // �������� �� �����������
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("JumpTrigger"); // ���������� Trigger ��� ��������� �������� ������
        }
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift)) // ���������
        {
            currentSpeed = sprintSpeed;
            animator.SetBool("IsRunning", true); // �������� �������� ����
        }
        else
        {
            currentSpeed = moveSpeed;
            animator.SetBool("IsRunning", false); // ������� ��������
        }
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("MovementSpeed", currentSpeed); // ���������� �������� ��� ��������
    }
}


