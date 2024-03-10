
using System;
using Unity.VisualScripting;
using UnityEngine;

public class Seedbed :  InteractionObject
{
    //[SerializeField] private PlantArea _plantArea;
    [SerializeField] CheckPlayer _plantArea;
    [SerializeField] private GrowthTimer _growthTimer;
    //[SerializeField] private Plant _plant;
    [SerializeField] private SeedbedState state;
    [SerializeField] private Transform _plantPoint;

    /* То, что отличается у разных грядок */
    [SerializeField] private PlantsData _plantData;
    private Plant _plant;

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
        LoadSeedBed();
        //_plantArea.PlayerOnPlantArea += CheckState;
        _plantArea.OnTrigger += TryCollect;
        _growthTimer.TimerFinish += AddPlant;
        //EventManager.PlantHarvested += Harvest; Зачем? мы же от отсюда этот эвент и отправляем
    }

    private void LoadSeedBed()
    {
        state = SeedbedState.Empty;
        //_plantArea.gameObject.SetActive(true);

    }

    private void OnDisable()
    {
        _plantArea.OnTrigger -= TryCollect;
        _growthTimer.TimerFinish -= AddPlant;
    }
    private void TryCollect(bool inTrigger = true)
    {
        if (inTrigger)
            CheckState();
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
                //EventManager.PlantHarvested?.Invoke();
                Harvest();

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
        _plant = Instantiate(_plantData.GetPlant(), _plantPoint);
        state = SeedbedState.Grown;
        _plantArea.gameObject.SetActive(true);
        _plantPoint.gameObject.SetActive(true);
    }

    private void Harvest()
    {
        //_plant.gameObject.SetActive(false);
        if (_plant != null)
            Destroy(_plant.gameObject);

        state = SeedbedState.Empty;
        PickUpItems.Instanse.AddItem(_plantData);
        EventManager.UpdateUIInventory?.Invoke();
        CheckState();
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
