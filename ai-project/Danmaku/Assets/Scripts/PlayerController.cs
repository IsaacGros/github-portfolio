using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 7f;
    public float focusSpeed = 3f;
    public Vector2 boundaryMin = new Vector2(-3.5f, -4.5f);
    public Vector2 boundaryMax = new Vector2(3.5f, 4.5f);

    [Header("Combat & Visuals")]
    public GameObject hitboxIndicator;
    public Transform firePoint;
    public float fireRate = 0.08f;

    private SpriteRenderer hitboxRenderer;
    private float nextFireTime = 0f;

    void Start()
    {
        if (hitboxIndicator != null)
        {
            hitboxRenderer = hitboxIndicator.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;

        Vector2 moveDir = new Vector2(moveX, moveY).normalized;

        bool isFocused = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        float currentSpeed = isFocused ? focusSpeed : normalSpeed;

        if (hitboxRenderer != null)
        {
            hitboxRenderer.enabled = isFocused;
        }

        Vector3 newPos = transform.position + (Vector3)(moveDir * currentSpeed * Time.deltaTime);
        newPos.x = Mathf.Clamp(newPos.x, boundaryMin.x, boundaryMax.x);
        newPos.y = Mathf.Clamp(newPos.y, boundaryMin.y, boundaryMax.y);
        transform.position = newPos;
    }

    private void HandleShooting()
    {
        bool shootPressed = Keyboard.current.zKey.isPressed || Keyboard.current.spaceKey.isPressed;

        if (shootPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            GameObject bullet = BulletPool.Instance.GetBullet(BulletPool.BulletType.Player);
            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.GetComponent<Bullet>().Initialize(Vector2.up, 18f, true);
            }
        }
    }
}