using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/NPS/Type")]
public class NPSType : ScriptableObject
{
    /*
     »м€ ? или тип ? или он сам €вл€етс€ типом себ€
    “ело - скин
    “овар который он хочет получить
    ¬озможно список товаров из которых он может выбрать случайное
    —ложность товара, чтобы по началу давать только то простые.
    Ќужно провер€ть на то, какие товары в целом мы можем сделать.

     */
    [SerializeField] private string _name;
    [SerializeField] private GameObject _skin;
    [SerializeField] private InventoryItem _potion;
    [SerializeField] private List<InventoryItem> _potions = new List<InventoryItem>();
    private int _minLevel = int.MaxValue;
    //private bool _minLevelInit = false;
    
    public InventoryItem SelectPotion(List<PotionTypes> dontUsePotion)
    {
        int level = WitchPlayerController.Instanse.PlayerLevel;
        List<InventoryItem> potions = new List<InventoryItem>();

        foreach (InventoryItem potion in _potions)
        {
            if (potion.GetLevelUnlockRecept() <= level && CheckUsePotion(potion.GetPotionType(), dontUsePotion))
            {
                potions.Add(potion);
            }
        }

        int random = 0;
        random = Random.Range(0, potions.Count);
        _potion = potions[random];
        return _potion;
    }
    private bool CheckUsePotion(PotionTypes potion, List<PotionTypes> dontUsePotion)
    {
        foreach (PotionTypes item in dontUsePotion)
        {
            if (potion == item) return false;
        }

        return true;
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
