using UnityEngine;

public class showInteractionText : MonoBehaviour
{
    [SerializeField] GameObject interactionPopupTxt;

    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            interactionPopupTxt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            interactionPopupTxt.SetActive(false);
        }
    }
}
