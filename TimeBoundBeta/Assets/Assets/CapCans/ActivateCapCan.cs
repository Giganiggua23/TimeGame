using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCapCan : MonoBehaviour
{
    [SerializeField] GameObject spikes;


    void OnTriggerEnter()
    {
        spikes.SetActive(true);
    }
}
