using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // —сылка на игрока
    public float height = 10f;  // Ќачальна€ высота камеры
    public float distance = 10f;  // –ассто€ние камеры от игрока по оси Z
    public float scrollSpeed = 2f;  // —корость прокрутки колеса мыши
    public float minHeight = 5f;  // ћинимальна€ высота камеры
    public float maxHeight = 20f;  // ћаксимальна€ высота камеры

    void LateUpdate()
    {
        // ѕолучаем значение прокрутки колесика мыши
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // »змен€ем высоту камеры в зависимости от прокрутки
        height -= scrollInput * scrollSpeed;

        // ќграничиваем высоту камеры в пределах минимального и максимального значений
        height = Mathf.Clamp(height, minHeight, maxHeight);

        // ¬ычисл€ем новую позицию камеры: сохран€ем рассто€ние по оси Z и измен€ем только по оси Y
        Vector3 newPosition = player.position + new Vector3(0, height, -distance);

        // ќбновл€ем позицию камеры
        transform.position = newPosition;

        //  амера всегда смотрит на игрока
        transform.LookAt(player);
    }
}
