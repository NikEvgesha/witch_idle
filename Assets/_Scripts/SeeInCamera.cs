using DG.Tweening;
using UnityEngine;

public class SeeInCamera : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    private Canvas _canvas;
    [SerializeField] int testVector;
    [SerializeField] Vector3 Vector3Test;

    private void OnEnable()
    {
        _canvas = gameObject.GetComponent<Canvas>();
        _mainCamera = FindObjectOfType<Camera>();
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (var camera in cameras)
        {
            if(camera.tag == "MainCamera") 
            {
                _mainCamera = camera;
            }
        }
        _canvas.worldCamera = _mainCamera;
        
    }
    private void Update()
    {
        Vector3 testV = Vector3.zero;
        switch (testVector)
        {
            case 0:
                gameObject.transform.LookAt(_mainCamera.transform.position);
                return;
            case 1:
                testV = Vector3.left;
                break;
            case 2:
                testV = Vector3.up;
                break;
            case 3:
                testV = Vector3.down;
                break;
            case 4:
                testV = Vector3.forward;
                break;
            case 5:
                testV = Vector3.back;
                break;
            case 6:
                testV = Vector3.one;
                break;
            case 7:
                testV = Vector3.zero;
                break;
            case 8:
                testV = Vector3.forward;
                break;
            case 9:
                testV = Vector3.negativeInfinity;
                break;
            case 10:
                testV = Vector3.positiveInfinity;
                break;
            case 11:
                testV = Vector3.right;
                break;
            default:
                break;
        }
        gameObject.transform.LookAt(-_mainCamera.transform.position, testV);
        Vector3Test = _mainCamera.transform.position;
    }
}
