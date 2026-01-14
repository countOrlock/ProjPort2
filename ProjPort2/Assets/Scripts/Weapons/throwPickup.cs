using UnityEngine;

public class throwPickup : MonoBehaviour
{
    [SerializeField] throwStats item;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null)
        {
            pik.getThrowStats(item);
            Destroy(gameObject);
        }
    }
}
