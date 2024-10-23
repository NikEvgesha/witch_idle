using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct InteractionObjectUI
{
    public GameObject UIObject;
    public TextMeshProUGUI MoneyText;
    public Image BuyTimeImage;
    public Image ProductionTimeImage;
    public CheckPlayer UpgradeArea;
}
[Serializable]
public struct BuyStruct
{
    public GameObject UIObject;
    public TextMeshProUGUI MoneyText;
    public Image BuyTimeImage;
    public CheckPlayer _buyArea;
}
[Serializable]
public struct WorkStruct
{
    public GameObject UIObject;
}
[Serializable]
public struct LockStruct
{
    public GameObject UIObject;
    public TextMeshProUGUI LevelText;
}