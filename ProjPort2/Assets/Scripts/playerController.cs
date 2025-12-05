using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
    Vector3 recoilSpeed;

    float jumpMod;
    int speedMod;
    int maxJump;

    [Header("----- Gun Fields -----")]
    [SerializeField] GameObject gunModel;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int recoil;
    float shootTimer;
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    int gunListPos;
    GameObject Bullet = null;
    

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

        if (recoilSpeed.magnitude > 0.1f)
        {
            controller.Move(recoilSpeed * Time.deltaTime);
            recoilSpeed = Vector3.Lerp(recoilSpeed, Vector3.zero, 5f * Time.deltaTime);
        }
        else
        {
            recoilSpeed = Vector3.zero;
        }

        selectGun();

        //movement execution
        walkDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        moveDir = walkDir.x * transform.right * speedMod + walkDir.y * transform.forward * speedMod + jumpMod * transform.up;
        controller.Move(moveDir * Time.deltaTime);
    }

    void shoot()
    {
        shootTimer = 0;

        recoilSpeed += -Camera.main.transform.forward * recoil;

        if (Bullet == null)
        {
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
        else
        {
            Instantiate(Bullet, gunModel.transform.position, transform.rotation);
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
        recoil = gunList[gunListPos].recoil;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        Bullet = gunList[gunListPos].Bullet;

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
