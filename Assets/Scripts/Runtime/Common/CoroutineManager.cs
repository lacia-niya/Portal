using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 协程单例管理器，负责执行和停止执行协程
/// </summary>
public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject coroutineManagerGO = new GameObject("CoroutineManager");
                //GameObject gameManager = GameObject.Find("GameManager");
                coroutineManagerGO.transform.SetParent(GameEntry.Instance.gameManager.transform);
                instance = coroutineManagerGO.AddComponent<CoroutineManager>();
                //DontDestroyOnLoad(coroutineManagerGO);
            }
            return instance;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    private void OnSceneUnLoaded(Scene scene)
    {
        EndAllCoroutines();
    }

    public void EndAllCoroutines()
    {
        StopAllCoroutines();
    }

    public Coroutine BeginRoutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }
}
