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
    private Slider _levelSlider;
    private void OnEnable()
    {
        EventManager.ExperienceChange += UpdateUI;
    }

    private void OnDisable()
    {
        EventManager.ExperienceChange -= UpdateUI;
    }
    private void UpdateUI(int experienceNow,int experienceNeed)
    {
        _levelExpNow.text = experienceNow.ToString();
        _levelExpNeed.text = experienceNeed.ToString();
        _levelSlider.value = (float)experienceNow / experienceNeed;
    }
}
