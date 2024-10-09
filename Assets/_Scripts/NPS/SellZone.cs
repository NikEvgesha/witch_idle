using System;
using UnityEngine;
using UnityEngine.UI;

public class SellZone : MonoBehaviour
{
    [SerializeField] private float _speedFill = 1;
    [SerializeField] private float _fullFill = 2;
    [SerializeField] private Image _sellTimeImage;
    [SerializeField] private GameObject _uIObject;
    [SerializeField] private CheckPlayer _sellArea;
    [SerializeField] private bool _haveBuyer = false;
    [SerializeField] private float _fill = 0.001F;
    private bool _characterInZone;
    public Action FillDone;
    protected void OnEnable()
    {
        _sellArea.OnTrigger += TrySell;
    }
    private void OnDisable()
    {
        _sellArea.OnTrigger -= TrySell;
    }
    private void FixedUpdate()
    {   
        if (_characterInZone && _haveBuyer)
        {
            _fill += _speedFill * Time.fixedDeltaTime;
            if (_fill > _fullFill)
            {
                _fill = _fullFill;
                Done();
            }
            ChangeUIPrice();
        }
    }
    private void ChangeUIPrice()
    {
        if (_sellTimeImage != null)
        {
            _sellTimeImage.fillAmount = _fill / _fullFill;
        }
    }
    private void Done()
    {
        _haveBuyer = false;
        _fill = 0.001F;
        TrySell(false);
        _uIObject.SetActive(false);
        FillDone?.Invoke();
    }
    public void NewBuyer()
    {
        _uIObject.SetActive(true);
        _haveBuyer = true;
    }
    private void TrySell(bool inTrigger)
    {
        if (!inTrigger)
        {
            _fill = 0.001F;
            ChangeUIPrice();
        }
        _characterInZone = inTrigger;
    }
}
