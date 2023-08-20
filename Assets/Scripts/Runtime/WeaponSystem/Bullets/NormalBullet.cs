using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���л�����������(����)����ͨ�ӵ�
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class NormalBullet : PortalTraveller
{
    public string hitSound;
    
    private Rigidbody bulletRigidbody;

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

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.PlayAudio(hitSound);
    }
}
