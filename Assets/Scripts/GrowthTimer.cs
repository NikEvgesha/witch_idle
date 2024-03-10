using System.Collections;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GrowthTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Image _timerFiller;

    public event Action TimerFinish;

    private float _timeLeft = 0f;
    private bool _isRunning;

    private IEnumerator StartTimer()
    {
        _isRunning = true;
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
        _isRunning = false;
        TimerFinish?.Invoke();
        this.gameObject.SetActive(false);
    }

    public void StartGrowthTimer(float time)
    {
        this.gameObject.SetActive(true);
        _timeLeft = time;
        StartCoroutine(StartTimer());
    }

    private void UpdateTimeText()
    {
        if (_timeLeft < 0)
            _timeLeft = 0;

        float minutes = Mathf.FloorToInt(_timeLeft / 60);
        float seconds = Mathf.FloorToInt(_timeLeft % 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds+1);
    }


    public bool IsRunning()
    {
        return _isRunning;
    }
}
