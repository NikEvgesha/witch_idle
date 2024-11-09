using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    private Transform _source;
    private Transform _target;
    private GameObject _itemObj;
    private float _distance;
    private float _minDist = 1f;
    private float _lerpPoint = 0;
    [SerializeField] private Color _color;

    public void ItemCollect(InventoryItem item, Transform obj, bool toPlayer)
    {
        if (toPlayer)
        {
            _source = obj.transform;
            _target = Inventory.Instanse.gameObject.transform;
        } else
        {
            _source = Inventory.Instanse.gameObject.transform;
            _target = obj.transform;
        }
        GameObject itemPrefab = item.GetItemPrefab();
        _itemObj = Instantiate(itemPrefab, _source);
    }

    private void FixedUpdate()
    {
        if (_itemObj != null)
        {
            _lerpPoint += Time.fixedDeltaTime * _moveSpeed;
            _itemObj.transform.position = Vector3.Lerp(_itemObj.transform.position, _target.position, _lerpPoint);
            //_distance = (_target.position - _itemObj.transform.position).magnitude;
            if (_lerpPoint >= 1)
            {
                Destroy(_itemObj);
                _itemObj = null;
                _source = null;
                _target = null;
                _distance = 0;
                _lerpPoint = 0;
            }
        }
    }

}
