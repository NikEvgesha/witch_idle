using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class MapCanvasController : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    public List<int> unlockedLevels;
    
    

    private void OnEnable()
    {
        EventManager.MoneyUpdated += MoneyUpdated;
        EventManager.StartGame += StartGame;
        EventManager.LevelUnlocked += LevelUnlocked;
    }

    private void MoneyUpdated()
    {
        moneyText.text = AbbrevationUtility.AbbreviateNumber(EventManager.GetGameData().totalMoneyAmount);
    }

    [Button]
    public void ResetUnlockedLevels()
    {
        if (ES3.KeyExists("levels"))
        {
            unlockedLevels = ES3.Load("levels") as List<int>;
            unlockedLevels.Clear();
            ES3.Save("levels",unlockedLevels);
        }
    }

    private void StartGame()
    {
        moneyText.text = AbbrevationUtility.AbbreviateNumber(EventManager.GetGameData().totalMoneyAmount);
        if (ES3.KeyExists("levels"))
        {
            unlockedLevels = ES3.Load("levels") as List<int>;
            EventManager.SetUnlockedLevels(unlockedLevels);
        }
    }

    private void OnDisable()
    {
        EventManager.MoneyUpdated -= MoneyUpdated;
        EventManager.StartGame -= StartGame;
        EventManager.LevelUnlocked -= LevelUnlocked;
    }

    private void LevelUnlocked(int obj)
    {
        if (!unlockedLevels.Contains(obj))
        {
            unlockedLevels.Add(obj);
        }
        
        ES3.Save("levels",unlockedLevels);
    }
}
