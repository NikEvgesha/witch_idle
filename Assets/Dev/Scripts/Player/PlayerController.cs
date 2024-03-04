using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int money;
    public GameObject moneyTextPrefab;
    public Transform moneyCanvas;
    public GameObject maxText;
    private void Start()
    {
        money = EventManager.GetGameData().totalMoneyAmount;
    }

    private void OnEnable()
    {
        EventManager.MoneyUpdated += MoneyUpdated;
        EventManager.PlayerCollectedMoney += PlayerCollectedMoney;
    }

    private void MoneyUpdated()
    {
        money = EventManager.GetGameData().totalMoneyAmount;
    }

    private void OnDisable()
    {
        EventManager.MoneyUpdated -= MoneyUpdated;
        EventManager.PlayerCollectedMoney -= PlayerCollectedMoney;
    }

    public void StackIsMax(bool activate)
    {
        maxText.SetActive(activate);
    }

    private void PlayerCollectedMoney(float amount)
    {
        var money = Instantiate(moneyTextPrefab, Vector3.zero, quaternion.identity, moneyCanvas);
        money.transform.localPosition = Vector3.zero;
        money.GetComponent<TextMeshProUGUI>().text = "$" + amount;
        money.transform.DOLocalMoveY(money.transform.localPosition.y + 2, .5f).OnComplete(() =>
        {
            Destroy(money.gameObject);
        });
    }
}
