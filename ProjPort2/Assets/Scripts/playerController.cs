using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Range (1, 10)][SerializeField] int wSpeed;
    [Range (1, 10)][SerializeField] int rSpeed;
    [Range (1, 20)][SerializeField] int jumpSpeed;
    [SerializeField] float gravity;
    [SerializeField] int jumpCount;

    Vector3 moveDir;

    Vector2 walkDir;

    float jumpMod;

    int speedMod;

    int maxJump;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jumpMod = 0f;
        speedMod = wSpeed;
        maxJump = jumpCount;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        //jumping and gravity

        if (!controller.isGrounded)
        {
            jumpMod = jumpMod - gravity;
        }
        else 
        {
            jumpMod = 0;
            jumpCount = maxJump;
        }

        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpMod = jumpSpeed;
            jumpCount--;
        }

        //sprinting
        if (Input.GetButton("Sprint"))
        {
            speedMod = rSpeed;
        }
        else
        {
            speedMod = wSpeed;
        }

        //movement execution
        walkDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        moveDir = walkDir.x * transform.right * speedMod + walkDir.y * transform.forward * speedMod + jumpMod * transform.up;
        controller.Move(moveDir * Time.deltaTime);
    }
}
