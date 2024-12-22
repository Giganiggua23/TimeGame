using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutor : MonoBehaviour
{
    [SerializeField] GameObject tutorialCanvas;
    int stepTutor;


    [SerializeField] GameObject tutorialWASD;
    [SerializeField] GameObject tutorialShift;
    [SerializeField] GameObject tutorialAttack;
    [SerializeField] GameObject tutorialNextLvl;


    bool W, A, S, D;



    void Start()
    {
        tutorialWASD.SetActive(false);
        tutorialShift.SetActive(false);
        tutorialAttack.SetActive(false);
        tutorialNextLvl.SetActive(false);

        stepTutor = 1;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (stepTutor)
        {
            case 1:
                tutorialWASD.SetActive(true);
                WASD();
                break;
                case 2:
                tutorialWASD.SetActive(false);
                tutorialShift.SetActive(true);
                Shift();
                break;
                case 3:
                tutorialShift.SetActive(false);
                tutorialAttack.SetActive(true);
                Attack();
                break;
                case 4:
                tutorialAttack.SetActive(false);
                tutorialNextLvl.SetActive(true);

                break;
        }
    }

    public void ENDtutor()
    {
        Destroy(tutorialCanvas);
    }

    void WASD()
    {
        if (Input.GetKey(KeyCode.W))
            W = true;
        if (Input.GetKey(KeyCode.A))
            A = true;
        if (Input.GetKey(KeyCode.S))
            S = true;
        if (Input.GetKey(KeyCode.D))
            D = true;
        
        if(W == true && A == true && S == true && D == true)
        {
            stepTutor ++;
        }
    }

    void Shift()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            stepTutor ++;
    }

    void Attack()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            stepTutor++;
    }
}
