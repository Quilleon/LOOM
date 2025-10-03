using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    private Camera _mainCamera;
    
    void Start()
    {
        _mainCamera = Camera.main;
    }

    
    void LateUpdate()
    {
        if (_mainCamera)
        {
            transform.LookAt(_mainCamera.transform.position);
            transform.rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, 0f);
            //transform.forward = _mainCamera.transform.forward;
        }
    }
}