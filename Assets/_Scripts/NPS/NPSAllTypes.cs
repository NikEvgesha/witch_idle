using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NPSTypes
{
    public NPSType NPSType;
    public int MinLevelUse;
}

[CreateAssetMenu(menuName = "WitchScripts/NPS/ListTypes")]

public class NPSAllTypes : ScriptableObject
{
    [SerializeField] List<NPSTypes> _nPSTypes = new List<NPSTypes>();

    public NPSType GetRandomNPS(int level = -1)
    {
        List<NPSTypes> useTypes = new List<NPSTypes>();
        if (level == -1)
        {
            useTypes = _nPSTypes;
        }
        else
        {
            foreach (NPSTypes nPSType in _nPSTypes)
            {
                if (nPSType.MinLevelUse <= level)
                {
                    useTypes.Add(nPSType);
                }
            }
        }
        int random = 0;
        random = Random.Range(0, useTypes.Count);
        return useTypes[random].NPSType;
    }
}
