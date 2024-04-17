using System.Collections.Generic;
using UnityEngine;

public class UniquenessСheck : MonoBehaviour
{
    [SerializeField]
    private List<string> _names = new List<string>();
    [SerializeField]
    private List<InteractionObject> _interactionObjects = new List<InteractionObject>();

    void Start()
    {
        CheckInteractionObjects(FindObjectsOfType<InteractionObject>());
    }
    private void CheckInteractionObjects(InteractionObject[] io)
    {
        if (io == null || io.Length == 0) return;
        int count = 0;
        foreach (InteractionObject o in io)
        {
            if (_names.Contains(o.GetName()))
            {
                continue;
            }
            foreach (InteractionObject o2 in io)
            {
                if (o2.GetName() == o.GetName())
                {
                    count++;
                    if (count > 2)
                    {
                        _interactionObjects.Add(o2);
                    }
                }
            }
            if (count > 1)
            {
                _names.Add(o.GetName());
            }
            count = 0;
        }        
    }
}
