using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "WitchScripts/PlantsData")]
public class PlantsData : ScriptableObject
{
    [SerializeField] private Plant _plantPrefab;
    [SerializeField] private float _growthTime;
    [SerializeField] private PlantTypes _plantType;
    [SerializeField] private PlantIcon _plantIcon;
    public float GetGrowthTime()
    {
        return _growthTime;
    }

    public Plant GetPlant()
    {
        return _plantPrefab;
    }

    public PlantIcon GetPlantIcon()
    {
        return _plantIcon;
    }
    public PlantTypes GetPlantType()
    {
        return _plantType;
    }

}
