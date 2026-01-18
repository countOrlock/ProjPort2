using UnityEngine;

public class showInteractionText : MonoBehaviour
{
    SphereCollider collider;

    public void Start()
    {
        collider = GetComponent<SphereCollider>();
    }
    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            gameManager.instance.playerInteract.interactRange = collider.radius;
            gameManager.instance.playerInteract.inRange = true;
        }
    }

    private void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            gameManager.instance.playerInteract.inRange = false;
        }
    }
}
