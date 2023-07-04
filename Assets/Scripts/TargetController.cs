using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private Material onHitMaterial;
    [SerializeField] private float health = 1;
    //private MeshRenderer meshRenderer;
    //private Material defaultMaterial;

    public delegate void OnDestroyCallback(GameObject gameObject);
    public OnDestroyCallback onDestroy;

    public float createdAt;

    // Start is called before the first frame update
    private void Start()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        //defaultMaterial = meshRenderer.material;
    }

    // Negative liveFor value to live forever.
    public void Initialize(float liveFor, OnDestroyCallback destroyCallback = null)
    {
        onDestroy = destroyCallback;
        createdAt = Time.time;

        if(liveFor >= 0)
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
