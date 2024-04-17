using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;


    private void Start()
    {
        UpdateMoneyText();
    }

    private void OnEnable()
    {
        EventManager.MoneyUpdated += UpdateMoneyText;
    }

    private void OnDisable()
    {
        EventManager.MoneyUpdated -= UpdateMoneyText;
    }

    public void UpdateMoneyText()
    {
       // moneyText.text = AbbrevationUtility.AbbreviateNumber(EventManager.GetGameData().totalMoneyAmount);
    }

    public void OpenWorldMap()
    {
        SceneManager.LoadScene(0);
    }

    public void AddMoney()
    {
        EventManager.GetGameData().totalMoneyAmount += 1000;
        EventManager.MoneyUpdated();
    }
}
