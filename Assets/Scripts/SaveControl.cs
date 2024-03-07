using UnityEngine;

public class SaveControl : MonoBehaviour
{

    private string _money = "Money";
    public static SaveControl Instanse;
    private void Awake()
    {
        Instanse = this;
    }
    public void SaveMoney(int money)
    {
        PlayerPrefs.SetInt(_money, money);
    }
    public bool TryGetMony()
    {
        return PlayerPrefs.HasKey(_money);
    }
    public int GetMoney()
    {
        return PlayerPrefs.GetInt(_money);
    }
}
