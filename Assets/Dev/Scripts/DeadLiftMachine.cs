using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeadLiftMachine : WorkoutMachine
{
    public Transform bench;
    
    

    public override void CustomerOnMachine()
    {
        base.CustomerOnMachine();
        DOVirtual.DelayedCall(.2f, () =>
        {
            bench.gameObject.SetActive(false);
            machineCustomer.ActivateDeadLiftBar(true);
        });


    }

    public override void RemoveCustomer()
    {
        base.RemoveCustomer();
        DOTween.Kill(this);
        machineCustomer.ActivateDeadLiftBar(false);
        bench.gameObject.SetActive(true);

    }
}
