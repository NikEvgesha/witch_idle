using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct storefront
{
    public Transform StorefrontPoint;
    public Transform LookPoint;
    public int Weight;
    public int WeightStatic;
}
public class Store : MonoBehaviour
{
    public static Store Init;
    [SerializeField] private List<storefront> _storefrontPoints;
    [SerializeField] private List<Transform> _queuePoints = new List<Transform>();
    [SerializeField] private List<NPSLogic> _nPSInStorefront;
    [SerializeField] private List<NPSLogic> _nPSInQueue;
    [SerializeField] private int _nPSInStorefrontCount = 0;
    [SerializeField] private int _nPSInQueueCount = 0;
    [SerializeField] private GameObject _storeObject;
    [SerializeField] private SellZone _sellZone;
    private int _levelSells = 1;
    [SerializeField] private int _storefrontCount;
    [SerializeField] private int _queueCount;
    private void Awake()
    {
        Init = this;
        if(_sellZone != null)
        {
            _sellZone.FillDone += SellItemFirstInQueue;
        }
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
        customer.SetSettingsNPS(GetStorefrontPoint(customer), _levelSells);
        //_nPSInStorefrontCount++;
    }
    private storefront GetStorefrontPoint(NPSLogic customer = null)
    {
        int coutWeight = 0;
        storefront storefrontPoint;
        foreach (storefront point in _storefrontPoints)
        {
            if (point.Weight == 0)
            {
                continue;
            }
            coutWeight += point.Weight;
        }

        if (coutWeight == 0) return new storefront();

        int randomWeight = Random.Range(0, coutWeight);
        coutWeight = 0;

        foreach (storefront point in _storefrontPoints)
        {
            if (point.Weight == 0)
            {
                continue;
            }
            coutWeight += point.Weight;

            if (coutWeight > randomWeight)
            {
                storefrontPoint = point;
                storefrontPoint.Weight = 0;
                return storefrontPoint;
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
    private void NewLevelSells(int level)
    {
        _levelSells = level;
    }
    private void RemoveNPSOfQueue(NPSLogic customer = null)
    {
        if (customer == null)
        {
            customer = _nPSInQueue[0];
        }

        NPSSpawner.Instans.NPSGoHome(customer);

        _nPSInQueue.Remove(customer);

        if (_nPSInQueue.Count <= 0) return;

        _sellZone.NewBuyer();

        foreach (NPSLogic _customer in _nPSInQueue)
        {
            SetPlaceInQueue(_customer);
        }
    }
    public void TakeItem(NPSLogic nPS)
    {
        if (!CheckQueue())
        {
            _sellZone.NewBuyer();
        }
        _nPSInStorefront.Remove(nPS);
        _nPSInQueue.Add(nPS);
        SetPlaceInQueue(nPS);
        nPS.AddItem();
        StoreRemoveItem(nPS.GetInventoryItem());
    }
    public void NPSReachedPoint(NPSLogic nPS)
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
    }
    public void SellItem()
    {
        if (!CheckQueue()) return;
        //ÔÓ‰‡Ê‡
        RemoveNPSOfQueue();
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
        return true;
    }
    private void StoreRemoveItem(InventoryItem inventoryItem)
    {

    }
    private void SellItemFirstInQueue()
    {
        SellItem();
    }

}
