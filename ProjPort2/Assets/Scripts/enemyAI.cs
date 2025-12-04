using UnityEngine;
using UnityEngine.AI;
public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] bool scaredOfPlayer;

    bool playerInRange;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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


            float xVectorSwapNum = 2 / Mathf.Abs(playerDir.x) + 1;
            float zVectorSwapNum = 2 / Mathf.Abs(playerDir.z) + 1;

            float oppositePlayerX = gameManager.instance.player.transform.position.x - (xVectorSwapNum * playerDir.x);
            float oppositePlayerZ = gameManager.instance.player.transform.position.z - (zVectorSwapNum * playerDir.z);

            Debug.Log(oppositePlayerX);

            agent.SetDestination(new Vector3(oppositePlayerX, transform.position.y, oppositePlayerZ));
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
            Destroy(gameObject);
        }
    }
}
