using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int spawnAmount;
    [SerializeField] float spawnRate;

    int spawnCount;
    float spawnTimer;

    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.instance.updateGameGoal(spawnAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if (spawnCount < spawnAmount && spawnTimer >= spawnRate)
            {
                spawn();
            }
        }
    }

    void spawn()
    {
        spawnTimer = 0;
        spawnCount++;
        Instantiate(objectToSpawn, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
