using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Music Manager for background music in scene
/// </summary>
public class MusicManager : MonoBehaviour
{
    private Scene currentScene;
    public AudioSource audioSource;
    public AudioClip[] clips;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        string sceneName = currentScene.name;
        int index = currentScene.buildIndex;

        switch (index)
        {
            case 0:
                if(sceneName == "Level1" && !audioSource.isPlaying)
                {
                    audioSource.clip = clips[0];
                    audioSource.Play();
                }
            break;

            case 1:
                if(sceneName == "Level2" && !audioSource.isPlaying)
                {
                    audioSource.clip = clips[1];
                    audioSource.Play();
                }
            break;

            case 2:
                if(sceneName == "Level3" && !audioSource.isPlaying)
                {
                    audioSource.clip = clips[2];
                    audioSource.Play();
                }
            break;

            case 3:
                if( sceneName == "Level4" && !audioSource.isPlaying)
                {
                    audioSource.clip = clips[3];
                    audioSource.Play();
                }
                break;
        }
    }
}
