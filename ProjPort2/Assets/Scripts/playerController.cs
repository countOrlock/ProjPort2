using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    public enum stanceType { sprinting, standing, crouching, prone};
    public stanceType stance;
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerCam;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int wSpeed;
    [Range(1, 10)][SerializeField] int rSpeed;
    [Range(1, 10)][SerializeField] int cSpeed;
    [Range(1, 10)][SerializeField] int pSpeed;
    [Range(1, 20)][SerializeField] int jumpSpeed;
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
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int recoil;
    float shootTimer;
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    int gunListPos;
    GameObject Bullet = null;

    bool reloading = false;

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
    [SerializeField] List<questInfo> questList = new List<questInfo>();
    string questName;
    string questObjective;
    int questItems;

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
        shootTimer += Time.deltaTime;

        movement();

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.white);

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate && reloading == false)
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

        if (gunList[gunListPos].shootSound.Length > 0)
            aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);

        recoilSpeed += -Camera.main.transform.forward * recoil;

        if (Bullet == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
            {

                if (gunList[gunListPos].hitEffect != null)
                {
                    Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
                }

                Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(shootDamage);
                }

                IGiveQuest questGiver = hit.collider.GetComponent<IGiveQuest>();

                if (questGiver != null && questList.Count < 1)
                {
                    questList.Add(questGiver.giveQuest());
                }
            }
        }
        else
        {
            Instantiate(Bullet, playerCam.transform.position, playerCam.transform.rotation);
        }

    }

    public void getGunStats(gunStats gun)
    {
        for (int i = 0; i < gunList.Count; i++)
        {
            if (gunList[i] == gun)
            {
                
                gunList[i].magsCur = gunList[i].magsMax;
                return;
            }
        }

        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        gunList[gunListPos].magsCur = gunList[gunListPos].magsMax;
        gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
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

    public void getQuestItem(questInfo quest)
    {
        questItems++;
    }

    void assignQuest(questInfo quest)
    {
        // all of the quest for the player should initially be set to Not_Accepted

        switch(quest.questStatus)
        {
            case (int)questID.Not_Accepted: 
        // if the quest wasn't accepted yet it should change the status of the quest and change the necessary stats on the player

                break;

            case (int)questID.In_Progress:
        // if the quest is in progress it should check to see if the player has completed the quest and if they did change the status of the quest to completed

                break;

            case (int)questID.Completed:
        // if the quest is completed the player shouldn't be able to activate/get it again until the game restarts

                break;

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

    IEnumerator Reload()
    {
        gunList[gunListPos].magsCur--;
        reloading = true;
        yield return new WaitForSeconds(gunList[gunListPos].reloadRate);
        gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        reloading = false;
    }

}
