using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器系统管理器，添加、切换武器等
/// </summary>
public class WeaponManager : MonoBehaviour
{
    public List<GameObject> weapons;
    public int currentWeaponIndex = 1;

    private void Start()
    {
        if (currentWeaponIndex < weapons.Count)
        {
            SwitchWeapon(currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (ESCMenu.Instance != null)
        {
            if (ESCMenu.Instance.escMenu.activeSelf)
            {
                weapons[currentWeaponIndex].SetActive(false);
                return;
            }
            else
            {
                weapons[currentWeaponIndex].SetActive(true);
            }
        }
        
        for (int i = 0; i < weapons.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchWeapon(i);
                break;
            }
        }
    }

    private void SwitchWeapon(int index)
    {
        if(index >= weapons.Count) 
        {
            return;
        }
        
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = index;
        weapons[currentWeaponIndex].SetActive(true);
    }
}
