using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pool : Machine
{
    public List<PoolLine> poolLines;
    public string animationString;
    public MoneyStack moneyStack;
    public GameObject poolLockUI;
    private void OnEnable()
    {
        EventManager.CustomerLeavedPool += CustomerLeavedPool;
    }

    private void OnDisable()
    {
        EventManager.CustomerLeavedPool -= CustomerLeavedPool;
    }


    public override void MachinePurchased()
    {
        base.MachinePurchased();
        poolLockUI.SetActive(false);
    }

    private void CustomerLeavedPool(Customer customer)
    {
        foreach (var line in poolLines)
        {
            if (line.customer==customer)
            {
                line.isBusy = false;
            }
        }
    }

    public PoolLine GetRandomLineTransform(Customer customer)
    {
        foreach (var line in poolLines)
        {
            if (!line.isBusy)
            {
                line.customer = customer;
                line.isBusy = true;
                return line;

            }
        }

        return poolLines[0];
    }
    public bool CheckIfPoolAvailable()
    {
        foreach (var line in poolLines)
        {
            if (!line.isBusy)
            {
                return true;
            }
        }
        return false;
    }
    public Transform GetWaitTransform(Customer customer)
    {
        foreach (var line in poolLines)
        {
            if (line.customer==customer)
            {
                return line.waitForDemandTransform;
            }
        }

        return null;

    }

    public PoolLine GetCustomerLine(Customer customer)
    {
        foreach (var line in poolLines)
        {
            if (line.customer==customer)
            {
                return line;
            }
        }

        return poolLines[0];
    }
    
}
