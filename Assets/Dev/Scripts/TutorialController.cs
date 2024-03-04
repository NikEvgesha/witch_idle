using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public List<GameObject> tutorialObjects;
    public int index;
    public GameObject groundStack;

    private void Start()
    {
        if (ES3.KeyExists("tutorial"))
        {
            if ((bool)ES3.Load("tutorial"))
            {
                Destroy(groundStack);
                Destroy(this);
            }
            else
            {
                DOVirtual.DelayedCall(.5f, () =>
                {
                    EventManager.TutorialCameraSet(tutorialObjects[0].transform);
                });
            }
        }
        else
        {
            DOVirtual.DelayedCall(.5f, () =>
            {
                EventManager.TutorialCameraSet(tutorialObjects[0].transform);
            });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MoneyStack>() && index ==0) 
        {
            tutorialObjects[index].SetActive(false);
            index++;
            tutorialObjects[index].SetActive(true);
            EventManager.TutorialCameraSet(tutorialObjects[index].transform);

        }
        if (other.transform.GetComponentInParent<Machine>() && !other.GetComponent<MoneyStack>() && index==3)
        {
            var machine = other.transform.GetComponentInParent<Machine>();
            if (machine.machineState == MachineState.Open)
            {
                if (machine as WorkoutMachine)
                {
                    ES3.Save("tutorial",true);
                    tutorialObjects[index].SetActive(false);
                    this.enabled = false;

                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("MachineBuyArea"))
        {
            if (other.transform.GetComponentInParent<Machine>() as WorkoutMachine)
            {
                var machine = other.transform.GetComponentInParent<Machine>();
            
                if (machine.machineState == MachineState.Open && index==1)
                {
                    tutorialObjects[index].SetActive(false);
                    index++;
                    tutorialObjects[index].SetActive(true);
                    EventManager.TutorialCameraSet(tutorialObjects[index].transform);

                }
            }
            else if(other.transform.GetComponentInParent<Machine>() as WaterMachine)
            {
                var machine = other.transform.GetComponentInParent<WaterMachine>();
            
                if (machine.machineState == MachineState.Open && index==2)
                {
                    tutorialObjects[index].SetActive(false);
                    index++;
                    tutorialObjects[index].SetActive(true);
                    tutorialObjects[index].GetComponentInChildren<TextMeshProUGUI>().text = "Give water to customer";
                    EventManager.TutorialCameraSet(tutorialObjects[index].transform);

                }
            }
           
            
        }
    }
}
