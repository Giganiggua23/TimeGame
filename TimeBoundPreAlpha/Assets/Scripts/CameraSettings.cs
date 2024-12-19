using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;  // ������ �� ������
    float _height = 27f;  // ��������� ������ ������
    float _distance = 11f;  // ���������� ������ �� ������ �� ��� Z
    float _scrollSpeed = 8f;  // �������� ��������� ������ ����
    float _minHeight = 15f;  // ���. ������ ������
    float _maxHeight = 30f;  // ����. ������ ������

    void LateUpdate()
    {
        // �������� �������� ��������� �������� ����
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // �������� ������ ������ � ����������� �� ���������
        _height -= scrollInput * _scrollSpeed;

        // ������������ ������ ������ � �������� ������������ � ������������� ��������
        _height = Mathf.Clamp(_height, _minHeight, _maxHeight);

        // ��������� ����� ������� ������: ��������� ���������� �� ��� Z � �������� ������ �� ��� Y
        Vector3 newPosition = player.position + new Vector3(0, _height, -_distance);

        // ��������� ������� ������
        transform.position = newPosition;

        // ������ ������ ������� �� ������
        transform.LookAt(player);
    }
}
