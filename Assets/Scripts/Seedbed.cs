
using System;
using UnityEngine;

public class Seedbed :  InteractionObject
{
    [SerializeField] private PlantArea _plantArea;
    [SerializeField] private GrowthTimer _growthTimer;
    [SerializeField] private Plant _plant;
    [SerializeField] private SeedbedState state;
    [SerializeField] private Transform _plantPoint;

    /* То, что отличается у разных грядок */
    [SerializeField] private PlantsData _plantData;

    /*
    private void Start()
    {
        switch (machineState)
        {
            case MachineState.Open:
                break;
            case MachineState.Close:
                machineObject.SetActive(false);
                _plantPoint.gameObject.SetActive(false);
                break;
        }

    }*/

    private new void OnEnable()
    {
        base.OnEnable();
        EventManager.ObjectPurshuased += SeedbedPurchased;
        _plantArea.PlayerOnPlantArea += CheckState;
        _growthTimer.TimerFinish += AddPlant;
        EventManager.PlantHarvested += Harvest;

    }

    private void SeedbedPurchased()
    {
        state = SeedbedState.Empty;
        _plantArea.gameObject.SetActive(true);

    }

    private void OnDisable()
    {
        EventManager.ObjectPurshuased -= SeedbedPurchased;
    }


    private void CheckState() {
        switch (state)
        {
            case SeedbedState.Empty:
                PlantSeeds();
                break;
            case SeedbedState.Growing:
                break;
            case SeedbedState.Grown:
                EventManager.PlantHarvested?.Invoke();
                break;
        }

    }

    private void PlantSeeds() {
        state = SeedbedState.Growing;
        _growthTimer.gameObject.SetActive(true);
        _growthTimer.StartGrowthTimer(_plantData.GetGrowthTime());
        _plantArea.gameObject.SetActive(false);
    }

    private void AddPlant() {
        _growthTimer.gameObject.SetActive(false);
        _plantPoint.gameObject.SetActive(true);
        Instantiate(_plantData.GetPlant(), _plantPoint);
        state = SeedbedState.Grown;
        _plantArea.gameObject.SetActive(true);
    }

    private void Harvest()
    {
        _plant.gameObject.SetActive(false);
        state = SeedbedState.Empty;
        PickUpItems.Instanse.AddItem(_plantData);
        EventManager.UpdateUIInventory?.Invoke();
    }

    /*
    public Stackable SpawnPlant()
    {
        var plant = Instantiate(_plantPrefab, transform.position, _plantPrefab.transform.rotation);
        return plant.GetComponent<Stackable>();
    }

    public bool CanHarvest() {
        if (state == SeedbedState.Grown) {
            return true;
        }
        return false;
    }
    */   
}
