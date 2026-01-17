using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class enemyAI : MonoBehaviour, IDamage, IStatEff
{
    public enum npcMode 
    { 
        Roam,
        Attack,
        Patrol,
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
    [SerializeField] bool scaredOfPlayer;
    [SerializeField] bool shootsProjectile;
    [SerializeField] bool attacksMelee;

    [Header("----- If Shoots Projectile -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    [Header("----- If Attacks Melee -----")]
    [SerializeField] LayerMask enemyIgnoreLayer;
    [SerializeField] int meleeDamage;
    [SerializeField] int meleeRange;
    [SerializeField] float meleeRate;
    [SerializeField] Transform attackPos;
    public bool debugHasMeleeAnim;



    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] shootSound;
    [Range(0f, 1f)][SerializeField] float shootVol;

    [Header("----- Status Effects -----")]
    float fireTimer;

    public bool isBurning;


    Color colorOrig;

    [Header("----- AI Logic / Behavior Variables -----")]
    [SerializeField] Waypoint startingWaypoint;
    [SerializeField] Waypoint currentWaypoint;
    [SerializeField] Vector3 targetPos;
    [SerializeField] Vector3 waypointPos;
    [SerializeField] int maxDistFromTarget;
    [SerializeField] int maxDistFromWaypoint;
    float distToTarget;
    float distToWaypoint;
    npcMode mode;
    bool resumingPatrol;

    [SerializeField] int resumePatrolTime;
    float resumePatrolTimer;

    [Header("----- Roaming -----")]
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTimer;
    float stoppingDistOrig;
    float roamTimer;

    float shootTimer;
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
        targetPos = transform.position;
        mode = npcMode.Roam;
        if (startingWaypoint)
        {
        waypointPos = startingWaypoint.transform.position;
        currentWaypoint = startingWaypoint;
        }
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;
        fireTimer += Time.deltaTime;

        locomotionAnim();

        // Logic for ALL non-waypoint related movement
        if(targetPos != null)
        {
            distToTarget = (targetPos - transform.position).magnitude;
        }

        if (agent.remainingDistance < 0.01 && !resumingPatrol)
        {
            roamTimer += Time.deltaTime;
        }
        else
        {
            roamTimer = 0.0f;
        }

        //Logic for JUST waypoint related movement
        if (waypointPos != null)
        {
            distToWaypoint = (waypointPos - transform.position).magnitude;
        }

        if (distToWaypoint < maxDistFromWaypoint)
        {
            SetWaypointPos(currentWaypoint.nextWaypoint.transform.position);
            currentWaypoint = currentWaypoint.nextWaypoint;
        }

        if (resumingPatrol)
        {
            resumePatrolTimer += Time.deltaTime;
        }

            // Finite State Machine -- Used to control switching between behaviors
            switch (mode)
            {
                // Basic Roam mode
                case npcMode.Roam:

                    checkRoam();
                    if (!playerInRange && currentWaypoint != null)
                    {
                        mode = npcMode.Patrol;
                    }
                    else if (playerInRange && canSeePlayer())
                    {
                        mode = npcMode.Attack;
                    }
                    break;
                // Attack mode following the NPC's designated attack logic
                // Happens mostly in canSeePlayer() during that check.
                case npcMode.Attack:
                    if (playerInRange && !canSeePlayer())
                    {
                        mode = npcMode.Roam;
                    }
                    else if (!playerInRange && currentWaypoint != null)
                    {
                        mode = npcMode.Patrol;
                    }
                    break;
                // Patrol mode following waypoints
                case npcMode.Patrol:
                    
                    // Resuming patrol once the enemy has finished it's current move action.
                    if (!resumingPatrol && resumePatrolTimer < resumePatrolTime && agent.remainingDistance < 0.01)
                    {
                        resumingPatrol = true;
                    }

                    // Moving to the next patrol waypoint once the original "resuming patrol" timer has gone
                    // above the SerializedField set variable.
                    if (resumePatrolTimer >= resumePatrolTime)
                    {
                        SetWaypointPos(currentWaypoint.transform.position);
                        moveToTarget(waypointPos);
                    }

                    // Have to reset the patrol timer and boolean variables when going into other NPC Modes
                    if (playerInRange && !canSeePlayer())
                    {
                        moveToTarget(transform.position);
                        resumingPatrol = false;
                        resumePatrolTimer = 0.0f;
                        mode = npcMode.Roam;
                        break;
                    }
                    else if (playerInRange && canSeePlayer())
                    {
                        resumingPatrol = false;
                        resumePatrolTimer = 0.0f;
                        mode = npcMode.Attack;
                        break;
                    }

                    break;
            }
    }

    public void SetTarget(Vector3 newTarget)
    {
        targetPos = newTarget;
    }

    public void SetWaypointPos(Vector3 newWaypoint)
    {
        waypointPos = newWaypoint;
    }

    void locomotionAnim()
    {
        float agentSpeedCurr = agent.velocity.normalized.magnitude;
        float agentSpeedAnim = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(agentSpeedAnim, agentSpeedCurr, Time.deltaTime * animTranSpeed));
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
                if (!scaredOfPlayer)
                {
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
                else if (scaredOfPlayer)
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

                if (shootsProjectile && shootTimer >= shootRate)
                {
                    shootPos.LookAt(hit.point);
                    shoot();
                }

                if (attacksMelee && meleeTimer >= meleeRate && inMeleeRange())
                {
                    if (!debugHasMeleeAnim)
                        meleeAttack();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
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
                if (debugHasMeleeAnim)
                    anim.SetTrigger("Melee");
                return true;
            }
        }

        return false;
    }

    void shoot()
    {
        shootTimer = 0;
        anim.SetTrigger("Shoot");
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, shootPos.transform.rotation);
        aud.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], shootVol);
    }

    void meleeAttack()
    {
        meleeTimer = 0;
        IDamage dmg = gameManager.instance.player.GetComponent<IDamage>();
        dmg.takeDamage(meleeDamage);
    }

    void moveToTarget(Vector3 target)
    {
        agent.stoppingDistance = 0;
        agent.SetDestination(target);
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
        if (!scaredOfPlayer)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
            if(dropItem != null)
            {
                Instantiate(dropItem, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }

            if (shootsProjectile)
                gameManager.instance.hunterAmountCurr--;

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
}
