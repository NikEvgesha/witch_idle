using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _levelExpNow;
    [SerializeField]
    private TextMeshProUGUI _levelExpNeed;
    [SerializeField]
    private TextMeshProUGUI _levelNum;
    [SerializeField]
    private Slider _levelSlider;
    private void OnEnable()
    {
        EventManager.ExperienceChange += UpdateUI;
    }

    private void OnDisable()
    {
        EventManager.ExperienceChange -= UpdateUI;
    }
    private void UpdateUI(int experienceNow,int experienceNeed,int level)
    {
        _levelExpNow.text = experienceNow.ToString();
        _levelExpNeed.text = experienceNeed.ToString();
        _levelNum.text = level.ToString();
        _levelSlider.value = (float)experienceNow / experienceNeed;
    }
}
