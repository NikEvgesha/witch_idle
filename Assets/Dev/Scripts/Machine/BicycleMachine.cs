using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BicycleMachine : WorkoutMachine
{
   public Transform wheel;
   public float animSpeed;
   public float degree;
   public override void CustomerOnMachine()
   {
      base.CustomerOnMachine();
      wheel.DORotate(new Vector3(-degree, 0, 0), animSpeed, RotateMode.LocalAxisAdd).SetId(this);
   }

   public override void RemoveCustomer()
   {
      base.RemoveCustomer();
      DOTween.Kill("this");
      wheel.DOLocalRotate(new Vector3(14,0,0),.1f);
   }
}
