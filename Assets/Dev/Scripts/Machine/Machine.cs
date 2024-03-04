using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
   public MachineState machineState;
   public float price;
   public MachineUI machineUI;
   public GameObject machineObject;
   public int income;
   private float firstPrice;

   public virtual void Start()
   {
      firstPrice = price;
   }

   private void OnValidate()
   {
      machineUI.moneyText.text = AbbrevationUtility.AbbreviateNumber(price);
   }
   


   public bool FillUITime(float fillAmount)
   {
      machineUI.buyTimeImage.fillAmount += fillAmount;
      firstPrice -= (fillAmount*price);
      machineUI.moneyText.text = "$" + (int)firstPrice;
      return machineUI.buyTimeImage.fillAmount>=1;
   }

   public virtual void MachinePurchased()
   {
      machineState = MachineState.Open;
      machineUI.UIObject.gameObject.SetActive(false);
      machineObject.SetActive(true);
      if (machineUI.upgradeAreaOpener!=null)
      {
         machineUI.upgradeAreaOpener.SetActive(true);

      }
      EventManager.MachinePurchased(this);
   }
}
