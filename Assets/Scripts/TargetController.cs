using System.Collections;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public delegate void OnDestroyCallback(GameObject gameObject);
    public OnDestroyCallback onDestroy;

    public delegate void OnDespawnCallback(GameObject gameObject);
    public OnDespawnCallback onDespawn;

    public float createdAt;

    [SerializeField] private Material onHitMaterial;
    [SerializeField] private float health = 1f;

    private bool movement = false;
    private bool movingToLeft = false;
    private float moveSpeed;
    private float minX;
    private float maxX;

    private void Update()
    {
        if (movement)
        {
            float moveX;

            if (movingToLeft)
            {
                if (transform.localPosition.x <= minX)
                {
                    // Limit reached. Start moving to right
                    movingToLeft = false;
                    return;
                }
                moveX = -(moveSpeed * Time.deltaTime);

            }
            else
            {
                if (transform.localPosition.x >= maxX)
                {
                    // Limit reached. Start moving to left
                    movingToLeft = true;
                    return;
                }
                moveX = (moveSpeed * Time.deltaTime);
            }

            transform.Translate(moveX, 0, 0, transform.parent);
        }
    }

    // Negative liveFor value to live forever.
    public void Initialize(float liveFor, OnDespawnCallback despawnCallback = null, OnDestroyCallback destroyCallback = null)
    {
        onDestroy = destroyCallback;
        onDespawn = despawnCallback;

        createdAt = Time.time;

        if (liveFor >= 0)
        {
            StartCoroutine(StartLife(liveFor));
        }
    }

    private IEnumerator StartLife(float liveFor)
    {
        yield return new WaitForSeconds(liveFor);

        if (isActiveAndEnabled)
        {
            onDespawn?.Invoke(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            onDestroy?.Invoke(gameObject);

        }
    }

    public void MoveBetween(float min, float max, float speed)
    {
        movement = true;
        movingToLeft = true;

        moveSpeed = speed;

        minX = min;
        maxX = max;

    }
}
