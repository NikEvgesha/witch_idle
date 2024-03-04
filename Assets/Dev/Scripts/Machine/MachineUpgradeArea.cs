using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineUpgradeArea : MonoBehaviour
{
    public Transform CameraLookTransform;

    public GameObject upgradeCanvas;



    public void OnOffUpgradeCanvas(bool isOn)
    {
        upgradeCanvas.SetActive(isOn);
    }

    public void PlayerOnArea()
    {
        OnOffUpgradeCanvas(true);
        EventManager.PlayerOnUpgrade(CameraLookTransform);
    }
    
    public void PlayerOffArea()
    {
        OnOffUpgradeCanvas(false);
        EventManager.PlayerOffUpgrade();
    }
    
    
}
