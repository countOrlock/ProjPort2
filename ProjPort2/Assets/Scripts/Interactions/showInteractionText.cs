using UnityEngine;

public class showInteractionText : MonoBehaviour
{
    SphereCollider newCollider;

    public void Start()
    {
        newCollider = GetComponent<SphereCollider>();
    }
    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            gameManager.instance.playerInteract.interactRange = newCollider.radius;
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
