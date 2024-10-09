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
    [SerializeField] private GameObject _product;
    [SerializeField] private List<GameObject> _products;
    [SerializeField] private int _difficulty;
    
    private void CheckPlayerProgress()
    {

    }

}
