using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingpanel;

    public void LevelLoad(int sceneIndex)
    {
        StartCoroutine(loadAsynchronosly(sceneIndex));
    }

    IEnumerator loadAsynchronosly(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingpanel.SetActive(true);

        yield return null;
    }









}

