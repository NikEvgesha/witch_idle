using UnityEngine;
using TMPro;

public class UIMoneyChangeAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] TextMeshProUGUI _text;
    public void Config(string text, bool isPositive)
    {
        _animator.SetTrigger(isPositive ? "Add" : "Remove");
        _text.text = text;

    }

    private void OnDisable()
    {
       Destroy(this.gameObject);
    }
}
