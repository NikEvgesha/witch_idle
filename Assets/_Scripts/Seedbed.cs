using UnityEngine;
using YG;

public class Seedbed :  InteractionObject
{
    //[SerializeField] private PlantArea _plantArea;
    [SerializeField] CheckPlayer _plantArea;
    [SerializeField] private GrowthTimer _growthTimer;
    //[SerializeField] private Plant _plant;
    [SerializeField] private SeedbedState state;
    [SerializeField] private Transform _plantPoint;
    [SerializeField] private HarvestTimer _timer;
    [SerializeField] private int _fillTime = 1;
    [SerializeField] private Transform _iconSign;

    /* То, что отличается у разных грядок */
    [SerializeField] private PlantsData _plantData;
    [SerializeField] private ItemCollector _itemCollector;
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
        _plantArea.OnTrigger += CheckState;
        _growthTimer.TimerFinish += AddPlant;
        _timer.TimerFinish += Harvest;
    }


    private void OnDisable()
    {
        _plantArea.OnTrigger -= CheckState;
        _growthTimer.TimerFinish -= AddPlant;
        _timer.TimerFinish -= Harvest;
    }
    private new void Start()
    {
        base.Start();
        UpdateYGSaveSistem(ref YandexGame.savesData.SBState);
        LoadSeedBed();
        var icon = Instantiate(_plantData.GetItem().GetIcon(), _iconSign);
        
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
            case 1:
                state = SeedbedState.Empty;
                break;
            case 2:
                state = SeedbedState.Growing;
                break;
            case 3:
                state = SeedbedState.Grown;
                break;
            default:
                //state = SeedbedState.Empty;
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
                YandexGame.savesData.SBState[GetIndex()] = 1;
                return;
            case SeedbedState.Growing:
                YandexGame.savesData.SBState[GetIndex()] = 2;
                return;
            case SeedbedState.Grown:
                YandexGame.savesData.SBState[GetIndex()] = 3;
                return;
            default:
                YandexGame.savesData.SBState[GetIndex()] = 1;
                return;
        }
        //CheckStateLoad();
        //_plantArea.gameObject.SetActive(true);

    }
    private void TryCollect(bool inTrigger = true)
    {
        _timer.gameObject.SetActive(inTrigger);
        if (inTrigger && Inventory.Instanse.HaveEmptySlot())
        {
            _timer.StartWellTimer(_fillTime);
        } else
        {
            // анимация отсутствия места в инвентаре
        }
        
        /*if (inTrigger)
            CheckState();*/
    }

    private new void CheckState(bool inTrigger) {
        base.CheckState();
        switch (state)
        {
            case SeedbedState.Empty:
                PlantSeeds();
                break;
            case SeedbedState.Growing:
                break;
            case SeedbedState.Grown:
                TryCollect(inTrigger);
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
        if (!Inventory.Instanse.AddItem(_plantData.GetItem(),true))
        {
            return;
        }
        if (_plant != null)
            Destroy(_plant.gameObject);
        _itemCollector.ItemCollect(_plantData.GetItem(), this.transform, true);

        state = SeedbedState.Empty;
        _timer.gameObject.SetActive(false);
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
