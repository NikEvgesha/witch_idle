using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/NPS/ListTypes")]
public class NPSAllTypes : ScriptableObject
{
    [SerializeField] List<NPSType> _NPSTypes = new List<NPSType>();

    public NPSType GetRandomNPS()
    {
        int random = 0;
        random = Random.Range(0, _NPSTypes.Count);
        return _NPSTypes[random];
    }
}
