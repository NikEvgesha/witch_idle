using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Customer : MonoBehaviour
{
    public NavMeshAgent meshAgent;
    public Machine customerMachine;
    public CustomerStates state;
    public Animator animator;
    public GameObject moneyPrefab;
    public List<GameObject> dumbells;
    public List<GameObject> inclineDumbells;

    public GameObject benchBar;
    public CustomerWaterTowelUI waterTowelUI;
    public CustomerNeed customerNeed;
    [HideInInspector]public Vector3 workoutOverPosition;
    public Renderer renderer;
    public GameObject waterAnimation;
    public GameObject towelAnimation;
    public GameObject deadLiftBar;

    private Transform customerLookAt;

    private void Start()
    {
        customerLookAt = FindObjectOfType<CameraManager>().gameCamera.transform;
    }

    public void SetCustomer(Material mat)
    {

        renderer.material = mat;
    }

    public void MoveToMachine(Transform target,Machine machine)
    {
        waterTowelUI.canvas.SetActive(false);
        waterTowelUI.machineImage.gameObject.SetActive(false);

        animator.SetBool("Idle",false);
        meshAgent.enabled = true;
        meshAgent.destination = target.position;
        customerMachine = machine;
        state = CustomerStates.WalkingToMachine;
    }

    public void ActivateDumbells(bool isActive)
    {
        foreach (var dumbell in dumbells)
        {
            dumbell.SetActive(isActive);
        }
    }
    public void ActivateInclineDumbells(bool isActive)
    {
        foreach (var dumbell in inclineDumbells)
        {
            dumbell.SetActive(isActive);
        }
    }
    public void ActivateBenchVar(bool isActive)
    {
            benchBar.SetActive(isActive);
    }
    public void ActivateDeadLiftBar(bool isActive)
    {
        deadLiftBar.SetActive(isActive);
    }

    public void GoToLine(Transform lineTransform,Machine machine)
    {
        meshAgent.enabled = true;

        animator.SetBool("Idle",false);
        customerMachine = machine;
        meshAgent.destination = lineTransform.position;

    }

    public void WaitForMachineOpen()
    {
        animator.SetBool("Idle",true);
        transform.LookAt(customerMachine.transform.position);
    }

    public void WorkingOutOver()
    {
        if (customerMachine as WorkoutMachine)
        {
            var workoutMachine = customerMachine as WorkoutMachine;
            transform.position = workoutMachine.moveTransform.position;
            meshAgent.enabled = true;
            meshAgent.destination = workoutOverPosition;

            animator.SetBool(workoutMachine.animationString,false);
            state = CustomerStates.LeavingTheGym;
            customerNeed = CustomerNeed.Nothing;
            waterTowelUI.canvas.SetActive(false);
            waterTowelUI.towelObject.SetActive(false);
            waterTowelUI.waterObject.SetActive(false);
        }
        else if (customerMachine as Pool)
        {
            var workoutMachine = customerMachine as Pool;
            transform.position = workoutMachine.GetCustomerLine(this).getOutTransform.position;
            var waitPosition = workoutMachine.GetWaitTransform(this).position 
                               + new Vector3(Random.insideUnitCircle.x,0,Random.insideUnitCircle.y)*2;
            animator.SetBool(workoutMachine.animationString,false);
            state = CustomerStates.WaitingForDemand;
            transform.DOLookAt(waitPosition, .1f);
            ((Pool)customerMachine).moneyStack.StackMoney(transform.position+Vector3.up);
            transform.DOMove(waitPosition, 1f).OnComplete(() =>
            {
                animator.SetBool("Idle",true);

                DemandForTowel();
                EventManager.CustomerLeavedPool(this);

            });
        }

    }

    public void GiveMoney()
    {
        var money = Instantiate(moneyPrefab, transform.position + Vector3.up, Quaternion.identity);
        money.GetComponent<Moneyy>().amount = customerMachine.income;
    }

    public void CustomerDemandGiven(int amount)
    {
        if (waterTowelUI.towelObject.activeSelf)
        {
            towelAnimation.SetActive(true);
        }
        else
        {
            waterAnimation.SetActive(true);
        }
        customerNeed = CustomerNeed.Nothing;
        waterTowelUI.canvas.SetActive(false);
        waterTowelUI.towelObject.SetActive(false);
        waterTowelUI.waterObject.SetActive(false);
        if (customerMachine as Pool)
        {
            ((Pool)customerMachine).moneyStack.StackMoney(transform.position+Vector3.up);
            animator.SetBool("Idle",false);

            meshAgent.enabled = true;
            meshAgent.destination = workoutOverPosition;
            
        }
        else if (customerMachine as WorkoutMachine)
        {
            ((WorkoutMachine)customerMachine).moneyStack.StackMoneyForDemand(transform.position+Vector3.up,amount);

        }
    }
    public void DemandForStackable()
    {
        if (Random.value>.5f)
        {
            DOVirtual.DelayedCall(Random.Range(0f, ((WorkoutMachine)customerMachine).workoutTime-2), () =>
            {
                var demand = EventManager.GetRandomDemand();
                switch (demand)
                {
                    case CustomerNeed.Nothing:
                        break;
                    case CustomerNeed.Towel:
                        DemandForTowel();
                        break;
                    case CustomerNeed.Water:
                        DemandForWater();
                        break;
                    case CustomerNeed.Snack:
                        DemandForSnack();
                        break;
                    
                }
            });
        }
    }

    public void DemandForTowel()
    {
        customerNeed = CustomerNeed.Towel;
        waterTowelUI.canvas.SetActive(true);
        waterTowelUI.towelObject.SetActive(true);
    }
    
    public void DemandForWater()
    {
        customerNeed = CustomerNeed.Water;
        waterTowelUI.canvas.SetActive(true);
        waterTowelUI.waterObject.SetActive(true);
    }
    public void DemandForSnack()
    {
        customerNeed = CustomerNeed.Snack;
        waterTowelUI.canvas.SetActive(true);
        waterTowelUI.snackObject.SetActive(true);
    }
    


    private void Update()
    {
        
        switch(state)
        {
            case CustomerStates.WalkingToMachine:
            {
                if (Vector3.Distance( meshAgent.destination, meshAgent.transform.position) <= meshAgent.stoppingDistance)
                {
                    if (customerMachine as WorkoutMachine)
                    {
                        meshAgent.enabled = false;
                        transform.position = ((WorkoutMachine)customerMachine).workoutTransform.position;
                        transform.rotation = ((WorkoutMachine)customerMachine).workoutTransform.rotation;
                        animator.SetBool(((WorkoutMachine)customerMachine).animationString,true);
                        ((WorkoutMachine)customerMachine).CustomerOnMachine();
                        state = CustomerStates.WorkingOut;
                        DemandForStackable();
                    }
                    if (customerMachine as Pool)
                    {
                        var line = ((Pool)customerMachine).GetCustomerLine(this);
                        meshAgent.enabled = false;
                        transform.position = line.startTransform.position;
                        transform.rotation = line.startTransform.rotation;
                        animator.SetBool(((Pool)customerMachine).animationString,true);
                        state = CustomerStates.WorkingOut;
                        transform.DOMove(line.endTransform.position, 4).OnComplete(WorkingOutOver);
                    }
                    
                }
                else
                {
                    animator.SetFloat("Movement",meshAgent.velocity.magnitude);
                }
                
            }
                break;
            case CustomerStates.WorkingOut:
                Vector3 rotation1 = Quaternion.LookRotation(customerLookAt.position).eulerAngles;
                rotation1.z = 60f;
                waterTowelUI.canvas.transform.rotation = Quaternion.Euler(rotation1);

                break;

            case CustomerStates.LeavingTheGym:
                
                animator.SetFloat("Movement",meshAgent.velocity.magnitude);
                break;
            case CustomerStates.WaitingForDemand:
                Vector3 rotation2 = Quaternion.LookRotation(customerLookAt.position).eulerAngles;
                rotation2.z = 60f;
 
                waterTowelUI.canvas.transform.rotation = Quaternion.Euler(rotation2);
                break;
            case CustomerStates.WalkingToLine:
                if (Vector3.Distance( meshAgent.destination, meshAgent.transform.position) <= meshAgent.stoppingDistance)
                {
                    if (customerMachine as WorkoutMachine)
                    {
                        waterTowelUI.canvas.SetActive(true);
                        waterTowelUI.machineImage.gameObject.SetActive(true);
                        waterTowelUI.machineImage.sprite = (customerMachine as WorkoutMachine).machineSprite;

                        meshAgent.enabled = false;
                        WaitForMachineOpen();
                        state = CustomerStates.WaitingForMachineOpen;
                    }
                    
                    
                }
                break;
            case CustomerStates.WaitingForMachineOpen:
                Vector3 rotation = Quaternion.LookRotation(customerLookAt.position).eulerAngles;
                rotation.z = 60f;
 
                waterTowelUI.canvas.transform.rotation = Quaternion.Euler(rotation);

                break;
        }

        
    }
}
