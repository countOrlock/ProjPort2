using UnityEngine;
using System.Collections;

public class bomb : MonoBehaviour
{
    public GameObject explosion;
    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroyTime);
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
        
    }

    
}
