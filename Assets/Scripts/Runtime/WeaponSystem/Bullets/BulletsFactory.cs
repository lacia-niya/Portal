using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 子弹工厂，提供子弹游戏对象的创建、初始化、销毁等方法
/// </summary>
public class BulletsFactory
{
    public GameObject bulletPrefab;
    public Transform bulletTransform;
    public Portal portal;

    public GameObject CreateBullet()
    {
        var bullet = Object.Instantiate(bulletPrefab, bulletTransform.position, bulletTransform.rotation);
        if(portal != null)
        {
            var bulletTraveller = bullet.GetComponent<PortalTraveller>();
            bulletTraveller.Init(portal);
        }
        return bullet;
    }

    public void OnTakeFromPool(GameObject bullet)
    {
        bullet.transform.position = bulletTransform.position;
        bullet.transform.rotation = bulletTransform.rotation;
        var bulletRigidbody = bullet.GetComponent<Rigidbody>();
        if(bulletRigidbody != null)
        {
            bulletRigidbody.velocity = Vector3.zero;
            bulletRigidbody.angularVelocity = Vector3.zero;
        }
        if (portal != null)
        {
            var bulletTraveller = bullet.GetComponent<PortalTraveller>();
            bulletTraveller.Init(portal);
        }
        bullet.SetActive(true);
    }

    public void OnReturnedToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }

    public void OnDestroyPoolObject(GameObject bullet) 
    {
        if(bullet != null)
        {
            Object.Destroy(bullet);
        }
    }

    /// <summary>
    /// 利用协程将子弹对象延迟回池
    /// </summary>
    public static IEnumerator ReturnToPoolAfterDelay(IObjectPool<GameObject> bulletPool, GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bullet != null && bulletPool != null)
        {
            bulletPool.Release(bullet);
        }
    }
}
