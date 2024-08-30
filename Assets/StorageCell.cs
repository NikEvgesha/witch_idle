using UnityEngine;
using System;
public class StorageCell : MonoBehaviour
{
    private int _index;

    public void onCellClick()
    {
        if (this.GetComponentInChildren<ItemIcon>() != null)
        {
            EventManager.StorageCellClick?.Invoke(_index);
        }
 
    }

    public void SetIndex(int idx)
    {
        _index = idx;
    }
}
