using System.Collections;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    private bool _isMoving = false;

    private void OnEnable()
    {
        EventManager.Item—ollect += ItemCollect;
    }

    private void ItemCollect(InventoryItem item, Transform source)
    {
        if (item == null || source.gameObject == this.gameObject) return;
        GameObject prefab = item.GetItemPrefab();
        var itemObj = Instantiate(prefab, source);
        itemObj.transform.Translate(source.position + new Vector3(0, 5, 0));
        //StartCoroutine(MoveCoroutine(itemObj, this.transform.position));
    }

    IEnumerator MoveCoroutine(GameObject itemObj, Vector3 moveTo)
    {
        _isMoving = true;
        var iniPosition = itemObj.transform.position;
        while (transform.position != moveTo)
        {
            Vector3 newPosition = moveTo - transform.position;
            transform.Translate(newPosition.normalized * Time.deltaTime * _moveSpeed);
            yield return null;
        }

        _isMoving = false;
        Destroy(itemObj);
    }
}
