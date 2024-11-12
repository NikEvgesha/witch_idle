using Unity.VisualScripting;
using UnityEngine;

public class ItemModel : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2;
    private Transform _target;
    private float _lerpPoint;
    private Vector3 _from;
    private Vector3 _to;
    private Vector3 _lerpPos;
    private float _y;
    private float _yStep = 0.2f;
    public void Config(Transform target)
    {
        _target = target;
        _lerpPoint = 0;
        transform.localScale = new Vector3(2, 2, 2);
        _y = this.transform.position.y;
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            _lerpPoint += Time.fixedDeltaTime * _moveSpeed;
            _from = new Vector3(this.transform.position.x, 0, this.transform.position.z);
            _to = new Vector3(_target.position.x, 0, _target.position.z);
            _lerpPos = Vector3.Lerp(this.transform.position, _target.position, _lerpPoint);
            _y = (_lerpPoint < 0.5) ? _y + _yStep : _y - _yStep;
            this.transform.position = new Vector3(_lerpPos.x, _y, _lerpPos.z);
            if (_lerpPoint >= 1)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
