using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing, thrown, explosion, fire}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject createdObject = null;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;

    bool isDamaging;
    bool canDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing || type == damageType.explosion)
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

        if (dmg!= null && type != damageType.DOT && type != damageType.fire)
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
            StartCoroutine(fuseTimer());
        }

        IStatEff stat = other.GetComponent<IStatEff>();

        if (stat != null && type == damageType.fire)
        {
            stat.fire(damageRate, damageAmount);
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

    IEnumerator fuseTimer()
    {
        yield return new WaitForSeconds(destroyTime);
        if (createdObject != null)
            Instantiate(createdObject, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }
}
