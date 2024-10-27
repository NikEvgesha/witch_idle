using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct storefront
{
    public Transform StorefrontPoint;
    public Transform LookPoint;
    public int Weight;
}
public class Store : MonoBehaviour
{
    public static Store Init;
    [SerializeField] private Storage _storage;
    [SerializeField] private List<storefront> _storefrontPoints;
    [SerializeField] private List<Transform> _queuePoints = new List<Transform>();
    [SerializeField] private List<NPSLogic> _nPSInStorefront;
    [SerializeField] private List<NPSLogic> _nPSInQueue;
    //[SerializeField] private int _nPSInStorefrontCount = 0;
    [SerializeField] private int _nPSInQueueCount = 0;
    [SerializeField] private GameObject _storeObject;
    [SerializeField] private SellZone _sellZone;
    [SerializeField] private int _storefrontCount;
    [SerializeField] private int _queueCount;
    private List<InventoryItem> _itemsInQueue = new List<InventoryItem>();
    [SerializeField] private List<storefront> _storefrontPointsDontUse = new List<storefront>();
    private void Awake()
    {
        Init = this;
        if(_sellZone != null)
        {
            _sellZone.FillDone += SellItemFirstInQueue;
        }
        foreach (storefront point in _storefrontPoints)
        {
            _storefrontPointsDontUse.Add(point);
        }
    }
    private void OnEnable()
    {
        _storage.StorageUpdate += UpdateStoreWarehouse;
    }
    private void Start()
    {
        _storefrontCount = _storefrontPoints.Count;
        _queueCount = _queuePoints.Count;
    }
    public bool Need—ustomers()
    {
        return (_nPSInStorefront.Count + _nPSInQueue.Count) < _queueCount && _nPSInStorefront.Count < _storefrontCount;
    }
    private bool HavePlaceInQueue()
    {
        return _nPSInQueue.Count < _queueCount;
    }
    public void New—ustomers(NPSLogic customer)
    {
        _nPSInStorefront.Add(customer);
        customer.SetSettingsNPS(GetStorefrontPoint(customer));
        //_nPSInStorefrontCount++;
    }
    private storefront GetStorefrontPoint(NPSLogic customer = null)
    {
        int coutWeight = 0;
        foreach (storefront point in _storefrontPointsDontUse)
        {
            coutWeight += point.Weight;
        }

        int randomWeight = Random.Range(0, coutWeight);
        coutWeight = 0;
        foreach (storefront point in _storefrontPointsDontUse)
        {
            coutWeight += point.Weight;
            if (coutWeight > randomWeight)
            {
                _storefrontPointsDontUse.Remove(point);
                return point;
            }
        }
        return new storefront();
    }
    private Transform GetQueuePoint(NPSLogic customer = null)
    {
        int queuePosition = _nPSInQueueCount;
        if (customer != null)
        {
            queuePosition = _nPSInQueue.FindIndex(x => x == customer);
        }
        return _queuePoints[queuePosition];
    }
    private Transform GetLookPoint(NPSLogic customer = null)
    {
        if (_nPSInQueueCount == 0)
        {
            return _storeObject.transform;
        }
        int queuePosition = _nPSInQueue.FindIndex(x => x == customer);
        return _queuePoints[queuePosition-1];
    }
    private void SetPlaceInQueue(NPSLogic customer)
    {
        customer.SetMovePoint(GetQueuePoint(customer));
        customer.SetLookPoint(GetLookPoint(customer));
    }
    private NPSLogic RemoveNPSOfQueue(NPSLogic customer = null)
    {
        if (customer == null)
        {
            customer = _nPSInQueue[0];
        }

        NPSSpawner.Instans.NPSGoHome(customer);

        _nPSInQueue.Remove(customer);
        _itemsInQueue.Remove(customer.GetInventoryItem());
        if (_nPSInQueue.Count <= 0) return customer;

        _sellZone.NewBuyer();

        foreach (NPSLogic _customer in _nPSInQueue)
        {
            SetPlaceInQueue(_customer);
        }
        return customer;
    }
    public void TakeItem(NPSLogic nPS)
    {
        if (!CheckQueue())
        {
            _sellZone.NewBuyer();
        }

        _nPSInStorefront.Remove(nPS);
        _nPSInQueue.Add(nPS);
        _storefrontPointsDontUse.Add(nPS.GetStorefront());
        SetPlaceInQueue(nPS);
        nPS.AddItem();
        StoreRemoveItem(nPS.GetInventoryItem());
    }
    /*public void NPSReachedPoint(NPSLogic nPS)
    {
        if (CheckItemInStoreWarehouse(nPS.GetInventoryItem()))
        {
            TakeItem(nPS);
        }
    }
    private bool CheckItemInStoreWarehouse(InventoryItem inventoryItem)
    {
        return true;
        //return inventoryItem != null;
    }
    public void UpdateStoreWarehouse()
    {
        foreach (NPSLogic customer in _nPSInStorefront)
        {
            if (customer._nPSStates == NPSStates.WaitingItemSale)
            {
                NPSReachedPoint(customer);
            }
        }
    }*/
    public void SellItem()
    {
        if (!CheckQueue()) return;
        //ÔÓ‰‡Ê‡
        NPSLogic removeCustomers = RemoveNPSOfQueue();
        WitchPlayerController.Instanse.Money += removeCustomers.GetMoney();
        WitchPlayerController.Instanse.Experience += removeCustomers.GetExp();


    }
    public bool CheckQueue()
    {
        return _nPSInQueue.Count > 0;
    }
    public bool CheckItemInStore(InventoryItem inventoryItem)
    {
        if (!HavePlaceInQueue())
        {
            return false;
        }
        if (!_storage.CheckItemInStorage(inventoryItem))
        {
            return false;
        }
        return true;
    }
    private void StoreRemoveItem(InventoryItem inventoryItem)
    {
        _itemsInQueue.Add(inventoryItem);
        _storage.TakeItemVault(inventoryItem);
    }
    private void SellItemFirstInQueue()
    {
        SellItem();
    }
    private void UpdateStoreWarehouse()
    {
        foreach (NPSLogic customer in _nPSInStorefront)
        {
            if (customer._nPSStates == NPSStates.WaitingItemSale)
            {
                if (CheckItemInStore(customer.GetInventoryItem()))
                {
                    customer.StoreItemUpdate();
                    return;
                }
            }
        }
    }
}
