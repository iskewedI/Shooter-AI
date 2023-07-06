using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private bool manualControl = false;
    [SerializeField] private float bulletSpeed = 1f;

    public float damage = 1f;

    private Camera cam;
    //private ObjectPooler pooler;

    private void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        //pooler = GetComponent<ObjectPooler>();  
    }

    private void Update()
    {
        //if (manualControl && Input.GetMouseButtonDown(0))
        //{
        //    FireBullet();
        //}
    }


    //public void FireBullet()
    //{
    //    Debug.Log("Actual FireBullet method");
    //    GameObject bullet = pooler.SpawnFromPool(bulletSpawnPoint.position, cam.transform.rotation);

    //    BulletController bulletController = bullet.GetComponent<BulletController>();
    //    bulletController.speed = bulletSpeed;
    //    bulletController.damage = damage;

    //    bulletController.Initialize(cam.transform.forward, (GameObject obj) => pooler.ReturnToPool(obj));
    //}


}
