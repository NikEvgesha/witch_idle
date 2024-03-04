using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera gameCamera;
    public CinemachineVirtualCamera upgradeCamera;
    public CinemachineVirtualCamera tutorialCamera;

    private void OnEnable()
    {
        EventManager.TutorialCameraSet += TutorialCameraSet;
        EventManager.PlayerOffUpgrade += PlayerOffUpgrade;
        EventManager.PlayerOnUpgrade += PlayerOnUpgrade;
    }

    private void TutorialCameraSet(Transform obj)
    {
        tutorialCamera.Priority = 20;
        tutorialCamera.LookAt = obj;
        tutorialCamera.Follow = obj;
        DOVirtual.DelayedCall(2, () =>
        {
            tutorialCamera.Priority = 0;

        });
    }

    private void PlayerOffUpgrade()
    {
        gameCamera.Priority = 10;
        upgradeCamera.Priority = 0;
    }

    private void OnDisable()
    {
        EventManager.TutorialCameraSet -= TutorialCameraSet;
        EventManager.PlayerOffUpgrade -= PlayerOffUpgrade;
        EventManager.PlayerOnUpgrade -= PlayerOnUpgrade;
    }

    private void PlayerOnUpgrade(Transform lookTransform)
    {
        gameCamera.Priority = 0;
        upgradeCamera.Priority = 10;
        upgradeCamera.LookAt = lookTransform;
        upgradeCamera.Follow = lookTransform;
    }
}