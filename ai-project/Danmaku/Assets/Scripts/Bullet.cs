using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private bool isPlayerBullet;

    public void Initialize(Vector2 dir, float spd, bool fromPlayer)
    {
        direction = dir.normalized;
        speed = spd;
        isPlayerBullet = fromPlayer;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Recycle bullet when it leaves the camera bounds
        if (Mathf.Abs(transform.position.x) > 6f || Mathf.Abs(transform.position.y) > 6f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet && other.CompareTag("Boss"))
        {
            // Boss damage hook
            other.SendMessage("TakeDamage", 1f, SendMessageOptions.DontRequireReceiver);
            gameObject.SetActive(false);
        }
        else if (!isPlayerBullet && other.CompareTag("PlayerHitbox"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }

            gameObject.SetActive(false);
        }
    }
}