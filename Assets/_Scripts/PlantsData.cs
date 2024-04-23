using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "WitchScripts/PlantsData")]
public class PlantsData : ScriptableObject
{
    [SerializeField] private Plant _plantPrefab;
    [SerializeField] private float _growthTime;
    [SerializeField] private InventoryItem _inventoryItem;

    public InventoryItem GetItem() {
        return _inventoryItem;
    }
    public float GetGrowthTime()
    {
        return _growthTime;
    }

    public Plant GetPlant()
    {
        return _plantPrefab;
    }

}
