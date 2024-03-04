using UnityEngine;
using System;

public class ISAdQualityInitCallbackWrapper : MonoBehaviour
{
    private ISAdQualityInitCallback mCallback;

    public ISAdQualityInitCallback AdQualityInitCallback 
    {
        set 
        {
            mCallback = value;  
        }
        get 
        {
            return mCallback;
        }
    }

    void Awake ()
    {
        DontDestroyOnLoad(gameObject);                 //Makes the object not be destroyed automatically when loading a new scene.
    }

    public void adQualitySdkInitSuccess(string unityMsg) 
    {
        if (mCallback != null) 
        {
            mCallback.adQualitySdkInitSuccess();
        }
    }

    public void adQualitySdkInitFailed(string unityMsg) 
    {
        ISAdQualityInitError sdkInitError = ISAdQualityInitError.EXCEPTION_ON_INIT;
        string errorMsg = String.Empty;
        try
        {
            if (!String.IsNullOrEmpty(unityMsg)) 
            {
                string[] separators = { "Unity:" };
                string[] splitArray = unityMsg.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
                if (splitArray.Length > 1) 
                {
                    sdkInitError = (ISAdQualityInitError)Enum.Parse(typeof(ISAdQualityInitError), splitArray[0]);
                    errorMsg = splitArray[1];
                }
            }
        }
        catch (Exception e) 
        {
            errorMsg = e.Message;
        }
        if (mCallback != null) 
        {
            mCallback.adQualitySdkInitFailed(sdkInitError, errorMsg);
        }
    }
}