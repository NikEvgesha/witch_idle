using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "WitchScripts/PlantsData")]
public class PlantsData : ScriptableObject
{
    [SerializeField] private GameObject _plantPrefab;
    [SerializeField] private float _growthTime;
    [SerializeField] private PlantTypes _plantType;
    [SerializeField] private GameObject _plantIcon;

    public float GetGrowthTime()
    {
        return _growthTime;
    }

    public GameObject GetPlant()
    {
        return _plantPrefab;
    }

    public GameObject GetPlantIcon()
    {
        return _plantIcon;
    }

}
