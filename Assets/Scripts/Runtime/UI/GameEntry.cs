using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// 持续在游戏期间存在的根游戏管理对象
/// </summary>
public class GameEntry : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject gameManager;

    public static GameEntry Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameEntry>();
                if(instance == null )
                {
                    var gameEntry = new GameObject("GameEntry");
                    instance = gameEntry.AddComponent<GameEntry>();
                }
            }

            return instance;
        }
    }

    private static GameEntry instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            gameManager = Instantiate(gameManagerPrefab, transform);
        }
    }
}
