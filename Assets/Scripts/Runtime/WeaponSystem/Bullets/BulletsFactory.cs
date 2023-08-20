using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// �ӵ��������ṩ�ӵ���Ϸ����Ĵ�������ʼ�������ٵȷ���
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
    /// ����Э�̽��ӵ������ӳٻس�
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
