using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StorageGrid : MonoBehaviour
{
    [SerializeField] private StorageCell _cellPrefab;
    private int _width = 1200;
    private int _height = 800;
    private int _cellSize = 150;
    private int _cellsAmount = 24;

    private List<StorageCell> _cells;

    public void InitGrid()
    {
        _cells = new List<StorageCell>();
        while (_cells.Count < _cellsAmount)
        {
            StorageCell cell = Instantiate(_cellPrefab, transform);
            _cells.Add(cell);
            cell.SetIndex(_cells.Count - 1);
        }
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
