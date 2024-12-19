using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] _levelPrefabs; // Префабы уровней
    [SerializeField] private GameObject _someOtherObject; // Пример другого объекта
    [SerializeField] private bool _someToggleOption; // Пример галочки
    [SerializeField] private float _someSliderOption; // Пример слайдера
    
    private void Start()
    {
        GenerateRandomLevel();
    }

    private void Update()
    {
        // Здесь можно вызывать другие методы, если необходимо
    }

    private void GenerateRandomLevel()
    {
        if (_levelPrefabs.Length == 0)
        {
            Debug.LogWarning("No level prefabs assigned!");
            return;
        }

        int randomIndex = GetRandomIndex();
        InstantiateLevel(randomIndex);
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, _levelPrefabs.Length);
    }

    private void InstantiateLevel(int index)
    {
        Instantiate(_levelPrefabs[index], Vector3.zero, Quaternion.identity);
    }
}