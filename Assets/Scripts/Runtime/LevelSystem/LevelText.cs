using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 关卡内提示文本的配置和间隔显示脚本
/// </summary>
public class LevelText : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public List<TextAsset> textAssets;
    public float displayTime = 5f;
    public GameObject HUD;

    private void Start()
    {
        //LevelManager.Instance.afterLoadLevel += (i) => CoroutineManager.Instance.BeginRoutine(ShowText(i));
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (LevelManager.Instance.currentLevel > 0)
        {
            HUD?.gameObject.SetActive(true);
            CoroutineManager.Instance.BeginRoutine(ShowText(LevelManager.Instance.currentLevel));
        }
        else
        {
            HUD?.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator ShowText(int levelindex)
    {
        string[] levelText = textAssets[levelindex - 1].text.Split('\n');

        for (int i = 0; i < levelText.Length; i++)
        {
            displayText.text = levelText[i];

            yield return new WaitForSeconds(10f);
        }
    }
}
