using System.IO;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class GameMenu : EditorWindow
{
    [MenuItem("WitchMenu/Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    [MenuItem("WitchMenu/Take Screenshot")]
    public static void TakeScreenshot()
    {
        string path = "Screenshots";

        Directory.CreateDirectory(path);

        int i = 0;
        while (File.Exists(path + "/" + i + ".png"))
        {
            i++;
        }

        ScreenCapture.CaptureScreenshot(path + "/" + i + ".png");
    }
}
#endif