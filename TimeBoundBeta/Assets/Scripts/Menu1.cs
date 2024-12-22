using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu1 : MonoBehaviour
{
    // Menu - 0   Lobby - 3
    // LVL 1 - 1  LVL 2 - 4
    // Death - 2 

    // SceneManager.LoadSceneAsync(0);

    [SerializeField] GameObject Qmenu;
    


    void Start()
    {
        Qmenu.SetActive(false);
        
    }

    public void QuickMenu()
    {
        if (Qmenu.activeSelf)
        {
            Qmenu.SetActive(false);
        }
        else
        {
            Qmenu.SetActive(true);
        }
    }

    

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    
}
