using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpgrade : MonoBehaviour
{

    [SerializeField] private GameObject _canvasTrigger;
    [SerializeField] private int ID = 0;                                // 1 - Speed   2 - Health  3 - GunDamage  4 -  Gun Reload

    public PlayerMovement PM;


    float keyHoldTime = 0f;

    int Speed, Health, GunDamage, GunReload;   // Ограничения

    void Start()
    {
          _canvasTrigger.SetActive(false);
    }

    
    void Update()
    {
            
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            
            
            if (!_canvasTrigger.activeSelf)  
            {
                _canvasTrigger.SetActive(true);
            }
                
            if (PM.ExpScore >= 1 && Input.GetKey(KeyCode.E))
            {
                Debug.Log("Клавиша нажата");// DELETE - need window

                keyHoldTime += Time.deltaTime;

                if (keyHoldTime >= 1.3f)
                {
                    switch (ID)
                    {
                        case 1:
                            if (Speed != 4)
                            {
                                PM.moveSpeed += 0.5f;
                                Speed += 1;
                                PM.ExpScore -= 1;
                            }
                            
                            break;
                        case 2:
                            if (Health != 4)
                            {
                                PM.maxHealth += 5;
                                Health += 1;
                                PM.ExpScore -= 1;
                            }
                            break;
                        case 3:
                            
                            break;
                        case 4:

                            break;

                    }
                    Debug.Log("Клавиша Зажата");  // DELETE  - check
                    keyHoldTime = 0;
                }
            }
            else
            {
                keyHoldTime = 0;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        
          _canvasTrigger.SetActive(false);
        
    }
}
