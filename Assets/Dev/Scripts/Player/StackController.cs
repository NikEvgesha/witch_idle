using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackController : MonoBehaviour
{
   public List<Stackable> stackObjects;
   public Transform stackStartTransform;
   public Transform stackPositionIndex;

   public int maxStackCount;
   public void StackObject(Stackable obj)
   {
      obj.transform.parent = stackStartTransform;
      obj.transform.localPosition = stackPositionIndex.localPosition;
      obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
      stackPositionIndex.localPosition += new Vector3(0, obj.colliderBound, 0);
      stackObjects.Add(obj);
   }

   public bool CheckIfCanBuyStackable(CustomerNeed stackableType)
   {
      int tempAmount = 0;

      foreach (var obj in stackObjects)
      {
         if (obj.itemType==stackableType)
         {
            tempAmount++;
         }
      }

      if (tempAmount>=maxStackCount)
      {
         return false;
      }
      else
      {
         return true;
      }
      
   }

   public void ReArrangeStack()
   {
      stackPositionIndex.position = stackStartTransform.position;
      for (int i = 0; i < stackObjects.Count; i++)
      {
         stackObjects[i].transform.position = stackPositionIndex.position;
         stackPositionIndex.localPosition += new Vector3(0, stackObjects[i].colliderBound, 0);
      }
   }

   public void CheckIfStackHasItem(Customer customer)
   {
      foreach (var obj in stackObjects)
      {
         if (obj.itemType==customer.customerNeed)
         {
            var item = obj;
            item.transform.parent = null;
            stackObjects.Remove(obj);
            customer.customerNeed = CustomerNeed.Nothing;
            customer.CustomerDemandGiven(obj.income);
            ReArrangeStack();
            item.transform.DOJump(customer.transform.position, 2, 1, .5f).OnComplete(() =>
            { 
               Destroy(item.gameObject);
            });
            return;
         }
      }

   }
}
