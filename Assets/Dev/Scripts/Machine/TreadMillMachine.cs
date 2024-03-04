using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TreadMillMachine : WorkoutMachine
{
   public Renderer renderer;

   public override void CustomerOnMachine()
   {
      base.CustomerOnMachine();
      DOVirtual.Float(0, -10, workoutTime, (x) =>
      {
         renderer.materials[1].mainTextureOffset = new Vector2(x,0);

      });
   }
}
