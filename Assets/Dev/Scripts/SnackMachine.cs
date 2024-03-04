using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackMachine : Machine
{
    public GameObject snackPrefab;
    public int snackPrice;
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
    public Stackable SpawnSnack()
    {
        var bottle = Instantiate(snackPrefab, transform.position,Quaternion.identity);
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
