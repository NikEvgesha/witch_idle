using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkoutMachine : Machine
{
    public WorkoutMachineTypes type;
    public WorkoutMachineState workoutMachineState;
    public string animationString;
    public Transform workoutTransform;
    public Transform moveTransform;
    public Customer machineCustomer;
    public float workoutTime;
    private float timer;
    public MoneyStack moneyStack;
    public Sprite machineSprite;
    private void Update()
    {
        switch (workoutMachineState)
        {
            case WorkoutMachineState.Available:
                break;
            case WorkoutMachineState.Busy:
                CustomerWorkingOut();
                break;
            case WorkoutMachineState.CustomerComing:
                break;
        }
    }

    public void CustomerWorkingOut()
    {
        timer += Time.deltaTime;
        machineUI.workoutTimeImage.fillAmount = timer / workoutTime;
        if (timer>=workoutTime)
        {
            if (machineCustomer.customerNeed==CustomerNeed.Nothing)
            {
                RemoveCustomer();
                timer = 0;
            }
        }
    }

    public void CustomerComing(Customer customer)
    {
        workoutMachineState = WorkoutMachineState.CustomerComing;
        machineCustomer = customer;
    }
    public virtual void CustomerOnMachine()
    {
        workoutMachineState = WorkoutMachineState.Busy;
    }

    public virtual void RemoveCustomer()
    {
        machineUI.workoutTimeImage.fillAmount = 0;
        machineCustomer.WorkingOutOver();
        workoutMachineState = WorkoutMachineState.Available;
        moneyStack.StackMoney(machineCustomer.transform.position + Vector3.up);
        EventManager.CustomerWorkOutOver(this);
    }
}
