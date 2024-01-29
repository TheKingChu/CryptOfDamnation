using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckWinScene : MonoBehaviour
{
    private Scene currentSceneName;
    private string sceneName;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene();
        sceneName = currentSceneName.name;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneName == "WinScene")
        {
            Destroy(player);
        }
    }
}
