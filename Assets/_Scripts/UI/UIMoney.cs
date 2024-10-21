using TMPro;
using UnityEngine;

public class UIMoney : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _moneyUI;
    private void OnEnable()
    {
        EventManager.MoneyChange += UpdateUI;
    }

    private void OnDisable()
    {
        EventManager.MoneyChange -= UpdateUI;
    }
    private void UpdateUI(int money)
    {
        _moneyUI.text = "$ " + money.ToString();
    }
}
