using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: Add support for different banner sizes
public class BannerWhiteSpaceHandler : MonoBehaviour
{
    private static BannerWhiteSpaceHandler instance;

    public Canvas myCanvas;
    public RectTransform whiteSpaceTransform;
    public float marginPoints = 2;
    private Rect lastSafeArea;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateSafeArea();
    }

    private void Update()
    {
        if (lastSafeArea != Screen.safeArea)
        {
            UpdateSafeArea();
        }
    }

    private void UpdateSafeArea()
    {
        var safeRect = Screen.safeArea;
        float bannerHeight = safeRect.position.y / myCanvas.scaleFactor + marginPoints + 22.5f;
        whiteSpaceTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bannerHeight);
        lastSafeArea = safeRect;
    }

    public static void SetWhiteSpaceActive(bool value)
    {
        if (instance == null) return;
        
        instance.whiteSpaceTransform.gameObject.SetActive(value);
    }

    public static Rect GetBannerSafeArea(Canvas canvas)
    {
        if (instance == null)
        {
            return Screen.safeArea;
        }
        else
        {
            Rect safeRect = Screen.safeArea;
            float scaleFactor = canvas.scaleFactor;
            return new Rect(new Vector2(safeRect.x / scaleFactor, (safeRect.y + instance.marginPoints + 22.5f) / scaleFactor),
                new Vector2(safeRect.width / scaleFactor, safeRect.y / scaleFactor));
        }
    }
}
