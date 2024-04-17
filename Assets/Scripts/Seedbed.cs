
using System.Diagnostics;
using UnityEngine;
using YG;
using static UnityEditor.ObjectChangeEventStream;

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
        //_plantArea.PlayerOnPlantArea += CheckState;
        _plantArea.OnTrigger += TryCollect;
        _growthTimer.TimerFinish += AddPlant;
        //EventManager.PlantHarvested += Harvest; Зачем? мы же от отсюда этот эвент и отправляем
    }


    private void OnDisable()
    {
        _plantArea.OnTrigger -= TryCollect;
        _growthTimer.TimerFinish -= AddPlant;
    }
    private new void Start()
    {
        base.Start();
        UpdateYGSaveSistem(ref YandexGame.savesData.SBState);
        LoadSeedBed();
    }
    private void UpdateYGSaveSistem(ref int[] state)
    {
        if (state == null || state.Length == 0)
        {
            state = new int[YandexGame.savesData.IOName.Length];
        }
        else if (state.Length == YandexGame.savesData.IOName.Length) { return; }
        else {
            int[] newArrState = new int[YandexGame.savesData.IOName.Length];
            for (int i = 0; i < state.Length; i++)
            {
                newArrState[i] = state[i];
            }
            state = newArrState;
        }
    }
    private void LoadSeedBed()
    {
        switch (YandexGame.savesData.SBState[GetIndex()])
        {
            case 0:
                state = SeedbedState.Empty;
                break;
            case 1:
                state = SeedbedState.Growing;
                break;
            case 2:
                state = SeedbedState.Grown;
                break;
            default:
                state = SeedbedState.Empty;
                break;
        }
        CheckStateLoad();
        //_plantArea.gameObject.SetActive(true);

    }
    private void SaveSeedBed()
    {
        switch (state)
        {
            case SeedbedState.Empty:
                YandexGame.savesData.SBState[GetIndex()] = 0;
                return;
            case SeedbedState.Growing:
                YandexGame.savesData.SBState[GetIndex()] = 1;
                return;
            case SeedbedState.Grown:
                YandexGame.savesData.SBState[GetIndex()] = 2;
                return;
            default:
                YandexGame.savesData.SBState[GetIndex()] = 0;
                return;
        }
        //CheckStateLoad();
        //_plantArea.gameObject.SetActive(true);

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

    private void CheckStateLoad()
    {
        if (state == SeedbedState.Growing) { PlantSeeds(); }
        else if (state == SeedbedState.Grown) { AddPlant(); }
    }

    private void PlantSeeds() {
        state = SeedbedState.Growing;
        _growthTimer.gameObject.SetActive(true);
        _growthTimer.StartGrowthTimer(_plantData.GetGrowthTime());
        _plantArea.gameObject.SetActive(false);
        SaveSeedBed();
    }

    private void AddPlant() {
        _growthTimer.gameObject.SetActive(false);
        _plant = Instantiate(_plantData.GetPlant(), _plantPoint);
        state = SeedbedState.Grown;
        _plantArea.gameObject.SetActive(true);
        _plantPoint.gameObject.SetActive(true);
        SaveSeedBed();
    }

    private void Harvest()
    {
        if (!Inventory.Instanse.AddItem(_plantData))
        {
            return;
        }
        //_plant.gameObject.SetActive(false);
        if (_plant != null)
            Destroy(_plant.gameObject);

        state = SeedbedState.Empty;
        CheckState();
        SaveSeedBed();
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
