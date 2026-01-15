using UnityEngine;

public class billboard : MonoBehaviour
{
    public enum BillboardType { LookAtCamera, CameraForward };

    [SerializeField] BillboardType billboardType;

    [Header("Lock Rotation")]
    [SerializeField] bool lockX;
    [SerializeField] bool lockY;
    [SerializeField] bool lockZ;

    Vector3 rotationOrig;

    private void Awake()
    {
        rotationOrig = transform.rotation.eulerAngles;
    }

    private void LateUpdate()
    {
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;

            case BillboardType.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            default:
                break;
        }

        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX)
            rotation.x = rotationOrig.x;
        if (lockY)
            rotation.y = rotationOrig.y;
        if (lockZ)
            rotation.z = rotationOrig.z;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
