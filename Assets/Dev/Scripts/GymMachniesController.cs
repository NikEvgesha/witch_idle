using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymMachniesController : MonoBehaviour
{
    public List<Machine> machines;

    private int index;

    private void OnEnable()
    {
        EventManager.SetGymMachines += SetGymMachines;
        EventManager.MachinePurchased += MachinePurchased;
    }

    private void MachinePurchased(Machine obj)
    {
        if (index<machines.Count)
        {
            if (machines.Contains(obj))
            {
                machines[index].gameObject.SetActive(true);
                index++;
            }
        }
       
    }

    private void OnDisable()
    {
        EventManager.SetGymMachines -= SetGymMachines;
        EventManager.MachinePurchased += MachinePurchased;
    }

    private void SetGymMachines()
    {
        var openMachines = new List<Machine>();
        foreach (var machine in machines)
        {
            if (machine.machineState == MachineState.Open)
            {
                openMachines.Add(machine);
            }
        }

        foreach (var machine in openMachines)
        {
            machines.Remove(machine);
        }
        for (int i = 0; i < machines.Count; i++)
        {
            if (i==0)
            {
                machines[i].gameObject.SetActive(true);
            }
            else
            {
                machines[i].gameObject.SetActive(false);

            }
        }

        index++;
    }
}
