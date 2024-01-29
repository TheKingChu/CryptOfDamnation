using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterPortal : MonoBehaviour
{
    private Scene currentScene;
    private int sceneBuildIndex;

    public GameObject loadingScreen;
    public Image loadingBarFill;


    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    /// <summary>
    /// When the player goes into the portal the next scene is loaded
    /// while its loading a loadscreen appears so that the player gets
    /// a little break beofre going into the new room
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        sceneBuildIndex = currentScene.buildIndex;

        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0)
            {
                StartCoroutine(LoadSceneAsync(sceneBuildIndex + 1));
            }
        }
    }

    IEnumerator LoadSceneAsync(int sceneBuildIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneBuildIndex);

        loadingScreen.SetActive(true);

        while(!asyncLoad.isDone)
        {
            float progressValue = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            loadingBarFill.fillAmount = progressValue;

            yield return null;
        }
    }
}
