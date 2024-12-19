using UnityEngine;
using System.Collections.Generic;


public class RoomGenerator : MonoBehaviour
{
    [Header("Настройки генерации")]
    public GameObject startRoomPrefab; // Префаб стартовой комнаты
    public GameObject endRoomPrefab;   // Префаб последней комнаты
    public GameObject[] roomPrefabs;   // Префабы остальных комнат
    public GameObject corridorPrefab;  // Префаб коридора
    public int numberOfRooms = 10;     // Количество генерируемых комнат
    public float roomDistance = 10f;   // Расстояние между комнатами
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // Занятые позиции

    private void Start()
    {
        GenerateRooms();
    }

    private void GenerateRooms()
    {
        if (roomPrefabs.Length == 0 || corridorPrefab == null) return;

        Vector3 currentPosition = transform.position;

        if (startRoomPrefab != null)
        {
            InstantiateObject(startRoomPrefab, currentPosition, true);
            occupiedPositions.Add(currentPosition);
        }

        for (int i = 1; i < numberOfRooms - 1; i++)
        {
            GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
            Room currentRoom = roomPrefab.GetComponent<Room>();

            Vector3 nextPosition = Vector3.zero;
            bool positionFound = false;
            int chosenExitIndex = -1;

            // Перемешиваем индексы выходов
            List<int> exitIndices = new List<int> { 0, 1, 2, 3 };
            ShuffleArray(exitIndices);

            foreach (int exitIndex in exitIndices)
            {
                if (currentRoom.exits[exitIndex])
                {
                    Vector3 direction = GetDirectionByIndex(exitIndex);
                    nextPosition = currentPosition + direction * roomDistance;
                    if (!occupiedPositions.Contains(nextPosition))
                    {
                        positionFound = true;
                        chosenExitIndex = exitIndex;
                        break;
                    }
                }
            }

            if (!positionFound)
            {
                Debug.LogError("Не удалось найти подходящую позицию для комнаты.");
                return;
            }

            // Проверяем, что следующая комната имеет вход, соответствующий направлению
            GameObject nextRoomPrefab = null;
            foreach (var prefab in roomPrefabs)
            {
                Room nextRoom = prefab.GetComponent<Room>();
                int oppositeIndex = (chosenExitIndex + 2) % 4; // Находим противоположный индекс
                if (nextRoom.exits[oppositeIndex])
                {
                    nextRoomPrefab = prefab;
                    break;
                }
            }

            if (nextRoomPrefab == null)
            {
                Debug.LogError("Не удалось найти подходящую комнату с входом.");
                return;
            }

            Vector3 corridorPosition = (currentPosition + nextPosition) / 2;
            InstantiateObject(corridorPrefab, corridorPosition, false);

            InstantiateObject(nextRoomPrefab, nextPosition, true);
            occupiedPositions.Add(nextPosition);

            currentPosition = nextPosition;
        }

        if (endRoomPrefab != null)
        {
            Vector3 finalPosition = GetRandomNextPosition(currentPosition);
            Vector3 corridorPosition = (currentPosition + finalPosition) / 2;
            InstantiateObject(corridorPrefab, corridorPosition, false);

            InstantiateObject(endRoomPrefab, finalPosition, true);
            occupiedPositions.Add(finalPosition);
        }
    }

    private void InstantiateObject(GameObject prefab, Vector3 position, bool isRoom)
    {
        Quaternion rotation = isRoom ? Quaternion.Euler(90, 0, 0) : Quaternion.identity;
        GameObject obj = Instantiate(prefab, position, rotation);
        obj.transform.parent = transform;
    }

    private Vector3 GetRandomNextPosition(Vector3 currentPosition)
    {
        Vector3[] directions =
        {
        Vector3.forward * roomDistance,
        Vector3.right * roomDistance,
        Vector3.back * roomDistance,
        Vector3.left * roomDistance
    };

        ShuffleArray(directions);

        foreach (Vector3 dir in directions)
        {
            Vector3 newPosition = currentPosition + dir;
            if (!occupiedPositions.Contains(newPosition))
            {
                return newPosition;
            }
        }

        return currentPosition;
    }

    private void ShuffleArray(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void ShuffleArray(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector3 temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    private Vector3 GetDirectionByIndex(int index)
    {
        switch (index)
        {
            case 0: return Vector3.forward;
            case 1: return Vector3.right;
            case 2: return Vector3.back;
            case 3: return Vector3.left;
            default: return Vector3.zero;
        }
    }
}