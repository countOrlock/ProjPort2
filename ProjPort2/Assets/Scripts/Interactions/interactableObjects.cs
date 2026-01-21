using UnityEngine;

public class interactableObjects : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        gameManager.instance.ShopMenu();
    }

}
