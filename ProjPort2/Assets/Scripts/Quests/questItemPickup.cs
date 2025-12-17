using UnityEngine;

public class questItemPickup : MonoBehaviour
{
    [SerializeField] questInfo quest;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null)
        {
            pik.getQuestItem(quest);
            Destroy(gameObject);
        }
    }
}
