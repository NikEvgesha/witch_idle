using System;
using UnityEngine;

public class CheckPlayer : MonoBehaviour
{
    public Action<bool> OnTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WitchPlayerController>())
            OnTrigger?.Invoke(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<WitchPlayerController>())
            OnTrigger?.Invoke(false);
    }
}
