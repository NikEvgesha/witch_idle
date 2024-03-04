using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InclinePressMachine : WorkoutMachine
{
    public override void CustomerOnMachine()
    {
        base.CustomerOnMachine();
        machineCustomer.ActivateInclineDumbells(true);


    }

    public override void RemoveCustomer()
    {
        base.RemoveCustomer();
        machineCustomer.ActivateInclineDumbells(false);


    }
}
