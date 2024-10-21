using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct LevelStep
{
    public int Level;
    public int Experience;
}

[CreateAssetMenu(menuName = "WitchScripts/LevelSteps")]
public class LevelSteps : ScriptableObject
{
    [SerializeField]
    private List<LevelStep> _levelSteps;
    [SerializeField]
    private float _multiplyMaxLevelExp = 1;

    public int GetExperience(int currentLevel)
    {
        int lastExperience = 0;
        int lastLevel = 0;
        foreach (LevelStep item in _levelSteps)
        {
            lastExperience = item.Experience;
            lastLevel = item.Level;
            if (item.Level == currentLevel) return item.Experience;
        }
        return (int)(lastExperience * (currentLevel - lastLevel + 1) * _multiplyMaxLevelExp);
    }


}
