using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbellMachine : WorkoutMachine
{
    public override void CustomerOnMachine()
    {
        base.CustomerOnMachine();
        machineCustomer.ActivateDumbells(true);


    }

    public override void RemoveCustomer()
    {
        base.RemoveCustomer();
        machineCustomer.ActivateDumbells(false);


    }
}
