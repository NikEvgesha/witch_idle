using UnityEngine;
using YG;

public class ToYG : MonoBehaviour
{
    public YandexGame YG;
    public static ToYG Instanse;
    private void Awake()
    {
        Instanse = this;
    }
    private void OnApplicationQuit()
    {
        YandexGame.SaveProgress();
    }
}
