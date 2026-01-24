using UnityEngine;
using System.Collections;

public class DeathCleanup : MonoBehaviour
{
    public float timeBeforeFade = 3f;
    public float fadeDuration = 2f;
    public float sinkSpeed = 0.5f;

    private Animator animator;
    private Rigidbody rb;
    private Collider col;
    private SkinnedMeshRenderer meshRenderer;

    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        col.enabled = false;
        StartCoroutine(DeathCleanupRoutine());
    }

    IEnumerator DeathCleanupRoutine()
    {
        yield return new WaitForSeconds(timeBeforeFade);
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Material mat = meshRenderer.material;
        Color startColor = mat.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = startPos - Vector3.up * (elapsed * sinkSpeed);
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
