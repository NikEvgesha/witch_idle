using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    private bool _isMoving = false;

    private void OnEnable()
    {
        EventManager.ItemÑollect += ItemCollect;
    }

    private void ItemCollect(InventoryItem item, Transform source)
    {
        if (item == null || source.gameObject == this.gameObject) return;
        GameObject prefab = item.GetItemPrefab();
        var itemObj = Instantiate(prefab, source);
        StartCoroutine(MoveCoroutine(itemObj));
    }

    IEnumerator MoveCoroutine(GameObject itemObj)
    {
        Vector3 moveTo = this.transform.position;
        _isMoving = true;
        var iniPosition = itemObj.transform.position;
        while ((itemObj.transform.position - moveTo).magnitude > 0.5f)
        {
            moveTo = this.transform.position;
            Vector3 newPosition = moveTo - itemObj.transform.position;
            itemObj.transform.Translate(newPosition.normalized * Time.deltaTime * _moveSpeed);
            yield return new WaitForFixedUpdate();
        }

        _isMoving = false;
       Destroy(itemObj);
    }
}
