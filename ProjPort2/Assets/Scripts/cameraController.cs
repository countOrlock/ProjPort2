using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Range (50, 500)][SerializeField] float sens;
    [SerializeField] int maxUP, maxDOWN;
    [SerializeField] bool invert;

    private Camera _camera;

    float camUpDown;
    float zoomMod;

    float targetFOV;
    float FOVOrig;
    float sensOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _camera = GetComponent<Camera>();
        FOVOrig = _camera.fieldOfView;
        targetFOV = FOVOrig;
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
    }

    public void zoomOut()
    {
        zoomMod = 1;
        sens = sensOrig;
        targetFOV = FOVOrig;
    }

    void zoomAdjust()
    {
        if (_camera.fieldOfView != targetFOV)
        {
            _camera.fieldOfView = targetFOV;
        }
    }
}
