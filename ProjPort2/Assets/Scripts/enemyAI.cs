using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] int FOV;

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

    Color colorOrig;

    float shootTimer;
    float meleeTimer;

    float angleToPlayer;

    bool playerInRange;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        meleeTimer += Time.deltaTime;

        if (playerInRange && canSeePlayer())
        {
            Debug.Log("Player is visible");
        }
        Debug.DrawRay(transform.position, playerDir * 10, Color.coral);
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
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
                    shoot();
                }

                if (attacksMelee && meleeTimer >= meleeRate && inMeleeRange())
                {
                    Debug.DrawRay(attackPos.position, transform.forward * meleeRange, Color.purple);
                }

                return true;
            }
        }
        return false;
    }

    bool inMeleeRange()
    {
        RaycastHit hit;
        Physics.Raycast(attackPos.position, transform.forward, out hit, meleeRange, ~enemyIgnoreLayer);

        Debug.Log(hit.collider);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                meleeAttack();
                return true;
            }
        }


        return false;
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void meleeAttack()
    {
        meleeTimer = 0;
        IDamage dmg = gameManager.instance.player.GetComponent<IDamage>();
        dmg.takeDamage(meleeDamage);
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
            playerInRange = false;
        }
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
            gameManager.instance.updateGameGoal(-1);
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
}
