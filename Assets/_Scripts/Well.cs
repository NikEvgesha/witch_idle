using UnityEngine;

public class Well : MonoBehaviour
{
    [SerializeField] private CheckPlayer _interactionArea;
    [SerializeField] private WellTimer _timer;
    //[SerializeField] private float _fillSpeed = 1f;
    [SerializeField] private int _fillTime = 5;
    [SerializeField] private InventoryItem _waterItem;
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
            EventManager.Item—ollect?.Invoke(_waterItem, this.transform);
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
