using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // ������ �� ������
    public float height = 10f;  // ��������� ������ ������
    public float distance = 10f;  // ���������� ������ �� ������ �� ��� Z
    public float scrollSpeed = 2f;  // �������� ��������� ������ ����
    public float minHeight = 5f;  // ����������� ������ ������
    public float maxHeight = 20f;  // ������������ ������ ������

    void LateUpdate()
    {
        // �������� �������� ��������� �������� ����
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // �������� ������ ������ � ����������� �� ���������
        height -= scrollInput * scrollSpeed;

        // ������������ ������ ������ � �������� ������������ � ������������� ��������
        height = Mathf.Clamp(height, minHeight, maxHeight);

        // ��������� ����� ������� ������: ��������� ���������� �� ��� Z � �������� ������ �� ��� Y
        Vector3 newPosition = player.position + new Vector3(0, height, -distance);

        // ��������� ������� ������
        transform.position = newPosition;

        // ������ ������ ������� �� ������
        transform.LookAt(player);
    }
}
