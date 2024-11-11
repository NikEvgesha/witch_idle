using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPSLogic : MonoBehaviour
{
    /*
    Спавн
    место куда идти
    */

    [SerializeField] private NavMeshAgent _meshAgent;
    [SerializeField] private NPSType _nPSType;
    [SerializeField] private NPSAllTypes _nPSAllTypes;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _distanceToStop;
    [SerializeField] private GameObject _skin;
    [SerializeField] private Transform _skinSpawnPoint;
    [SerializeField] private Transform _itemIconPosition;
    //public GameObject moneyPrefab;

    //public Renderer renderer; // Переделать на случайный скин?

    private storefront _storefront;
    [SerializeField] private InventoryItem _needItem;
    private ItemIcon _itemIcon;
    private bool _haveItem = false;
    private Transform _customerLookAt;
    private Transform _customerMovePoint;

    [HideInInspector] public NPSStates _nPSStates = new NPSStates();

    public void SetSettingsNPS(storefront storefront, List<PotionTypes> dontUsePotion)
    {
        _storefront = storefront;
        SetMovePoint(_storefront.StorefrontPoint);
        SetLookPoint(_storefront.LookPoint);
        _nPSStates = NPSStates.WalkingToStore; 
        StartSetting();
        _needItem = _nPSType.SelectPotion(dontUsePotion);
        _itemIcon = Instantiate(_needItem.GetIcon(),_itemIconPosition);
        
    }
    public void StartSetting()
    {
        _nPSType = _nPSAllTypes.GetRandomNPS();
        SetSkin();
    }
    private void SetSkin()
    {
        _skin = Instantiate(_nPSType.GetSkin(), _skinSpawnPoint);
        _animator = _skin.GetComponent<Animator>();
    }
    public void SetMovePoint(Transform point)
    {
        _customerMovePoint = point;
        MoveTo(_customerMovePoint);
    }
    public void SetLookPoint(Transform point)
    {
        _customerLookAt = point;
    }
    public InventoryItem GetInventoryItem()
    {
        return _needItem;
    }
    public void MoveTo(Transform target)
    {
        _meshAgent.enabled = true;
        //animator.SetBool("Idle", false);
        //_meshAgent.enabled = true;
        _meshAgent.destination = target.position;
        //state = CustomerStates.WalkingToMachine;
    }
    void Update()
    {
        _animator.SetFloat("Movement", _meshAgent.velocity.magnitude);
        switch (_nPSStates)
        {
            case NPSStates.WalkingToStore:

                // Проверка на то, достаточно ли близок агент к цели
                if (_meshAgent.remainingDistance < _distanceToStop)
                {
                    // Смена точки на следующую
                    //_meshAgent.enabled = false;
                    gameObject.transform.LookAt(_customerLookAt);
                    _nPSStates = NPSStates.WaitingItemSale;
                    StoreItemUpdate();
                }
                break;

            //case NPSStates.WaitingItemSale:
                ////gameObject.transform.LookAt(_customerLookAt);
               // break;

            case NPSStates.WalkingToQueue:

                // Проверка на то, достаточно ли близок агент к цели
                if (_meshAgent.remainingDistance < _distanceToStop)
                {
                    // Смена точки на следующую
                    //_meshAgent.enabled = false;
                    gameObject.transform.LookAt(_customerLookAt);
                    _nPSStates = NPSStates.WaitingInQueue;
                }
                break;

            ///case NPSStates.WaitingInQueue:
              //  gameObject.transform.LookAt(_customerLookAt);
              //  break;

            case NPSStates.WalkingHome:

                // Проверка на то, достаточно ли близок агент к цели
                if (_meshAgent.remainingDistance < _distanceToStop)
                {
                    //_meshAgent.enabled = false;
                    //gameObject.SetActive(false);
                    NPSSpawner.Instans.NPSGone(this);
                }
                break;

            default:

                break;
        }

       
    }
    private void BuyItem()
    {
        Store.Init.TakeItem(this);
        _nPSStates = NPSStates.WalkingToQueue;
    }
    public void AddItem()
    {
        _itemIcon.ChangeIcon();
        _haveItem = true;
    }
    public void StoreItemUpdate()
    {
        if (Store.Init.CheckItemInStore(_needItem))
        {
            BuyItem();

        }
    }
    public void GoHome(Transform point)
    {
        if (_haveItem)
        {
            Destroy(_itemIcon.gameObject);
        }
        _nPSStates = NPSStates.WalkingHome;
        SetMovePoint(point);
    }
    public int GetMoney() 
    {
        return _needItem.GetPrice();
    }
    public int GetExp()
    {
        return _needItem.GetExperience();
    }
    public storefront GetStorefront()
    {
        return _storefront;
    }
}

