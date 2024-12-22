using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpgrade : MonoBehaviour
{
    [SerializeField] private GameObject _canvasTrigger;
    [SerializeField] private GameObject infostat;

    [Tooltip("ID")]
    [SerializeField] private int ID = 0;                                // 1 - Speed   2 - Health  3 - GunDamage  4 -  Gun Reload

    public PlayerMovement PM;


    float keyHoldTime = 0f;

    public UIstats stats;

    int ExScore;
    void Start()
    {
        _canvasTrigger.SetActive(false);
        infostat.SetActive(false);
    }


    void FixedUpdate()
    {
        ExScore = PM.ExpScore;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {


            if (!_canvasTrigger.activeSelf)
            {
                _canvasTrigger.SetActive(true);

            }
            if (Input.GetKey(KeyCode.E) && !infostat.activeSelf)
            {
                infostat.SetActive(true);
            }

            if (ExScore >= 1 && Input.GetKey(KeyCode.E))
            {
                Debug.Log("Клавиша нажата");// DELETE - need window

                keyHoldTime += Time.deltaTime;

                if (keyHoldTime >= 1.3f)
                {
                    switch (ID)
                    {
                        case 1:
                            if (stats.Speed != 4)
                            {
                                PM.moveSpeed += 0.5f;
                                stats.Speed++;
                                PM.ExpScore -= 1;
                                Debug.Log(stats.Speed);
                            }
                            else
                            {

                            }

                            break;
                        case 2:
                            if (stats.Health != 4)
                            {
                                PM.maxHealth += 5;
                                stats.Health += 1;
                                PM.ExpScore -= 1;
                                Debug.Log(stats.Health);
                            }
                            break;
                        case 3:
                            /*  if (stats.GunDamage != 4)                 // Урон оружия
                               {
                                   PM. += 5;
                                   stats.GunDamage += 1;
                                   PM.ExpScore -= 1;
                               }*/
                            break;
                        case 4:
                            /*  if (stats.GunReload != 4)                 // Перезарядка 
                              {
                                  PM. += 5;
                                  stats.GunReload += 1;
                                  PM.ExpScore -= 1;
                              }*/
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
        infostat.SetActive(false);

    }
}
