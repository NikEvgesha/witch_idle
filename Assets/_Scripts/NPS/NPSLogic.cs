using UnityEngine;
using UnityEngine.AI;

public class NPSLogic : MonoBehaviour
{
    /*
    Спавн
    место куда идти
    */

    [SerializeField] private NavMeshAgent _meshAgent;
    [SerializeField] private NPSType _NPSType;
    [SerializeField] private storefront _storefront;
    //public NPSState state;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _distanceToStop;

    [SerializeField] private float _testSpeed;


    public NPSStates _nPSStates = new NPSStates();
    //public GameObject moneyPrefab;
    //public List<GameObject> dumbells;
    //public List<GameObject> inclineDumbells;

    //public GameObject benchBar;
    //public CustomerWaterTowelUI waterTowelUI;
    //public CustomerNeed customerNeed;
    //[HideInInspector] public Vector3 workoutOverPosition;
    //public Renderer renderer; // Переделать на случайный скин?
    //public GameObject waterAnimation;
    //public GameObject towelAnimation;
    //public GameObject deadLiftBar;
    private InventoryItem _needItem;
    private bool _haveItem = false;
    private Transform _customerLookAt;
    private Transform _customerMovePoint;

    public void SetSettingsNPS(Transform pointMove, Transform pointLook, int LevelSells)
    {
        SetMovePoint(pointMove);
        SetLookPoint(pointLook);
        SetlevelSells(LevelSells);
    }
    public void SetSettingsNPS(storefront storefront, int LevelSells)
    {
        _storefront = storefront;
        SetMovePoint(storefront.StorefrontPoint);
        SetLookPoint(storefront.LookPoint);
        SetlevelSells(LevelSells);
        _nPSStates = NPSStates.WalkingToStore;
    }

    public void SetMovePoint(Transform point)
    {
        _customerMovePoint = point;
        MoveTo(point);
    }
    public void SetLookPoint(Transform point)
    {
        _customerLookAt = point;
    }
    public void SetlevelSells(int level)
    {

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
        _testSpeed = _meshAgent.velocity.magnitude;
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
        _nPSStates = NPSStates.WalkingHome;
        SetMovePoint(point);
    }
}

