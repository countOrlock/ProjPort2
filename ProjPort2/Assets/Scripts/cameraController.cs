using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Range (50, 500)][SerializeField] float sens;
    [SerializeField] int maxUP, maxDOWN;
    [Range(0, 100)][SerializeField] int zoomSpeed;
    [SerializeField] bool invert;

    private Camera _camera;
    public Camera _gunCam;

    float camUpDown;
    float zoomMod;

    float targetFOV;
    float gcTargetFOV;
    float FOVOrig;
    float gcFOVOrig;
    float sensOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _camera = GetComponent<Camera>();
        
        FOVOrig = _camera.fieldOfView;
        gcFOVOrig = _gunCam.fieldOfView;
        targetFOV = FOVOrig;
        gcTargetFOV = gcFOVOrig;
        sensOrig = sens;
    }

    // Update is called once per frame
    void Update()
    {
        zoomAdjust();
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        if (invert)
        {
            camUpDown += mouseY;
        }
        else
        {
            camUpDown -= mouseY;
        }

        camUpDown = Mathf.Clamp(camUpDown, maxDOWN, maxUP);

        transform.localRotation = Quaternion.Euler(camUpDown, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);
    }

    public void zoomIn(float amount)
    {
        zoomMod = amount;
        sens = sens * zoomMod;
        targetFOV = _camera.fieldOfView * zoomMod;
        gcTargetFOV = _gunCam.fieldOfView * zoomMod;
    }

    public void zoomOut()
    {
        zoomMod = 1;
        sens = sensOrig;
        targetFOV = FOVOrig;
        gcTargetFOV = gcFOVOrig;
    }

    void zoomAdjust()
    {
        if (_camera.fieldOfView != targetFOV)
        {
            _camera.fieldOfView = Mathf.MoveTowards(_camera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
            _gunCam.fieldOfView = Mathf.MoveTowards(_gunCam.fieldOfView, gcTargetFOV, zoomSpeed * Time.deltaTime);
        }
    }
}
