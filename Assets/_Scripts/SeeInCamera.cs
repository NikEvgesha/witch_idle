using UnityEngine;

public class SeeInCamera : MonoBehaviour
{
    private Camera _mainCamera;
    private Canvas _canvas;

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
        gameObject.transform.LookAt(_mainCamera.transform.position);
    }
}
