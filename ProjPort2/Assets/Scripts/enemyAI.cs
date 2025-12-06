using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
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

    bool playerInRange;
    bool playerInMeleeRange;

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

        if (playerInRange)
        {
            if (!scaredOfPlayer)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
            else
            {
                playerDir = gameManager.instance.player.transform.position - transform.position;

                float oppositePlayerX = transform.position.x - playerDir.x;
                float oppositePlayerZ = transform.position.z - playerDir.z;

                Vector3 targetPos = new Vector3(oppositePlayerX, transform.position.y, oppositePlayerZ);

                agent.SetDestination(targetPos);
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                playerDir = gameManager.instance.player.transform.position - transform.position;
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
        }
    }

    bool inMeleeRange()
    {
        RaycastHit hit;
        Physics.Raycast(attackPos.position, transform.forward, out hit, meleeRange, ~enemyIgnoreLayer);


        if (hit.collider.CompareTag("Player"))
        {
            Debug.Log(hit.collider);
            meleeAttack();
            return true;
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
