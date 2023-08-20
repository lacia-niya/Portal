using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 过关触发器脚本
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
