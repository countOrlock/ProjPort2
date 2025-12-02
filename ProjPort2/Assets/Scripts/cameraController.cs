using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Range (50, 500)][SerializeField] int sens;
    [SerializeField] int maxUP, maxDOWN;
    [SerializeField] bool invert;

    float camUpDown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
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
}
