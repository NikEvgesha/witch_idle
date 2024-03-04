using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct MachineUI
{
    public GameObject UIObject;
    public TextMeshProUGUI moneyText;
    public Image buyTimeImage;
    public Image workoutTimeImage;
    public GameObject upgradeAreaOpener;
}
[Serializable]
public struct CustomerWaterTowelUI
{
    public GameObject canvas;
    public GameObject towelObject;
    public GameObject waterObject;
    public GameObject snackObject;

    public SpriteRenderer machineImage;
}
[Serializable]
public struct UpgradeCanvasItem
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI priceText;
    public Button itemButton;
    public int level;
}
[Serializable]
public struct UpgradePrices
{
    public int amount;
    public int price;
}
[Serializable]
public class MachinesData
{
    public Vector3 position;
    public WorkoutMachineTypes type;
    public MachineState state;
    public int speedLevel;
    public int incomeLevel;

}
[Serializable]
public class LevelMachinesData
{
    public List<MachinesData> data;

}
[Serializable]
public class PoolLine
{
    public Customer customer;
    public Transform moveTransform;
    public Transform startTransform;
    public Transform endTransform;
    public Transform getOutTransform;
    public Transform waitForDemandTransform;

    public bool isBusy;

}
[Serializable]
public struct LevelMachineUpgradeData
{
    public List<MachineUpgradeObject> machineData;

}
[Serializable]
public struct MachineUpgradeObject
{
    public WorkoutMachineTypes type;
    public List<UpgradePrices> speedValues;
    public List<UpgradePrices> incomeValues;

}