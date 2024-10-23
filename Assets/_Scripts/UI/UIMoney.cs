using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMoney : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyUI;
    [SerializeField] private UIMoneyChangeAnimation _moneyChangeAnimationPrefab;
    private int _money;
    private int _changeAmount;

    private void Start()
    {
    }

    private void OnEnable()
    {
        EventManager.MoneyChange += UpdateUI;
    }

    private void OnDisable()
    {
        EventManager.MoneyChange -= UpdateUI;
    }
    private void UpdateUI(int newMoney)
    {
        _changeAmount = newMoney - _money;
        if (_changeAmount != 0)
        {
            string text = (_changeAmount > 0) ? "+" + _changeAmount.ToString() : _changeAmount.ToString();
            UIMoneyChangeAnimation animation = Instantiate(_moneyChangeAnimationPrefab, this.transform);
            animation.Config(text, _changeAmount > 0);

            _moneyUI.text = "$ " + newMoney.ToString();
            _money = newMoney;
        }
        
    }

    public void UseAnimator()
    {

    }



}
