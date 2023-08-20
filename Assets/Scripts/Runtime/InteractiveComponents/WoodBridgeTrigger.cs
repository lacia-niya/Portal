using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���غ���ʾľ�ŵĴ��������ؽű�
/// </summary>
public class WoodBridgeTrigger : MonoBehaviour
{
    public GameObject woodBridge;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CubeBullet"))
        {
            woodBridge?.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CubeBullet"))
        {
            woodBridge.SetActive(false);
        }
    }
}
