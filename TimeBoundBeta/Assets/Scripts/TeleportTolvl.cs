using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportTolvl : MonoBehaviour
{
    [SerializeField] GameObject TeleportE;

    [SerializeField] int ID = 0;


    void Start()
    {
        TeleportE.SetActive(false);
    }
    
    void OnTriggerStay(Collider other)
    {
        TeleportE.SetActive(true);

        if (other.tag == "Player" && Input.GetKey(KeyCode.E))
        {

            switch (ID)
            {
                case 1:

                    SceneManager.LoadSceneAsync(1);
                    break;
                case 2:
                    SceneManager.LoadSceneAsync(4);
                    break;

            }

                    

        }
    }
    void OnTriggerExit()
    {
        TeleportE.SetActive(false);
    }
}
