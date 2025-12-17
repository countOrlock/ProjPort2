using UnityEngine;

public class questItemPickup : MonoBehaviour
{
    [SerializeField] GameObject questItem;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null)
        {
            pik.getQuestItem(questItem);
            Destroy(gameObject);
        }
    }
}
