using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    public enum stanceType { sprinting, standing, crouching, prone};
    public stanceType stance;
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerCam;

    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int wSpeed;
    [Range(1, 10)][SerializeField] int rSpeed;
    [Range(1, 10)][SerializeField] int cSpeed;
    [Range(1, 10)][SerializeField] int pSpeed;
    [Range(1, 20)][SerializeField] int jumpSpeed;
    [Range(0f, 1f)][SerializeField] float cHeight;
    [Range(0f, 1f)][SerializeField] float pHeight;
    [SerializeField] float gravity;
    [SerializeField] int jumpCount;
    [SerializeField] float stanceChangeSpeed;

    

    Vector3 moveDir;
    Vector2 walkDir;
    Vector3 recoilSpeed;

    float jumpMod;
    int speedMod;
    int maxJump;
    int HPOrig;
    float heightOrig;
    float targetHeight;

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
        HPOrig = HP;
        updatePlayerUI();

        jumpMod = 0f;
        speedMod = wSpeed;
        maxJump = jumpCount;
        heightOrig = playerCam.transform.localPosition.y;
        targetHeight = heightOrig;
        stance = stanceType.standing;
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
        jump();

        //sprinting
        sprint();

        selectGun();

        //crouch/prone
        stanceChange();

        //recoil
        if (recoilSpeed.magnitude > 0.1f)
        {
            controller.Move(recoilSpeed * Time.deltaTime);
            recoilSpeed = Vector3.Lerp(recoilSpeed, Vector3.zero, 5f * Time.deltaTime);
        }
        else
        {
            recoilSpeed = Vector3.zero;
        }

        //movement execution
        walkDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        moveDir = walkDir.x * transform.right * speedMod + walkDir.y * transform.forward * speedMod + jumpMod * transform.up;
        controller.Move(moveDir * Time.deltaTime);
    }

    void setStance()
    {
        switch(stance)
        {
            case stanceType.sprinting:
                targetHeight = heightOrig;
                speedMod = rSpeed;
                break;

            case stanceType.standing:
                targetHeight = heightOrig;
                speedMod = wSpeed;
                break;

            case stanceType.crouching:
                targetHeight = cHeight;
                speedMod = cSpeed;
                break;

            case stanceType.prone:
                targetHeight = pHeight;
                speedMod = pSpeed;
                break;
        }
    }
    void stanceChange()
    {
        if(Input.GetButtonDown("Crouch"))
        {
            stance = stanceType.crouching;
            setStance();
        }

        if(Input.GetButtonDown("Prone"))
        {
            stance = stanceType.prone;
            setStance();
        }

        if(playerCam.transform.localPosition.y != targetHeight)
        {
            float newHeight = Mathf.MoveTowards(playerCam.transform.localPosition.y, targetHeight, stanceChangeSpeed * Time.deltaTime);
            float heightChange = newHeight - playerCam.transform.localPosition.y;
            playerCam.transform.Translate(0, heightChange, 0);
        }
    }

    void sprint()
    {
        if (Input.GetButton("Sprint"))
        {
            stance = stanceType.sprinting;
            setStance();
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            stance = stanceType.standing;
            setStance();
        }

        
    }

    void jump()
    {
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
            if (stance == stanceType.crouching || stance == stanceType.prone)
            {
                stance = stanceType.standing;
                setStance();
            }
            else
            {
                jumpMod = jumpSpeed;
                jumpCount--;
            }
        }
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

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashRed()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

}
