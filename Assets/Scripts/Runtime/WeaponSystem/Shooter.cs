using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Éä»÷ÎäÆ÷
/// </summary>
public class Shooter : MonoBehaviour, IWeapon
{
    public float fireCD = 0.5f;
    public float fireImpulse = 20f;
    public float bulletActiveTime = 30f;
    public GameObject bulletPrefab;
    public Transform bulletTransform;
    public Portal portal;

    private float lastFireTime;
    private bool shooting;
    private BulletsFactory bulletsFactory;
    private IObjectPool<GameObject> bulletsPool;

    private void Start()
    {
        bulletsFactory = new BulletsFactory();
        bulletsFactory.portal = portal;
        bulletsFactory.bulletPrefab = bulletPrefab;
        bulletsFactory.bulletTransform = bulletTransform;
        bulletsPool = new ObjectPool<GameObject>(bulletsFactory.CreateBullet, bulletsFactory.OnTakeFromPool,
            bulletsFactory.OnReturnedToPool, bulletsFactory.OnDestroyPoolObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireStart();
        }
    }

    private void FixedUpdate()
    {
        Shoot();
    }

    private void OnDestroy()
    {
        if (bulletsPool != null)
        {
            bulletsPool.Clear();
        }
    }

    public void Shoot()
    {
        if (shooting)
        {
            shooting = false;
            lastFireTime = Time.time;
            var bullet = bulletsPool.Get();
            var bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.AddForce(bullet.transform.forward * fireImpulse, ForceMode.Impulse);
            CoroutineManager.Instance.BeginRoutine(BulletsFactory.ReturnToPoolAfterDelay(bulletsPool, bullet, bulletActiveTime));
        }
    }

    public void FireStart()
    {
        if (Time.time - lastFireTime > fireCD)
        {
            shooting = true;
        }
    }

    public void FireEnd()
    {

    }
}
