using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{


    int level;

    [SerializeField] GameObject room1;
    [SerializeField] GameObject room2;
    [SerializeField] GameObject room3;
    [SerializeField] GameObject room4;
    [SerializeField] GameObject room5;
    [SerializeField] GameObject room6;

    void Start()
    {
        level = UnityEngine.Random.Range(1, 6);
        switch (level)
        {
            case 1: 
                room1.SetActive(true);
                break;
            case 2:
                room2.SetActive(true);
                break;
            case 3:
                room3.SetActive(true);
                break;
            case 4:
                room4.SetActive(true);
                break;
            case 5:
                room5.SetActive(true);
                break;
            case 6:
                room6.SetActive(true);
                break;


        }
    }
}

