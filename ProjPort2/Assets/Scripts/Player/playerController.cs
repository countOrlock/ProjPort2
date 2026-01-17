using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

public class playerController : MonoBehaviour, IDamage, IPickup, IStatEff
{
    public enum stanceType { sprinting, standing, crouching, prone, dead};
    public stanceType stance;
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerCam;

    [Header("----- Player Stats -----")]
    [Range(1,  10)][SerializeField] int HP;
    [Range(1,  10)][SerializeField] int wSpeed;
    [Range(1,  10)][SerializeField] int rSpeed;
    [Range(1,  10)][SerializeField] int cSpeed;
    [Range(1,  10)][SerializeField] int pSpeed;
    [Range(1,  20)][SerializeField] int jumpSpeed;
    [Range(0f, 3f)][SerializeField] float cHeight;
    [Range(0f, 3f)][SerializeField] float pHeight;
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
    float controllerHeightOrig;
    float targetHeight;

    [Header("----- Gun Fields -----")]
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject gunCam;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LineRenderer Laser;
    float shootTimer;
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    int gunListPos;
    public int currentAmmo;
    public int maxAmmo;
    public int currentMags;
    public int maxMags;


    bool reloading = false;

    [Header("----- Throwable Fields -----")]
    [SerializeField] GameObject throwModel;
    [SerializeField] List<throwStats> throwList = new List<throwStats>();
    int throwListPos;
    float throwTimer;
    float throwRate;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] stepSound;
    [Range(0, 1f)][SerializeField] float stepVol;
    [SerializeField] AudioClip[] jumpSound;
    [Range(0, 1f)][SerializeField] float jumpVol; 
    [SerializeField] AudioClip[] hurtSound;
    [Range(0, 1f)][SerializeField] float hurtVol;

    bool isPlayingStep;


    [Header("----- Quest Fields -----")]
    [Range(0, 1000)][SerializeField] public int Gold;
    [SerializeField] List<questInfo> questList = new List<questInfo>();
    [SerializeField] List<GameObject> questItemList = new List<GameObject>();
    public string questName;
    public string questObjective;

    [Header("----- Status Effect -----")]
    float fireTimer;

    public bool isBurning;

    [Header("----- Animation -----")]
    [SerializeField] Animator anim;


    public enum questID
    {
        Completed, // 0
        In_Progress, // 1
        Not_Accepted // 2
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();

        jumpMod = 0f;
        speedMod = wSpeed;
        maxJump = jumpCount;
        heightOrig = controller.height;
        controllerHeightOrig = controller.center.y;
        targetHeight = heightOrig;
        stance = stanceType.standing;
    }

    // Update is called once per frame
    void Update()
    {
        switch (stance)
        {
            case stanceType.dead:
                break;
            default:
                shootTimer += Time.deltaTime;
                throwTimer += Time.deltaTime;
                fireTimer += Time.deltaTime;

                movement();

                if (gunList.Any())
                    Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunList[gunListPos].shootDist, Color.white);
                break;
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

        //zoom
        if (Input.GetButtonDown("Fire2") && gunList.Any())
            playerCam.GetComponent<cameraController>().zoomIn(gunList[gunListPos].zoomMod);
        else if (Input.GetButtonUp("Fire2"))
            playerCam.GetComponent<cameraController>().zoomOut();

        //throwable object
        if (Input.GetButtonDown("Fire3") && throwList.Any() && throwList[throwListPos].ammoCurr > 0 && !gameManager.instance.isPaused)
        {
            throwItem();
        }
        selectThrow();

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

        if(Input.GetButtonDown("Reload Gun") && gunList.Count > 0 && reloading == false && gunList[gunListPos].magsCur > 0 && gunList[gunListPos].ammoCur < gunList[gunListPos].ammoMax)
        {
            StartCoroutine(Reload());
        }

        //movement execution
        walkDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        if (walkDir.magnitude > 0.3f && !isPlayingStep && controller.isGrounded)
            StartCoroutine(playStep());

        moveDir = walkDir.x * transform.right * speedMod + walkDir.y * transform.forward * speedMod + jumpMod * transform.up;
        controller.Move(moveDir * Time.deltaTime);

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= gunList[gunListPos].shootRate && reloading == false && !gameManager.instance.isPaused)
        {
            
            shoot();
        }
        else
        {
            Laser.enabled = false;
        }
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

        if(controller.height != targetHeight)
        {
            float newHeight = Mathf.MoveTowards(controller.height, targetHeight, stanceChangeSpeed * Time.deltaTime);
            float heightChange = newHeight - controller.height;
            playerCam.transform.Translate(0, heightChange, 0, Space.World);
            controller.height += heightChange;
            controller.center = new Vector3(0, controller.center.y + (heightChange / heightOrig) * controllerHeightOrig, 0);
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

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(stepSound[Random.Range(0, stepSound.Length)], stepVol);
        if (stance == stanceType.sprinting)
            yield return new WaitForSeconds(0.3f);
        else
            yield return new WaitForSeconds(0.5f);
        isPlayingStep = false;
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
                aud.PlayOneShot(jumpSound[Random.Range(0, jumpSound.Length)], jumpVol);
            }
        }
    }

    void shoot()
    {
        shootTimer = 0;

        gunList[gunListPos].ammoCur--;
        gameManager.instance.updateAmmoCount(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoMax);

        if (gunList[gunListPos].shootSound.Length > 0)
            aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);

        recoilSpeed += -Camera.main.transform.forward * gunList[gunListPos].recoil;

        if (gunList[gunListPos].Bullet == null || gunList[gunListPos].shootLaser == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, gunList[gunListPos].shootDist, ~ignoreLayer))
            {
                if (gunList[gunListPos].shootEffect != null)
                {
                    Instantiate(gunList[gunListPos].shootEffect, gunModel.transform.position, playerCam.transform.rotation);
                }


                if (gunList[gunListPos].hitEffect != null)
                {
                    Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
                }

                if(gunList[gunListPos].shootLaser == true)
                {

                    Laser.enabled = true;

                    Laser.SetPosition(0, gunModel.transform.position);
                    Laser.SetPosition(1, hit.point);

                    if(gunList[gunListPos].Bullet != null)
                    {
                        Instantiate(gunList[gunListPos].Bullet, hit.point, Quaternion.identity);
                    }
                }

                Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(gunList[gunListPos].shootDamage);
                }
            }
        }
        else
        {
            Instantiate(gunList[gunListPos].Bullet, playerCam.transform.position, playerCam.transform.rotation);
        }

    }

    public void getQuestItem(GameObject quest)
    {
        questItemList.Add(quest);
    }

    public void getGunStats(gunStats gun)
    {
        if(gunList.Any())
        {
            for (int i = 1; i < gunList.Count; i++)
            {
                if (gunList[i] == gun)
                {
                    gunList[i].magsCur = gunList[i].magsMax;
                    gameManager.instance.updateMagCount(gunList[gunListPos].magsCur);
                    return;
                }
            }
        }

        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        gunList[gunListPos].magsCur = gunList[gunListPos].magsMax;
        gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        gameManager.instance.updateAmmoCount(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoMax);
        gameManager.instance.updateMagCount(gunList[gunListPos].magsCur);
        changeGun();
    }

    void changeGun()
    {
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterials = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterials;
        gameManager.instance.updateAmmoCount(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoMax);
        gameManager.instance.updateMagCount(gunList[gunListPos].magsCur);
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1 && reloading == false)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0 && reloading == false)
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
        aud.PlayOneShot(hurtSound[Random.Range(0, hurtSound.Length)], hurtVol);

        if (HP <= 0)
        {
            die();
        }
    }

    public void die()
    {
        stance = stanceType.dead;
        gunCam.SetActive(false);
        anim.SetTrigger("Dead");
    }

    public void lose()
    {
        gameManager.instance.youLose();
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.updateAmmoCount(currentAmmo, maxAmmo);
        gameManager.instance.updateMagCount(currentMags);
    }

    IEnumerator flashRed()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    IEnumerator Reload()
    {
        gunList[gunListPos].magsCur--;
        gameManager.instance.updateMagCount(gunList[gunListPos].magsCur);
        aud.PlayOneShot(gunList[gunListPos].reloadSound[Random.Range(0, gunList[gunListPos].reloadSound.Length)], gunList[gunListPos].reloadSoundVol);

        reloading = true;
        yield return new WaitForSeconds(gunList[gunListPos].reloadRate);
        gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        gameManager.instance.updateAmmoCount(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoMax);
        reloading = false;
    }

    public void getThrowStats(throwStats item)
    {
        if (throwList.Any())
        {
            for (int i = 1; i < throwList.Count; i++)
            {
                if (throwList[i] == item)
                {
                    throwList[i].ammoCurr = throwList[i].ammoMax;
                    //gameManager.instance.updateItemCount(throwList[i].ammoMax);
                    return;
                }
            }
        }

        throwList.Add(item);
        throwListPos = throwList.Count - 1;
        throwList[throwListPos].ammoCurr = throwList[throwListPos].ammoMax;
        //gameManager.instance.updateAmmoCount(gunList[gunListPos].ammoCur, gunList[gunListPos].ammoMax);
        //gameManager.instance.updateMagCount(gunList[gunListPos].magsCur);
        changeThrow();
    }

    void throwItem()
    {
        throwTimer = 0;
        throwList[throwListPos].ammoCurr--;
        gameManager.instance.updateItemCount(throwList[throwListPos].ammoCurr);

        if (throwList[throwListPos].throwSound.Length > 0)
            aud.PlayOneShot(throwList[throwListPos].throwSound[Random.Range(0, throwList[throwListPos].throwSound.Length)], throwList[throwListPos].throwSoundVol);

        if (throwList[throwListPos].animObject != null)
        {
            throwModel.GetComponent<MeshFilter>().sharedMesh = throwList[throwListPos].animObject.GetComponent<MeshFilter>().sharedMesh;
            throwModel.GetComponent<MeshRenderer>().sharedMaterial = throwList[throwListPos].animObject.GetComponent<MeshRenderer>().sharedMaterial;
            //add animation code here
        }
        else
        {
            Instantiate(throwList[throwListPos].projectile, playerCam.transform.position, playerCam.transform.rotation);
        }
    }

    void changeThrow()
    {
        //gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        //gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        gameManager.instance.updateItem(throwList[throwListPos].itemName);
        gameManager.instance.updateItemCount(throwList[throwListPos].ammoCurr);
    }

    void selectThrow()
    {
        if (Input.GetButtonDown("Throwable Right") && throwListPos < throwList.Count - 1 && reloading == false)
        {
            throwListPos++;
            changeThrow();
        }
        else if (Input.GetButtonDown("Throwable Left") && throwListPos > 0 && reloading == false)
        {
            throwListPos--;
            changeThrow();
        }
    }

    public void fire(float time, int hpRate)
    {
        fireTimer = 0;

        if (!isBurning)
        {
            StartCoroutine(burning(time, hpRate));
        }
    }

    IEnumerator burning (float time, int hpRate)
    {
        isBurning = true;
        while (fireTimer < time && stance != stanceType.dead)
        {
            takeDamage(hpRate);
            yield return new WaitForSeconds(0.5f);
        }
        isBurning = false;
    }
}
