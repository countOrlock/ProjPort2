using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] bool scaredOfPlayer;

    Color colorOrig;

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
        if (playerInRange && !scaredOfPlayer)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
        else if (playerInRange && scaredOfPlayer)
        {
            playerDir = gameManager.instance.player.transform.position - transform.position;

            float oppositePlayerX = transform.position.x - playerDir.x;
            float oppositePlayerZ = transform.position.z - playerDir.z;

            Vector3 targetPos = new Vector3(oppositePlayerX, transform.position.y, oppositePlayerZ);

            agent.SetDestination(targetPos);
        }
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
