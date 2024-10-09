using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Potions
{
    public InventoryItem Potion;
    public int MinLevelUse;
}
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
    [SerializeField] private List<Potions> _potions = new List<Potions>();
    
    public InventoryItem SelectPotion(int level = -1)
    {
        List<Potions> potions = new List<Potions>();
        if (level == -1)
        {
            potions = _potions;
        }
        else
        {
            foreach (Potions potion in _potions)
            {
                if (potion.MinLevelUse <= level)
                {
                    potions.Add(potion);
                }
            }
        }
        int random = 0;
        random = Random.Range(0, potions.Count);
        _potion = potions[random].Potion;
        return _potion;
    }
    public GameObject GetSkin() 
    { 
        return _skin; 
    }


}
