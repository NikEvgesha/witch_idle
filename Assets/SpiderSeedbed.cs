using UnityEngine;

public class SpiderSeedbed : Seedbed
{
    [SerializeField] private GameObject _spiderWinner;
    [SerializeField] private GameObject _spiderCollect;

    private Animator _spiderWinnerAnimator;
    private Animator _spiderCollectAnimator;

    private new void Start()
    {
        base.Start();
        _spiderCollectAnimator = _spiderCollect.GetComponent<Animator>();
        _spiderWinnerAnimator = _spiderWinner.GetComponent<Animator>();
    }

    private new void OnEnable()
    {
        base.OnEnable();
        _growthTimer.TimerFinish += EndFight;
        _timer.TimerFinish += ResetFight;
    }

    private new void OnDisable()
    {
        base.OnDisable();
        _growthTimer.TimerFinish += EndFight;
        _timer.TimerFinish += ResetFight;
    }

    private void EndFight()
    {
        Debug.Log("END FIGHT");
        _spiderCollectAnimator.SetBool("isFighting", false);
        _spiderWinnerAnimator.SetBool("isFighting", false);
        _spiderWinnerAnimator.SetTrigger("Win");
        _spiderCollectAnimator.SetTrigger("Death");
    }

    private void ResetFight()
    {
        Debug.Log("RESET FIGHT");
        _spiderWinnerAnimator.SetTrigger("Fight");
        _spiderCollectAnimator.SetTrigger("Fight");
        _spiderCollectAnimator.SetBool("isFighting", true);
        _spiderWinnerAnimator.SetBool("isFighting", true);
    }
}
