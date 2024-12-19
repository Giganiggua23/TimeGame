using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] _levelPrefabs; // ������� �������
    [SerializeField] private GameObject _someOtherObject; // ������ ������� �������
    [SerializeField] private bool _someToggleOption; // ������ �������
    [SerializeField] private float _someSliderOption; // ������ ��������
    
    private void Start()
    {
        GenerateRandomLevel();
    }

    private void Update()
    {
        // ����� ����� �������� ������ ������, ���� ����������
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