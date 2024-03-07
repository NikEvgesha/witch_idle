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