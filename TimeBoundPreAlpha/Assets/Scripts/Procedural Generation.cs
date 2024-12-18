using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [Range(5, 100)]
    public int roomCount = 10; // ���������� ������
    [Range(3f, 20f)]
    public float roomMinSize = 5f; // ���. ������ �������
    [Range(5f, 30f)]
    public float roomMaxSize = 10f; // ����. ������ �������

    [Header("Wall Settings")]
    public GameObject wallPrefab; // ������ �����
    [Range(2f, 10f)]
    public float wallHeight = 3f; // ������ �����
    [Range(0.1f, 2f)]
    public float wallThickness = 0.5f; // ������� �����

    [Header("Floor Settings")]
    public GameObject floorPrefab; // ������ ����
    [Range(0.1f, 1f)]
    public float floorHeight = 0.1f; // ������ ����

    [Header("Corridor Settings")]
    public GameObject corridorPrefab; // ������ ��������
    [Range(2f, 10f)]
    public float corridorWidth = 3f; // ������ ��������
    [Range(1f, 100f)]
    public float corridorHeight = 3f; // ������ ��������

    [Header("Room Objects")]
    public List<GameObject> roomObjects; // ���. ������� � ��������
    [Range(0f, 10f)]
    public float objectSpawnChance = 0.7f; // ���� ������ �������

    [Header("Enemies")]
    public List<EnemyType> enemyTypes; // ���� ������
    [Range(0f, 1f)]
    public float enemySpawnRate = 0.5f; // ��������� ������ ������

    [Header("Chests")]
    public GameObject chestPrefab; // ������ �������
    [Range(0f, 1f)]
    public float chestSpawnChance = 0.3f; // ���� ������ �������

    private List<Room> rooms = new List<Room>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // ��������� ������
        for (int i = 0; i < roomCount; i++)
        {
            bool newRoomValid = false;
            Room newRoom = null;
            int maxTries = 100;

            while (!newRoomValid && maxTries > 0)
            {
                newRoom = new Room(
                    Random.Range(roomMinSize, roomMaxSize),
                    Random.Range(roomMinSize, roomMaxSize),
                    new Vector2(Random.Range(-50, 50), Random.Range(-50, 50))
                );

                newRoomValid = true;
                foreach (Room other in rooms)
                {
                    if (newRoom.Intersects(other))
                    {
                        newRoomValid = false;
                        break;
                    }
                }
                maxTries--;
            }

            if (newRoomValid && newRoom != null)
            {
                rooms.Add(newRoom);
                // �������� ���� �������
                CreateRoomFloor(newRoom);
                // �������� ���� �������
                CreateRoomWalls(newRoom);
                // ���������� �������� � �������
                AddObjectsToRoom(newRoom);
                // ���������� ��������
                if (Random.value <= chestSpawnChance)
                {
                    AddChestToRoom(newRoom);
                }
                // ����� ������
                SpawnEnemiesInRoom(newRoom);
            }
        }

        // �������� ��������� ����� ���������
        CreateCorridors();
    }

    void CreateRoomFloor(Room room)
    {
        if (floorPrefab == null) return;

        Vector3 floorPosition = new Vector3(room.position.x, -floorHeight, room.position.y);
        Vector3 floorScale = new Vector3(room.width, floorHeight, room.depth);

        GameObject floor = Instantiate(floorPrefab, floorPosition, Quaternion.identity, this.transform);
        floor.transform.localScale = floorScale;
    }

    void CreateRoomWalls(Room room)
    {
        // ������ ������� ���� �������
        Vector3 roomPosition = new Vector3(room.position.x, 0, room.position.y);
        Vector3 roomScale = new Vector3(room.width, wallHeight, room.depth);

        GameObject roomObject = new GameObject("Room_" + rooms.Count);
        roomObject.transform.position = roomPosition;
        roomObject.transform.parent = this.transform;

        // �������� ������� ����
        // �������� �����
        CreateWall(new Vector3(0, wallHeight / 2, room.depth / 2), new Vector3(room.width, wallHeight, wallThickness), roomObject.transform);
        // ����� �����
        CreateWall(new Vector3(0, wallHeight / 2, -room.depth / 2), new Vector3(room.width, wallHeight, wallThickness), roomObject.transform);
        // ��������� �����
        CreateWall(new Vector3(room.width / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, room.depth), roomObject.transform);
        // �������� �����
        CreateWall(new Vector3(-room.width / 2, wallHeight / 2, 0), new Vector3(wallThickness, wallHeight, room.depth), roomObject.transform);
    }

    void CreateWall(Vector3 localPosition, Vector3 localScale, Transform parent)
    {
        if (wallPrefab == null) return;

        GameObject wall = Instantiate(wallPrefab, parent);
        wall.transform.localPosition = localPosition;
        wall.transform.localScale = localScale;
    }

    void AddRoomObjects(Room room)
    {
        if (roomObjects == null || roomObjects.Count == 0) return;

        // ������, ��������� ������ ��� ��� �� ������ ����a
        if (Random.value > objectSpawnChance) return;

        // ���������� �������� ����� ������� ���������������� ������� �������
        int objectCount = Random.Range(1, Mathf.Max(1, Mathf.RoundToInt(room.width * room.depth / 20)));
        for (int i = 0; i < objectCount; i++)
        {
            GameObject objPrefab = roomObjects[Random.Range(0, roomObjects.Count)];
            Vector3 objPosition = new Vector3(
                room.position.x + Random.Range(-room.width / 2 + 1, room.width / 2 - 1),
                0,
                room.position.y + Random.Range(-room.depth / 2 + 1, room.depth / 2 - 1)
            );

            Instantiate(objPrefab, objPosition, Quaternion.identity, this.transform);
        }
    }

    void AddObjectsToRoom(Room room)
    {
        AddRoomObjects(room);
    }

    void AddChestToRoom(Room room)
    {
        if (chestPrefab == null) return;

        Vector3 chestPosition = new Vector3(
            room.position.x + Random.Range(-room.width / 2 + 1, room.width / 2 - 1),
            0,
            room.position.y + Random.Range(-room.depth / 2 + 1, room.depth / 2 - 1)
        );

        Instantiate(chestPrefab, chestPosition, Quaternion.identity, this.transform);
    }

    void SpawnEnemiesInRoom(Room room)
    {
        if (enemyTypes == null || enemyTypes.Count == 0) return;

        int enemyCount = Mathf.RoundToInt(enemySpawnRate * Random.Range(1, Mathf.Max(2, room.width / 5)));
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyPrefab = GetRandomEnemy();
            if (enemyPrefab != null)
            {
                Vector3 enemyPosition = new Vector3(
                    room.position.x + Random.Range(-room.width / 2 + 1, room.width / 2 - 1),
                    0,
                    room.position.y + Random.Range(-room.depth / 2 + 1, room.depth / 2 - 1)
                );

                Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, this.transform);
            }
        }
    }

    GameObject GetRandomEnemy()
    {
        float totalRarity = 0f;
        foreach (var enemy in enemyTypes)
        {
            totalRarity += enemy.rarity;
        }

        float rand = Random.Range(0, totalRarity);
        float cumulative = 0f;
        foreach (var enemy in enemyTypes)
        {
            cumulative += enemy.rarity;
            if (rand <= cumulative)
            {
                return enemy.prefab;
            }
        }

        return null;
    }

    void CreateCorridors()
    {
        if (rooms.Count < 2) return;

        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Room currentRoom = rooms[i];
            Room nextRoom = rooms[i + 1];

            Vector3 start = new Vector3(currentRoom.position.x, 0, currentRoom.position.y);
            Vector3 end = new Vector3(nextRoom.position.x, 0, nextRoom.position.y);

            Vector3 direction = end - start;
            float distance = direction.magnitude;
            Vector3 corridorScale = new Vector3(corridorWidth, corridorHeight, distance);

            GameObject corridor = Instantiate(corridorPrefab, (start + end) / 2, Quaternion.identity, this.transform);
            corridor.transform.localScale = corridorScale;

            // ������� �������� � ����������� ��������� �������
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            corridor.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    // ��������������� ����� ��� ������������� �������
    class Room
    {
        public float width;
        public float depth;
        public Vector2 position;

        public Room(float width, float depth, Vector2 position)
        {
            this.width = width;
            this.depth = depth;
            this.position = position;
        }

        // �������� ����������� � ������ ��������
        public bool Intersects(Room other)
        {
            return (Mathf.Abs(this.position.x - other.position.x) * 2 < (this.width + other.width)) &&
                   (Mathf.Abs(this.position.y - other.position.y) * 2 < (this.depth + other.depth));
        }
    }
}

[System.Serializable]
public class EnemyType
{
    public string name;
    public GameObject prefab;
    [Range(0.1f, 10f)]
    public float rarity = 1f; // ���� �������� - ���� ����������� ���������
}
