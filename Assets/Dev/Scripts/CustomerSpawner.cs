using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerSpawner : MonoBehaviour
{
    public List<Machine> availableGymMachines;
    public List<Machine> busyGymMachines;
    public List<Machine> closeGymMachines;
    public List<Machine> stackableMachines;

    public GameObject customerPrefab;
    public List<Transform> randomSpawnPoints;
    public List<Material> customerMaterials;
    private float timer;
    public LineManager lineManager;

    private void OnEnable()
    {
        EventManager.GetRandomDemand += GetRandomDemand;
        EventManager.CloseMachine += CloseMachine;
        EventManager.CustomerWorkOutOver += CustomerWorkOutOver;
        EventManager.MachinePurchased += MachinePurchased;
    }

    private CustomerNeed GetRandomDemand()
    {
        if (stackableMachines.Count==0)
        {
            return CustomerNeed.Nothing;
        }
        else
        {
            var randMachine = stackableMachines[Random.Range(0, stackableMachines.Count)];
            if (randMachine as WaterMachine)
            {
                return CustomerNeed.Water;
            }
            else if (randMachine as TowelMachine)
            {
                return CustomerNeed.Towel;
                
            }
            else
            {
                return CustomerNeed.Snack;
            }
        }
    }

    private void CloseMachine(Machine obj)
    {
        closeGymMachines.Add(obj);
    }


    private void CustomerWorkOutOver(Machine obj)
    {
        availableGymMachines.Add(obj);
        busyGymMachines.Remove(obj);
    }

    private void MachinePurchased(Machine obj)
    {
        if (obj as WorkoutMachine || obj as Pool)
        {
            availableGymMachines.Add(obj);
            closeGymMachines.Remove(obj);
        }
        else if (obj as WaterMachine || obj as TowelMachine || obj as SnackMachine)
        {
            stackableMachines.Add(obj);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > .1f)
        {
            timer = 0;
            SpawnCustomer();
        }
    }

    private void OnDisable()
    {
        EventManager.GetRandomDemand -= GetRandomDemand;
        EventManager.CloseMachine -= CloseMachine;
        EventManager.CustomerWorkOutOver -= CustomerWorkOutOver;
        EventManager.MachinePurchased -= MachinePurchased;
    }

    public bool CheckLineCustomerCanWorkout()
    {
        foreach (var customer in lineManager.lineCustomers)
        {
            foreach (var machine in availableGymMachines)
            {
                if (customer.customerMachine == machine)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Customer GetLineCustomerCanWorkout()
    {
        foreach (var customer in lineManager.lineCustomers)
        {
            foreach (var machine in availableGymMachines)
            {
                if (customer.customerMachine == machine)
                {
                    return customer;
                }
            }
        }

        return null;
    }


    [Button]
    public void SpawnCustomer()
    {
        if (availableGymMachines.Count != 0)
        {
            if (CheckLineCustomerCanWorkout())
            {
                var customer = GetLineCustomerCanWorkout();
                var workoutMachine = customer.customerMachine as WorkoutMachine;
                workoutMachine.CustomerComing(customer);
                customer.MoveToMachine(workoutMachine.moveTransform, workoutMachine);
                customer.workoutOverPosition = randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)].position;
                lineManager.lineCustomers.Remove(customer);
                lineManager.ReArrangeLine();
                availableGymMachines.Remove(workoutMachine);
                busyGymMachines.Add(workoutMachine);
            }
            else
            {
                var machine = availableGymMachines[Random.Range(0, availableGymMachines.Count)];
                if (machine as WorkoutMachine)
                {
                    var workoutMachine = machine as WorkoutMachine;
                    var customer = Instantiate(customerPrefab,
                        randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity,
                        transform).GetComponent<Customer>();
                    workoutMachine.CustomerComing(customer);
                    customer.SetCustomer(customerMaterials[Random.Range(0, customerMaterials.Count)]);
                    customer.MoveToMachine(workoutMachine.moveTransform, workoutMachine);
                    customer.workoutOverPosition = randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)].position;
                    availableGymMachines.Remove(workoutMachine);
                    busyGymMachines.Add(workoutMachine);
                }
                else if (machine as Pool)
                {
                    var pool = machine as Pool;
                    if (pool.CheckIfPoolAvailable())
                    {
                        var customer = Instantiate(customerPrefab,
                            randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity,
                            transform).GetComponent<Customer>();
                        var randLine = pool.GetRandomLineTransform(customer);
                        customer.SetCustomer(customerMaterials[Random.Range(0, customerMaterials.Count)]);
                        customer.MoveToMachine(randLine.moveTransform, pool);
                        customer.workoutOverPosition =
                            randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)].position;
                    }
                }
            }
        }
        else if (closeGymMachines.Count != 0)
        {
            if (lineManager.CheckIfLineHasPosition())
            {
                var machine = closeGymMachines[Random.Range(0, closeGymMachines.Count)];
                var customer = Instantiate(customerPrefab,
                    randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity,
                    transform).GetComponent<Customer>();
                customer.state = CustomerStates.WalkingToLine;
                customer.SetCustomer(customerMaterials[Random.Range(0, customerMaterials.Count)]);
                customer.GoToLine(lineManager.GetLineTransform(), machine);
                lineManager.lineCustomers.Add(customer);
            }
        }
        else if (busyGymMachines.Count != 0)
        {
            if (lineManager.CheckIfLineHasPosition())
            {
                var machine = busyGymMachines[Random.Range(0, busyGymMachines.Count)];
                var customer = Instantiate(customerPrefab,
                    randomSpawnPoints[Random.Range(0, randomSpawnPoints.Count)].position, Quaternion.identity,
                    transform).GetComponent<Customer>();
                customer.state = CustomerStates.WalkingToLine;
                customer.SetCustomer(customerMaterials[Random.Range(0, customerMaterials.Count)]);
                customer.GoToLine(lineManager.GetLineTransform(), machine);
                lineManager.lineCustomers.Add(customer);
            }
        }
    }
}