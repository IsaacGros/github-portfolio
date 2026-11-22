using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    public float startDelay = 3f;
    public float leftBound = 0f;
    public float rightBound = 0f;

    [Header("References")]
    public Transform shootOrigin;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        float bossSpriteHalfWidth = 0f;

        if (spriteRenderer != null)
        {
            bossSpriteHalfWidth = spriteRenderer.bounds.size.x / 2f;
        }
        
        if (Camera.main != null)
        {
            Camera mainCam = Camera.main;
            float zCamDistance = Mathf.Abs(mainCam.transform.position.z - transform.position.z);

            float edgeX = mainCam.ViewportToWorldPoint(new Vector3(1, 0, zCamDistance)).x;
            leftBound = -edgeX + bossSpriteHalfWidth;
            rightBound = edgeX - bossSpriteHalfWidth;
        }

        StartCoroutine(BulletHellRoutine());
        StartCoroutine(MovementRoutine());

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetBossHealthUI(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetBossHealthUI(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        Debug.Log("Boss Defeated!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerVictory();
        }

        gameObject.SetActive(false);
    }

    private IEnumerator MoveToX(float targetX)
    {
        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            float nextX = Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator MovementRoutine()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            yield return StartCoroutine(MoveToX(leftBound));
            

            yield return StartCoroutine(MoveToX(rightBound));
            
        }
    }

    private IEnumerator BulletHellRoutine()
    {
        while (true)
        {
            // Phase 1: 360-degree Ring Burst
            yield return StartCoroutine(Pattern_RingBurst(16, 5f));
            yield return new WaitForSeconds(1.5f);

            // Phase 2: Rotating 4-Arm Spiral
            yield return StartCoroutine(Pattern_Spiral(4, 6f, 3f));
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Pattern_RingBurst(int bulletCount, float speed)
    {
        float angleStep = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));

            GameObject bullet = BulletPool.Instance.GetBullet(BulletPool.BulletType.Enemy);
            if (bullet != null)
            {
                bullet.transform.position = shootOrigin.position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.GetComponent<Bullet>().Initialize(dir, speed, false);
            }
        }
        yield return null;
    }

    private IEnumerator Pattern_Spiral(int arms, float speed, float duration)
    {
        float timer = 0f;
        float angleOffset = 0f;

        while (timer < duration)
        {
            for (int i = 0; i < arms; i++)
            {
                float currentAngle = (angleOffset + (i * (360f / arms))) * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));

                GameObject bullet = BulletPool.Instance.GetBullet(BulletPool.BulletType.Enemy);
                if (bullet != null)
                {
                    bullet.transform.position = shootOrigin.position;
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.GetComponent<Bullet>().Initialize(dir, speed, false);
                }
            }

            angleOffset += 15f;
            timer += 0.15f;
            yield return new WaitForSeconds(0.15f);
        }
    }
}