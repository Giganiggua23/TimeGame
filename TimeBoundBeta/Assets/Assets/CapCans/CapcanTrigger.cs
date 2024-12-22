using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapcanTrigger : MonoBehaviour
{
    public PlayerMovement PM;

   
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            PM.currentHealth -= 30;
    }
}
