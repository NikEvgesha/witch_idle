using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoneyStack : MonoBehaviour
{
    public List<Moneyy> stackObjects;
    public Transform stackStartTransform;
    public Transform stackPositionIndex;
    public GameObject moneyPrefab;

    public int PlayerCollectedAllMoney(Vector3 jumpPos)
    {
        var allMoneyAmount = 0;
        foreach (var obj in stackObjects)
        {
            allMoneyAmount += obj.amount;
        }

        for (int i = 0; i <stackObjects.Count; i++)
        {
            var temp = stackObjects[i];
            DOVirtual.DelayedCall(i * .1f, () =>
            {
                temp.collider.isTrigger = true;
                EventManager.PlayerCollectedMoney(temp.amount);
                temp.transform.DOJump(jumpPos, 2, 1, .1f).OnComplete(() => { Destroy(temp.gameObject); });
            });
        }

        stackObjects.Clear();
        ReArrangeStack();
        return allMoneyAmount;
    }
    [Button]
    public void StackMoneyWithIncome(Vector3 startPos,int income)
    {
        var obj = Instantiate(moneyPrefab, startPos, Quaternion.identity).GetComponent<Moneyy>();
        obj.transform.DOJump(stackPositionIndex.position, 2, 1, .5f);

        obj.amount = income;

        obj.transform.parent = stackStartTransform;
        obj.transform.position = stackPositionIndex.position;
        stackPositionIndex.localPosition += new Vector3(0, obj.boundY, 0);
        stackObjects.Add(obj);
    }
    [Button]
    public void StackMoney(Vector3 startPos)
    {
        var obj = Instantiate(moneyPrefab, startPos, Quaternion.identity).GetComponent<Moneyy>();
        obj.transform.DOJump(stackPositionIndex.position, 2, 1, .5f);

        obj.amount = GetComponentInParent<Machine>().income;

        obj.transform.parent = stackStartTransform;
        obj.transform.position = stackPositionIndex.position;
        stackPositionIndex.localPosition += new Vector3(0, obj.boundY, 0);
        stackObjects.Add(obj);
    }
    [Button]
    public void StackMoneyForDemand(Vector3 startPos,int amount)
    {
        var obj = Instantiate(moneyPrefab, startPos, Quaternion.identity).GetComponent<Moneyy>();
        obj.transform.DOJump(stackPositionIndex.position, 2, 1, .5f);

        obj.amount = amount;

        obj.transform.parent = stackStartTransform;
        obj.transform.position = stackPositionIndex.position;
        stackPositionIndex.localPosition += new Vector3(0, obj.boundY, 0);
        stackObjects.Add(obj);
    }

    public void ReArrangeStack()
    {
        stackPositionIndex.position = stackStartTransform.position;
    }
}