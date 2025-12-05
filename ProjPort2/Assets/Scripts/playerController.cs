using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IPickup
{
    [SerializeField] CharacterController controller;

    [Range(1, 10)][SerializeField] int wSpeed;
    [Range(1, 10)][SerializeField] int rSpeed;
    [Range(1, 20)][SerializeField] int jumpSpeed;
    [SerializeField] float gravity;
    [SerializeField] int jumpCount;

    Vector3 moveDir;
    Vector2 walkDir;

    float jumpMod;
    int speedMod;
    int maxJump;

    [Header("----- Gun Fields -----")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    float shootTimer;
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    int gunListPos;
    [SerializeField] GameObject gunModel;

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
        shootTimer += Time.deltaTime;

        movement();

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.white);

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
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

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun();
    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

}
