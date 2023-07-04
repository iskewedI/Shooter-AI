using System.Collections;
using UnityEngine;

public class TargetController : MonoBehaviour
{


    public delegate void OnDestroyCallback(GameObject gameObject);
    public OnDestroyCallback onDestroy;

    public float createdAt;

    [SerializeField] private Material onHitMaterial;
    [SerializeField] private float health = 1f;

    //private MeshRenderer meshRenderer;
    //private Material defaultMaterial;

    private bool movement = false;
    private bool movingToLeft = false;
    private float moveSpeed;
    private float minX;
    private float maxX;

    // Start is called before the first frame update
    private void Start()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        //defaultMaterial = meshRenderer.material;
    }

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
            //transform.localPosition = new Vector3(moveX, transform.localPosition.y, transform.localPosition.z);
        }
    }

    // Negative liveFor value to live forever.
    public void Initialize(float liveFor, OnDestroyCallback destroyCallback = null)
    {
        onDestroy = destroyCallback;
        createdAt = Time.time;

        if (liveFor >= 0)
        {
            StartCoroutine(StartLife(liveFor));
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        //StartCoroutine(OnHit());

        if (health <= 0)
        {
            Die();
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

    private void Die()
    {
        onDestroy?.Invoke(gameObject);
    }

    private IEnumerator StartLife(float liveFor)
    {
        yield return new WaitForSeconds(liveFor);

        Die();
    }


    //private IEnumerator OnHit()
    //{
    //    meshRenderer.material = onHitMaterial;
    //    yield return new WaitForSeconds(0.1f); // Wait for 0.2 seconds
    //    meshRenderer.material = defaultMaterial;
    //}


}
