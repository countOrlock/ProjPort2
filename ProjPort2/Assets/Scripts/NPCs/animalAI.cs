using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class animalAI : MonoBehaviour, IDamage, IStatEff
{
    public enum npcMode
    { 
        Roam,
        Attack,
        Dying
    }


    [SerializeField] Animator anim;
    [SerializeField] public Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int animTranSpeed;

    [SerializeField] GameObject dropItem;

    [Header("----- Toggles -----")]
    [SerializeField] bool aggressive;

    [Header("----- If Attacks Melee -----")]
    [SerializeField] LayerMask enemyIgnoreLayer;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange;
    [SerializeField] float meleeRate;
    [SerializeField] Transform attackPos;
    public bool debugHasMeleeAnim;

    [Header("----- Roaming -----")]
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTimer;
    float stoppingDistOrig;

    [Header("----- Move To Target -----")]
    [SerializeField] Vector3 targetPos;
    [SerializeField] int maxDistFromTarget;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] stepSound;
    [Range(0f, 1f)][SerializeField] float stepVol;
    [SerializeField] AudioClip[] hurtSound;
    [Range(0f, 1f)][SerializeField] float hurtVol;
    [SerializeField] AudioClip[] attackSound;
    [Range(0f, 1f)][SerializeField] float attackVol;
    [SerializeField] AudioClip[] deathSound;
    [Range(0f, 1f)][SerializeField] float deathVol;
    [SerializeField] AudioClip[] idleSound;
    [Range(0f, 1f)][SerializeField] float idleVol;
    bool playingHurtSound;

    [Header("----- Status Effects -----")]
    float fireTimer;

    public bool isBurning;
    public bool isSlow;
    float slowMod;

    // AI Logic / behavior
    DeathCleanup enemyDeathCleanup;
    npcMode mode;
    bool isDying;
    bool isAttacking;

    float distToTarget;

    Color colorOrig;
    float speedOrig;

    float roamTimer;
    float meleeTimer;

    float angleToPlayer;
    float angleToPlayerVert;

    bool playerInRange;

    Vector3 playerDir;
    Vector3 startingPos;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        startingPos = transform.position;
        targetPos = startingPos;
        stoppingDistOrig = agent.stoppingDistance;
        speedOrig = agent.speed;
        slowMod = 1;
        mode = npcMode.Roam;
        isDying = false;
        enemyDeathCleanup = GetComponent<DeathCleanup>();
        playingHurtSound = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying)
        {
            return;
        }

        meleeTimer += (slowMod * Time.deltaTime);
        fireTimer += Time.deltaTime;

        locomotionAnim();

        if (targetPos != null)
        {
            distToTarget = (targetPos - transform.position).magnitude;
        }

        if (agent.remainingDistance < 0.01)
        {
            roamTimer += Time.deltaTime;
        }
        else
        {
            roamTimer = 0.0f;
        }

        // Finite State Machine -- Used to control switching between behaviors
        switch (mode)
        {
            // Basic Roam mode
            case npcMode.Roam:

                if (isDying)
                {
                    mode = npcMode.Dying;
                    break;
                }

                checkRoam();

                if (aggressive && playerInRange && canSeePlayer())
                {
                    mode = npcMode.Attack;
                }
                else if (canSeePlayer() && !playingHurtSound)
                {
                    StartCoroutine(playHurtSound());
                }
                break;
            // Attack mode following the NPC's designated attack logic
            // Happens mostly in canSeePlayer() during that check.
            case npcMode.Attack:
                if (isDying)
                {
                    mode = npcMode.Dying;
                    break;
                }

                if (aggressive)
                {
                    agent.SetDestination(gameManager.instance.player.transform.position);
                    if (!isAttacking)
                    {
                        playAttackSound();
                        isAttacking = true;
                    }
                }

                if (playerInRange && !canSeePlayer())
                {
                    playIdleSound();
                    mode = npcMode.Roam;
                }

                break;

            case npcMode.Dying:
                agent.isStopped = true;
                return;
        }
    }

    void locomotionAnim()
    {
        float agentSpeedCurr = agent.velocity.normalized.magnitude;
        float agentSpeedAnim = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(agentSpeedAnim, agentSpeedCurr, Time.deltaTime * animTranSpeed));
    }

    public void playStepSound()
    {
        aud.PlayOneShot(stepSound[Random.Range(0, stepSound.Length)], stepVol);
    }

    IEnumerator playHurtSound()
    {
        playingHurtSound = true;
        aud.PlayOneShot(hurtSound[Random.Range(0, hurtSound.Length)], hurtVol);
        yield return new WaitForSeconds(1);
        playingHurtSound = false;
    }

    public void playAttackSound()
    {
        if (attackSound.Any())
        {
            aud.PlayOneShot(attackSound[Random.Range(0, attackSound.Length)], attackVol);
        }
    }

    public AudioClip playDeathSound()
    {
        AudioClip dsound = deathSound[Random.Range(0, deathSound.Length)];
        aud.PlayOneShot(dsound, deathVol);
        return dsound;
    }

    public void playIdleSound()
    {
        aud.PlayOneShot(idleSound[Random.Range(0, idleSound.Length)], idleVol);
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit, 10, ~enemyIgnoreLayer))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                if (aggressive)
                {
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
                else
                {
                    float oppositePlayerX = transform.position.x - playerDir.x;
                    float oppositePlayerZ = transform.position.z - playerDir.z;

                    Vector3 targetPos = new Vector3(oppositePlayerX, transform.position.y, oppositePlayerZ);

                    agent.SetDestination(targetPos);
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (aggressive && meleeTimer >= meleeRate && inMeleeRange(hit))
                {
                    if (debugHasMeleeAnim)
                    {
                        anim.SetTrigger("Melee");
                    }
                    else
                        meleeAttack();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    bool inMeleeRange(RaycastHit hit)
    {
        if (hit.distance <= meleeRange)
        {
            Debug.Log(hit.collider);

            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }


        return false;
    }

    bool inMeleeRange()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, playerDir, out hit, meleeRange, ~enemyIgnoreLayer);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider);

            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }


        return false;
    }

    void meleeAttack()
    {
        meleeTimer = 0;
        IDamage dmg = gameManager.instance.player.GetComponent<IDamage>();
        if (inMeleeRange())
            dmg.takeDamage(meleeDamage);
    }

    void moveToTarget()
    {
        agent.stoppingDistance = 0;
        agent.SetDestination(targetPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.stoppingDistance = 0;
            playerInRange = false;
        }
    }

    void checkRoam()
    {
        if (roamDist > 0 && agent.remainingDistance < 0.01f && roamTimer >= roamPauseTimer)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;

        if (targetPos != null)
        {
            ranPos += transform.position;
        }
        else
        {
            ranPos += startingPos;
        }

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
        playIdleSound();
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }


    public void takeDamage(int amount)
    {
        HP -= amount;
        if (!playingHurtSound)
        {
            StartCoroutine(playHurtSound());
        }

        if (HP <= 0)
        {
            mode = npcMode.Dying;
            isDying = true;
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            //enemyDeathCleanup.Die();
            StartCoroutine(die());
        }
        else
        {
            StartCoroutine(flashRed());
        }

        if (mode != npcMode.Dying)
        {
            if (mode != npcMode.Attack)
            {
                mode = npcMode.Attack;
            }
        }
    }

    IEnumerator die()
    {
        AudioClip dsound = playDeathSound();
        yield return new WaitForSeconds(dsound.length);
        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }

        NPCManager.instance.UpdateNPCCount(gameObject, -1);

        Destroy(gameObject);
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public void fire(float time, int hpRate)
    {
        fireTimer = 0;

        if (!isBurning)
            StartCoroutine(burning(time, hpRate));
    }

    IEnumerator burning(float time, int hpRate)
    {
        isBurning = true;
        while (fireTimer < time)
        {
            takeDamage(hpRate);
            yield return new WaitForSeconds(0.5f);
        }
        isBurning = false;
    }

    public void slow(float time, float slowAmount)
    {
        if (!isSlow)
            StartCoroutine(slowed(time, slowAmount));
    }

    IEnumerator slowed(float time, float slowAmount)
    {
        isSlow = true;
        agent.speed = slowAmount * speedOrig;
        slowMod = slowAmount;
        yield return new WaitForSeconds(time);
        agent.speed = speedOrig;
        slowMod = 1;
        isSlow = false;
    }

    public void damageUP(float time, int damageAmount)
    {
        
    }

    public void speedUP(float time, float speedAmount)
    {
        
    }

    public void jumpUP(float time, float jumpAmount)
    {
        
    }

    public void jumpDouble(float time, int jumpAdd)
    {
        
    }

    public void healthUP(float time, float healRate, int healthAmount)
    {

    }

    public void drunk(float time)
    {
        
    }
}
