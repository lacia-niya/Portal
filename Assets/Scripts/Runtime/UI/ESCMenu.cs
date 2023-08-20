using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 单局内UI菜单管理
/// </summary>
public class ESCMenu : MonoBehaviour
{
    public GameObject escMenu;
    public Button retryButton;
    public Button mainMenuButton;
    public Button resumeButton;

    public GameObject winMenu;
    public Button winMainMenuButton;
    public Button nextLevelButton;

    private static ESCMenu _instance;

    public static ESCMenu Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.Find("UIManager")?.GetComponent<ESCMenu>();
            }

            return _instance;
        }
    }

    private void Start()
    {
        retryButton?.onClick.AddListener(Retry);
        mainMenuButton?.onClick.AddListener(MainMenu);
        resumeButton?.onClick.AddListener(OpenESCMenu);

        winMainMenuButton?.onClick.AddListener(MainMenu);
        nextLevelButton?.onClick.AddListener(NextLevel);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (LevelManager.Instance.currentLevel == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenESCMenu();
        }
    }

    public void OpenESCMenu()
    {
        OpenMenu(escMenu);
    }

    private void OpenMenu(GameObject menu)
    {
        AudioManager.PlayAudio(AudioName.CLICK);
        
        var active = menu.activeSelf;
        if (!active)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }

        menu.SetActive(!menu.activeSelf);
    }

    private void Retry()
    {
        AudioManager.PlayAudio(AudioName.CLICK);
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevel);
        //OpenESCMenu();
    }

    private void MainMenu()
    {
        AudioManager.PlayAudio(AudioName.CLICK);
        LevelManager.Instance.LoadLevel(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        retryButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        winMainMenuButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
        escMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    private void NextLevel()
    {
        AudioManager.PlayAudio(AudioName.CLICK);
        LevelManager.Instance.LoadLevel(LevelManager.Instance.currentLevel + 1);
        //OpenWinMenu();
    }

    public void OpenWinMenu()
    {
        OpenMenu(winMenu);
    }
}
