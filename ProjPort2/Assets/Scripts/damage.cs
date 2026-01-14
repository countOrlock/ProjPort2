using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing, thrown}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;
    bool canDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);

            if (type == damageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }

        if (type == damageType.thrown)
        {
            rb.linearVelocity = transform.forward * speed;
            canDamage = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == damageType.homing)
        {
            rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg!= null && type != damageType.DOT)
        {
            if (type == damageType.thrown)
            {
                if(canDamage)
                {
                    dmg.takeDamage(damageAmount);
                    canDamage = false;
                }
            }
            else
            {
                dmg.takeDamage(damageAmount);
            }
        }

        if (type == damageType.homing || type == damageType.moving)
        {
            Destroy(gameObject);
        }

        if (type == damageType.thrown)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
