using UnityEngine;

public class SaveControl : MonoBehaviour
{
    [SerializeField] private bool _removeSaveOnStart;
    private string _money = "Money";
    public static SaveControl Instanse;
    private void Awake()
    {
        if (_removeSaveOnStart)
        {
            PlayerPrefs.DeleteAll();
        }
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
