using System.Collections.Generic;
using UnityEngine;

public class StorageGrid : MonoBehaviour
{
    [SerializeField] private StorageCell _cellPrefab;
    private int _cellsAmount;
    private List<StorageCell> _cells;
    private string _name;
    public void InitGrid(string name, int cellsAmount)
    {
        _name = name;
        _cellsAmount = cellsAmount;
        _cells = new List<StorageCell>();
        while (_cells.Count < cellsAmount)
        {
            StorageCell cell = Instantiate(_cellPrefab, transform);
            _cells.Add(cell);
            cell.SetIndex(_cells.Count - 1);
        }
    }

    public string GetName()
    {
        return _name;
    }

    public void setIcon(ItemIcon icon, int idx)
    {
        if (idx >= 0 && idx < _cellsAmount)
        {
            Instantiate(icon, _cells[idx].transform);
        }
    }

    public void ClearCell(int idx)
    {
        foreach (Transform child in _cells[idx].transform)
        {
            Destroy(child.gameObject);
        }
    }

}
