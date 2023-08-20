using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

/// <summary>
/// 游戏启动页面的UI管理
/// </summary>
public class StartWindow : MonoBehaviour
{
    public Button[] levelButtons;

    public Button playButton;
    public Button quitButton;

    private void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i + 1;
            levelButtons[i].onClick.AddListener(() =>
            {
                AudioManager.PlayAudio(AudioName.CLICK);
                LevelManager.Instance.LoadLevel(index);
            });
        }
        playButton?.onClick.AddListener(PlayClick);
        quitButton?.onClick.AddListener(QuitClick);
    }

    private void PlayClick()
    {
        playButton?.gameObject.SetActive(false);
        AudioManager.PlayAudio(AudioName.CLICK);

        foreach (Button btn in levelButtons)
        {
            btn.gameObject.SetActive(true);
        }
    }

    private void QuitClick()
    {
        AudioManager.PlayAudio(AudioName.CLICK);
        Application.Quit();
    }
}
