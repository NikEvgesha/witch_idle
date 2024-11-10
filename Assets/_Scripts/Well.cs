using UnityEngine;

public class Well : MonoBehaviour
{
    [SerializeField] private CheckPlayer _interactionArea;
    [SerializeField] private HarvestTimer _timer;
    //[SerializeField] private float _fillSpeed = 1f;
    [SerializeField] private int _harvestTime = 5;
    [SerializeField] private InventoryItem _waterItem;
    [SerializeField] private ItemCollector _itemCollector;
    private bool _inInteractionArea;


    private void Start()
    {
        _timer.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        _timer.TimerFinish += tryCollectItem;
        _interactionArea.OnTrigger += onInteractionAreaEnter;
    }

    private void OnDisable()
    {
        _timer.TimerFinish -= tryCollectItem;
        _interactionArea.OnTrigger -= onInteractionAreaEnter;
    }


    private void tryCollectItem()
    {
        _timer.gameObject.SetActive(false);
        if (Inventory.Instanse.AddItem(_waterItem))
        {
            _itemCollector.ItemCollect(_waterItem, this.transform, true);
            TryHarvest();
        }
       
    }


    private void onInteractionAreaEnter(bool inTrigger)
    {
        _inInteractionArea = inTrigger;
        TryHarvest();
    }

    private void TryHarvest()
    {
        if (Inventory.Instanse.HaveEmptySlot())
        {
            _timer.gameObject.SetActive(_inInteractionArea);
            if (_inInteractionArea)
            {
                _timer.StartWellTimer(_harvestTime);
            }
        } else
        {
            // No space message
        }
        
    }

}
