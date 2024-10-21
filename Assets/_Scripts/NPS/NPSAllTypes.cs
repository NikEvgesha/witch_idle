using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/NPS/ListTypes")]
public class NPSAllTypes : ScriptableObject
{
    [SerializeField] List<NPSType> _nPSTypes = new List<NPSType>();

    public NPSType GetRandomNPS()
    {
        int level = WitchPlayerController.Instanse.PlayerLevel;
        List<NPSType> useTypes = new List<NPSType>();
        
        foreach (NPSType nPSType in _nPSTypes)
        {
            if (nPSType.GetMinLevel() <= level)
            {
                useTypes.Add(nPSType);
            }
        }
        
        int random = 0;
        random = Random.Range(0, useTypes.Count);
        return useTypes[random];
    }
}
