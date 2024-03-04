using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BenchPressMachine : WorkoutMachine
{
    public Transform bench;
    
    

    public override void CustomerOnMachine()
    {
        base.CustomerOnMachine();
        DOVirtual.DelayedCall(.2f, () =>
        {
            bench.gameObject.SetActive(false);
            machineCustomer.ActivateBenchVar(true);
        });


    }

    public override void RemoveCustomer()
    {
        base.RemoveCustomer();
        DOTween.Kill(this);
        machineCustomer.ActivateBenchVar(false);
        bench.gameObject.SetActive(true);

    }
}
