using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Vector3 direction;

    private Rigidbody rb;

    public float damage = 1;
    public float speed = 10f;
    public float range = 300f; // Maximum distance the skillshot can travel

    public delegate void OnDestroy(GameObject gameObject);
    private OnDestroy onDestroy;

    // On Bullet Hit Event
    public delegate void BulletHitHandler(float timeTaken);
    public static event BulletHitHandler OnBulletHit;

    // On Bullet Hit Event
    public delegate void BulletMissHandler();
    public static event BulletMissHandler OnBulletMiss;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction, OnDestroy onDestroy)
    {
        this.direction = direction;
        this.onDestroy = onDestroy;
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = rb.position + speed * Time.fixedDeltaTime * direction;
        rb.MovePosition(newPosition);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<TargetController>(out var targetController))
        {
            //Debug.Log("Collider hit target");
            float timeTaken = Time.time - targetController.createdAt;

            targetController.TakeDamage(damage);

            OnBulletHit(timeTaken);
            onDestroy?.Invoke(gameObject);

            return;
        }

        OnBulletMiss();
        onDestroy?.Invoke(gameObject);
    }
}
