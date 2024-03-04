using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapUILevel : MonoBehaviour
{
    public GameObject lockObject;
    public Button buyButton;
    public int price;
    public GameObject earthLock;
    public TextMeshProUGUI priceText;
    public int levelIndex;
    public int openButton;
    

    private void OnEnable()
    {
        EventManager.SetUnlockedLevels += SetUnlockedLevels;
        EventManager.StartGame += StartGame;
    }

    private void SetUnlockedLevels(List<int> obj)
    {
        if (obj.Contains(levelIndex))
        {
            UnlockLevel();
        }
    }

    private void OnDisable()
    {
        EventManager.SetUnlockedLevels -= SetUnlockedLevels;
        EventManager.StartGame -= StartGame;
    }

    private void StartGame()
    {
        if (buyButton!=null)
        {
            if (EventManager.GetGameData().totalMoneyAmount <= price)
            {
                buyButton.interactable = false;
            }
            else
            {
                buyButton.interactable = true;
            }
            priceText.text = AbbrevationUtility.AbbreviateNumber(price);

            buyButton.onClick.AddListener(BuyLevel);
        }
       
    }

    public void UnlockLevel()
    {
        lockObject.SetActive(false);
        earthLock.SetActive(false);
    }

    public void BuyLevel()
    {
        EventManager.GetGameData().totalMoneyAmount -= price;
        EventManager.MoneyUpdated();
        lockObject.SetActive(false);
        earthLock.SetActive(false);
        EventManager.LevelUnlocked(levelIndex);
    }

    public void OpenLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}