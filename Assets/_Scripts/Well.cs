using UnityEngine;

public class Well : MonoBehaviour
{
    [SerializeField] private CheckPlayer _interactionArea;
    [SerializeField] private HarvestTimer _timer;
    //[SerializeField] private float _fillSpeed = 1f;
    [SerializeField] private int _fillTime = 5;
    [SerializeField] private InventoryItem _waterItem;
    [SerializeField] private ItemCollector _itemCollector;
    //private bool _inInteractionArea;


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
        if (Inventory.Instanse.AddItem(_waterItem))
        {
             _timer.gameObject.SetActive(false);
            _itemCollector.ItemCollect(_waterItem, this.transform, true);
        }
       
    }


    private void onInteractionAreaEnter(bool inTrigger)
    {
        //_inInteractionArea = inTrigger;
        _timer.gameObject.SetActive(inTrigger);
        if (inTrigger)
        {
            _timer.StartWellTimer(_fillTime);
        }
        
    }

}
