using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Range (1, 10)][SerializeField] int speed;

    Vector3 moveDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir.normalized * speed * Time.deltaTime);
    }
}
