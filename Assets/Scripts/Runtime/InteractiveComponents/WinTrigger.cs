using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ش������ű�
/// </summary>
public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            LevelManager.Instance.OnGameWin();
        }
    }
}
