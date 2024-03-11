using UnityEngine;

public class SaveControl : MonoBehaviour
{
    [SerializeField] private bool _removeSaveOnStart;
    private string _money = "Money";
    private string _capacity = "Capacity";
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
        PlayerPrefs.Save();
    }
    public bool TryGetMoney()
    {
        return PlayerPrefs.HasKey(_money);
    }
    public int GetMoney()
    {
        return PlayerPrefs.GetInt(_money);
    }


    public void SaveCapacity(int capacity)
    {
        PlayerPrefs.SetInt(_capacity, capacity);
        PlayerPrefs.Save();
    }
    public bool TryGetCapacity()
    {
        return PlayerPrefs.HasKey(_capacity);
    }
    public int GetCapacity()
    {
        return PlayerPrefs.GetInt(_capacity);
    }
}
