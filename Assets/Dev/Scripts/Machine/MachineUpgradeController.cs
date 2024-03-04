using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MachineUpgradeController : MonoBehaviour
{
    public WorkoutMachine workoutMachine;
    public int speedLevel;
    public int incomeLevel;
    public Button speedUpgradeButton;
    public TextMeshProUGUI speedUpgradePriceText;
    public TextMeshProUGUI speedLevelText;

    public Button incomeUpgradeButton;
    public TextMeshProUGUI incomeUpgradePriceText;
    public TextMeshProUGUI incomeLevelText;


    private void OnEnable()
    {
        EventManager.StartGame += StartGame;
        EventManager.MoneyUpdated += SetUpgradeValues;
    }

    private void StartGame()
    {
        var data = EventManager.GetLevelMachineData().GetMachineData(transform.position, workoutMachine.type);
        if (data != null)
        {
            if (data.state == MachineState.Open)
            {
                workoutMachine.MachinePurchased();
                speedLevel = data.speedLevel;
                incomeLevel = data.incomeLevel;
            }
            else
            {
                EventManager.CloseMachine(workoutMachine);

            }
        }
        else
        {
            EventManager.CloseMachine(workoutMachine);
        }
        

        SetUpgradeValues();
    }

    private void OnDisable()
    {
        EventManager.StartGame -= StartGame;
        EventManager.MoneyUpdated -= SetUpgradeValues;
    }

    public void UpgradeIncome()
    {
        List<UpgradePrices> tempIncomeList = new List<UpgradePrices>();

        foreach (var obj in EventManager.GetMachineUpgradeData().machineData[SceneManager.GetActiveScene().buildIndex-1].machineData)
        {
            if (obj.type == (workoutMachine).type)
            {
                tempIncomeList = obj.incomeValues;
                break;
            }
        }

        EventManager.GetGameData().totalMoneyAmount -= tempIncomeList[incomeLevel].price;
        EventManager.MoneyUpdated();
        incomeLevel++;
        SetUpgradeValues();
    }

    public void UpgradeSpeed()
    {
        List<UpgradePrices> tempSpeedList = new List<UpgradePrices>();

        foreach (var obj in EventManager.GetMachineUpgradeData().machineData[SceneManager.GetActiveScene().buildIndex-1].machineData)
        {
            if (obj.type == (workoutMachine).type)
            {
                tempSpeedList = obj.speedValues;
                break;
            }
        }

        EventManager.GetGameData().totalMoneyAmount -= tempSpeedList[speedLevel].price;
        EventManager.MoneyUpdated();
        speedLevel++;
        SetUpgradeValues();
    }

    public void SetUpgradeValues()
    {
        List<UpgradePrices> tempSpeedList = new List<UpgradePrices>();
        List<UpgradePrices> tempIncomeList = new List<UpgradePrices>();

        foreach (var obj in EventManager.GetMachineUpgradeData().machineData[SceneManager.GetActiveScene().buildIndex-1].machineData)
        {
            if (obj.type == (workoutMachine).type)
            {
                tempSpeedList = obj.speedValues;
                tempIncomeList = obj.incomeValues;
                break;
            }
        }

        workoutMachine.income = tempIncomeList[incomeLevel].amount;
        workoutMachine.workoutTime = tempSpeedList[speedLevel].amount;
        speedLevelText.text = "Lvl." + (speedLevel+1);
        incomeLevelText.text = "Lvl." + (incomeLevel+1);

        var moneyAmount = EventManager.GetGameData().totalMoneyAmount;
        if (speedLevel >= tempSpeedList.Count - 1)
        {
            speedUpgradePriceText.text = "MAX";
            speedUpgradeButton.interactable = false;
        }
        else
        {
            speedUpgradePriceText.text = AbbrevationUtility.AbbreviateNumber(tempSpeedList[speedLevel + 1].price);
            if (moneyAmount < tempSpeedList[speedLevel + 1].price)
            {
                speedUpgradeButton.interactable = false;
            }
            else
            {
                speedUpgradeButton.interactable = true;

            }
        }

        if (incomeLevel >= tempIncomeList.Count - 1)
        {
            incomeUpgradePriceText.text = "MAX";
            incomeUpgradeButton.interactable = false;
        }
        else
        {
            incomeUpgradePriceText.text = AbbrevationUtility.AbbreviateNumber(tempIncomeList[incomeLevel + 1].price);
            if (moneyAmount < tempIncomeList[incomeLevel + 1].price)
            {
                incomeUpgradeButton.interactable = false;
            }
            else
            {
                incomeUpgradeButton.interactable = true;

            }
        }

        var data = new MachinesData();
        data.type = workoutMachine.type;
        data.incomeLevel = incomeLevel;
        data.speedLevel = speedLevel;
        data.position = transform.position;
        data.state = workoutMachine.machineState;
        EventManager.GetLevelMachineData().MachineAction(data);
    }
}