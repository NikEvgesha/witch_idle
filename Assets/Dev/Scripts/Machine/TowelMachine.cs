using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowelMachine : Machine
{
    public GameObject towelPrefab;
    public int towelPrice;
    
    private void OnEnable()
    {
        EventManager.StartGame += StartGame;
    }

    private void OnDisable()
    {
        EventManager.StartGame -= StartGame;
    }

    private void StartGame()
    {
        var data = EventManager.GetLevelMachineData().GetStackableMachineData(transform.position);
        if (data != null)
        {
            if (data.state == MachineState.Open)
            {
                MachinePurchased();
            }
        }
    }
    public Stackable SpawnTower()
    {
        var bottle = Instantiate(towelPrefab, transform.position,Quaternion.identity);
        return bottle.GetComponent<Stackable>();
    }
    public bool FillBuyUI(float fillAmount)
    {
        machineUI.workoutTimeImage.fillAmount += fillAmount;

        return machineUI.workoutTimeImage.fillAmount>=1;
    }
    
    public override void MachinePurchased()
    {
        base.MachinePurchased();
        var data = new MachinesData();
        data.position = transform.position;
        data.state = machineState;
        EventManager.GetLevelMachineData().StackableMachineAction(data);
    }
}
