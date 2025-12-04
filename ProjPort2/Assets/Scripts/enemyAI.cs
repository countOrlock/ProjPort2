using UnityEngine;
using UnityEngine.AI;
public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] bool scaredOfPlayer;

    GameObject player;

    bool playerInRange;

    Vector3 playerDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !scaredOfPlayer)
        {
            agent.SetDestination(player.transform.position);
        }
        else if (playerInRange && scaredOfPlayer)
        {
            playerDir = player.transform.position - transform.position;


            float xVectorSwapNum = 2 / Mathf.Abs(playerDir.x) + 1;
            float zVectorSwapNum = 2 / Mathf.Abs(playerDir.z) + 1;

            float oppositePlayerX = player.transform.position.x - (xVectorSwapNum * playerDir.x);
            float oppositePlayerZ = player.transform.position.z - (zVectorSwapNum * playerDir.z);

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
