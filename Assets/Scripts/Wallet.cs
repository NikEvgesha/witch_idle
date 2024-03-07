using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : ScriptableObject
{
    [SerializeField]
    private int _money;
    public int Money 
    { 
        get { return _money; }
        set 
        {
            _money = value;
            // Функция сохранения значения
            // Функция изменения UI
        }
    }
}
