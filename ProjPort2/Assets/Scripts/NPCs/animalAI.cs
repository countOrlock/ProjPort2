using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class animalAI : MonoBehaviour, IDamage, IStatEff
{
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

    [Header("----- Status Effects -----")]
    float fireTimer;

    public bool isBurning;
    public bool isSlow;
    float slowMod;

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
        stoppingDistOrig = agent.stoppingDistance;
        speedOrig = agent.speed;
        slowMod = 1;
    }

    // Update is called once per frame
    void Update()
    {
        meleeTimer += (slowMod * Time.deltaTime);
        fireTimer += Time.deltaTime;

        locomotionAnim();

        if (targetPos != null)
        {
            distToTarget = (targetPos - transform.position).magnitude;
        }


        targetPos = startingPos;

        if (agent.remainingDistance < 0.01)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange && targetPos != null && distToTarget > maxDistFromTarget)
        {
            moveToTarget();
        }
        else if (!playerInRange)
        {
            checkRoam();
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

    public void playHurtSound()
    {
        aud.PlayOneShot(hurtSound[Random.Range(0, hurtSound.Length)], hurtVol);
    }

    public void playAttackSound()
    {
        aud.PlayOneShot(attackSound[Random.Range(0, attackSound.Length)], attackVol);
    }

    public void playDeathSound()
    {
        aud.PlayOneShot(deathSound[Random.Range(0, deathSound.Length)], deathVol);
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
        if (Physics.Raycast(transform.position, playerDir, out hit, Mathf.Infinity, ~enemyIgnoreLayer))
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
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }


    public void takeDamage(int amount)
    {
        HP -= amount;
        if (aggressive)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
            if (dropItem != null)
            {
                Instantiate(dropItem, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }

            NPCManager.instance.UpdateNPCCount(gameObject, -1);

            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
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
