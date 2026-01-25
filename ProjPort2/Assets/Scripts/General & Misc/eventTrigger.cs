using UnityEngine;

public class eventTrigger : MonoBehaviour
{
    //alien event trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NPCManager.instance.AlienSpawnEvent();
        }
    }
}
