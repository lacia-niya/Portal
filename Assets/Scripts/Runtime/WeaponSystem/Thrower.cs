using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ø…–Ó¡¶Õ∂÷¿Œ‰∆˜
/// </summary>
public class Thrower : MonoBehaviour, IWeapon
{
    public float fireCD = 0.2f;
    public float maxDuration = 3f;
    public float minDuration = 0.5f;
    public float fireImpulse = 20f;
    public float bulletActiveTime = 30f;
    public GameObject bulletPrefab;
    public Transform bulletTransform;
    public Portal portal;

    private float lastFireTime;
    private float startFiretime;
    private bool shooting;
    private bool startFire;
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
        else if (Input.GetMouseButtonUp(0))
        {
            FireEnd();
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
            startFire = false;
            lastFireTime = Time.time;
            var bullet = bulletsPool.Get();
            var bulletRigidbody = bullet.GetComponent<Rigidbody>();
            var duration = Mathf.Clamp(Time.time - startFiretime, minDuration, maxDuration);
            var finalImpulse = fireImpulse / maxDuration * duration;
            bulletRigidbody.AddForce(bullet.transform.forward * finalImpulse, ForceMode.Impulse);
            CoroutineManager.Instance.BeginRoutine(BulletsFactory.ReturnToPoolAfterDelay(bulletsPool, bullet, bulletActiveTime));
        }
    }

    public void FireStart()
    {
        if (Time.time - lastFireTime > fireCD)
        {
            startFire = true;
            startFiretime = Time.time;
        }
    }

    public void FireEnd()
    {
        if (startFire)
        {
            shooting = true;
        }
    }
}
