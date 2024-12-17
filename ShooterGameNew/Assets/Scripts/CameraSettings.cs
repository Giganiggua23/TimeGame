using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;  // —сылка на игрока
    float _height = 27f;  // Ќачальна€ высота камеры
    float _distance = 11f;  // –ассто€ние камеры от игрока по оси Z
    float _scrollSpeed = 8f;  // —корость прокрутки колеса мыши
    float _minHeight = 15f;  // ћин. высота камеры
    float _maxHeight = 30f;  // ћакс. высота камеры

    void LateUpdate()
    {
        // ѕолучаем значение прокрутки колесика мыши
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // »змен€ем высоту камеры в зависимости от прокрутки
        _height -= scrollInput * _scrollSpeed;

        // ќграничиваем высоту камеры в пределах минимального и максимального значений
        _height = Mathf.Clamp(_height, _minHeight, _maxHeight);

        // ¬ычисл€ем новую позицию камеры: сохран€ем рассто€ние по оси Z и измен€ем только по оси Y
        Vector3 newPosition = player.position + new Vector3(0, _height, -_distance);

        // ќбновл€ем позицию камеры
        transform.position = newPosition;

        //  амера всегда смотрит на игрока
        transform.LookAt(player);
    }
}
