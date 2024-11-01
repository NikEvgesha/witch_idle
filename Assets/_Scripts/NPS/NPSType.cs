using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/NPS/Type")]
public class NPSType : ScriptableObject
{
    /*
     ��� ? ��� ��� ? ��� �� ��� �������� ����� ����
    ���� - ����
    ����� ������� �� ����� ��������
    �������� ������ ������� �� ������� �� ����� ������� ���������
    ��������� ������, ����� �� ������ ������ ������ �� �������.
    ����� ��������� �� ��, ����� ������ � ����� �� ����� �������.

     */
    [SerializeField] private string _name;
    [SerializeField] private GameObject _skin;
    [SerializeField] private InventoryItem _potion;
    [SerializeField] private List<InventoryItem> _potions = new List<InventoryItem>();
    private int _minLevel = int.MaxValue;
    //private bool _minLevelInit = false;
    
    public InventoryItem SelectPotion()
    {
        int level = WitchPlayerController.Instanse.PlayerLevel;
        List<InventoryItem> potions = new List<InventoryItem>();

        foreach (InventoryItem potion in _potions)
        {
            if (potion.GetLevelUnlockRecept() <= level)
            {
                potions.Add(potion);
            }
        }

        int random = 0;
        random = Random.Range(0, potions.Count);
        _potion = potions[random];
        return _potion;
    }
    public GameObject GetSkin() 
    { 
        return _skin; 
    }
    public int GetMinLevel()
    {
        int minLevel = int.MaxValue;
        foreach (InventoryItem potion in _potions)
        {
            if (minLevel > potion.GetLevelUnlockRecept())
            {
                minLevel = potion.GetLevelUnlockRecept();
            }
        }
        _minLevel = minLevel;
        return _minLevel;
    }

}
