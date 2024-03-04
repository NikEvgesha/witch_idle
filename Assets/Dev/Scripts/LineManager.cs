using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public List<Transform> lineTransforms;
    public int lineIndex;

    public List<Customer> lineCustomers;
    public Transform GetLineTransform()
    {
        var tr = lineTransforms[lineIndex];
            lineIndex++;
            return tr;

    }

    public void ReArrangeLine()
    {
        lineIndex = 0;
        for (int i = 0; i < lineCustomers.Count; i++)
        {
            lineCustomers[i].state = CustomerStates.WalkingToLine;
            lineCustomers[i].GoToLine(lineTransforms[i], lineCustomers[i].customerMachine);
            lineIndex++;
        }
    }
    

    public bool CheckIfLineHasPosition()
    {
        if (lineIndex>=lineTransforms.Count)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
