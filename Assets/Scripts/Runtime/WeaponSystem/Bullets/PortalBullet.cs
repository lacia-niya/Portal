using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 传送门子弹
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PortalBullet : PortalTraveller
{
    public string hitSound;
    public string portalSound;
    public Portal portal;
    public bool colorIsRed = false;
    
    private Rigidbody bulletRigidbody;

    public override void Init(Portal instance)
    {
        portal = instance;
    }

    private void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {

    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        base.Teleport(fromPortal, toPortal, pos, rot);
        bulletRigidbody.velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(bulletRigidbody.velocity));
        bulletRigidbody.angularVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(bulletRigidbody.angularVelocity));
    }

    /// <summary>
    /// 根据碰撞信息打开传送门
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // 判断碰撞对象的层是否为"PortalWall"
        if (collision.gameObject.layer == LayerMask.NameToLayer("PortalWall"))
        {
            AudioManager.PlayAudio(portalSound);

            var contact = collision.GetContact(0);
            var normal = contact.normal;
            var collisionPoint = contact.point;

            var portalWallBounds = collision.collider.bounds;
            var portalBounds = portal.GetComponent<Collider>().bounds;

            portal.transform.position = collisionPoint + normal * 0.1f;
            normal = colorIsRed ? normal : -normal;
            portal.transform.rotation = Quaternion.FromToRotation(portal.transform.forward, normal) * portal.transform.rotation;
            portal.OnTransformChanged();
            portal.wallCollider = collision.collider;

            this.gameObject.SetActive(false);
        }
        else
        {
            AudioManager.PlayAudio(hitSound);
        }
    }
}
