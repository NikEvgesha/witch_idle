using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public int totalMoneyAmount;

    [Button]
    public void Save()
    {
        ES3.Save("gameData",this);
    }

    public void Load()
    {
        if (ES3.KeyExists("gameData"))
        {
            totalMoneyAmount = (ES3.Load("gameData") as GameData).totalMoneyAmount;

        }
    }

   
}
