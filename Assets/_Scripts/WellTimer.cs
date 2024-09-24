using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WellTimer : MonoBehaviour
{
    public event Action TimerFinish;

    [SerializeField] private Image _fillImage;
    private float _timeLeft;
    private float _timeFull;
    private bool _isRunning;


    public void StartWellTimer(int time/*, float speed*/)
    {
        _timeLeft = time;
        _timeFull = time;
        _fillImage.fillAmount = 0;
        StartCoroutine(StartTimer());
    }


    private IEnumerator StartTimer()
    {
        _isRunning = true;
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft < 0)
                _timeLeft = 0;
            UpdateFilling();
            yield return null;
        }
        _isRunning = false;
        TimerFinish?.Invoke();
    }

    private void UpdateFilling()
    {
        _fillImage.fillAmount = (_timeFull - _timeLeft) / _timeFull;
    }
}
