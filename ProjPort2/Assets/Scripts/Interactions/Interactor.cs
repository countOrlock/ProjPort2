using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public bool inRange;
    public float interactRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(interactorSource.position, interactorSource.forward, out hit, interactRange))
            {
                IInteractable interact = hit.collider.GetComponent<IInteractable>();
                if (interact != null)
                {
                    gameManager.instance.InteractOn();
                    if (Input.GetButtonDown("Interact"))
                    {
                        interact.Interact();
                    }
                }
            }
            else
            {
                gameManager.instance.InteractOff();
            }
            //if (Input.GetButtonDown("Interact")) ;
            //{
            //    Ray ray = new Ray(interactorSource.position, interactorSource.forward);
            //    if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
            //    {
            //        if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            //        {
            //            interactObj.Interact();
            //        }
            //    }
            //}
        }
    }
}
