using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡管理器，查询当前关卡，进行场景切换等
/// </summary>
public class LevelManager : MonoBehaviour
{
    //public Action<int> afterLoadLevel;
    public int currentLevel = 0;

    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject levelManager = new GameObject("LevelManager");
                //GameObject gameManager = GameObject.Find("GameManager");
                levelManager.transform.SetParent(GameEntry.Instance.gameManager.transform);
                instance = levelManager.AddComponent<LevelManager>();
                //DontDestroyOnLoad(levelManager);
            }
            return instance;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevel = levelIndex;
        SceneManager.LoadScene(levelIndex);
        //afterLoadLevel?.Invoke(levelIndex);
    }

    public void OnGameWin()
    {
        AudioManager.PlayAudio(AudioName.GAMEWIN);

        if (currentLevel + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            ESCMenu.Instance.nextLevelButton.gameObject.SetActive(false);
        }
        ESCMenu.Instance.OpenWinMenu();
    }
}
