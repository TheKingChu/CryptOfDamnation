using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public GameObject options, paused;
    public Slider slider;
    public GameObject music;
    private AudioSource source;

    private GameObject player;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        music = GameObject.FindGameObjectWithTag("MusicManager");
        source = music.GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        //freeze the game and open the pausemenu object
        Time.timeScale = 0f;
        paused.SetActive(true);
    }

    /// <summary>
    /// Lets player resume the game by clicking the resume button
    /// </summary>
    public void ResumeGame()
    {
        //unfreeze the game and destroy the pausemenu object
        Time.timeScale = 1.0f;
        Destroy(gameObject);
        //TODO PlayerPrefs.Save();
    }

    /// <summary>
    /// Opens the option menu which will contain:
    /// volume sliders
    /// dmg popup disable/enable
    /// adjust brightness?
    /// </summary>
    public void Options()
    {
        paused.SetActive(false);
        options.SetActive(true);

        slider.value = source.volume; //PlayerPrefs.GetFloat("MusicVolume", 0.75f); //getting the default value

        //TODO dmg popup enable/disable
        //TODO let players adjust the brightness
    }

    /// <summary>
    /// In the option menu part the player is able to adjust volumes
    /// right now its only music volume
    /// </summary>
    public void AdjustVolume()
    {
        float sliderValue = slider.value;
        if (music)
        {
            source.volume = sliderValue;
        }
        //TODO PlayerPrefs.SetFloat("MusicVolume", sliderValue); //for saving the new value
        //TODO add options for changing values of SFX
    }

    public void Back()
    {
        options.SetActive(false);
        paused.SetActive(true);
    }

    /// <summary>
    /// When in pause menu and player clicks quit button
    /// it sends the player to main menu
    /// </summary>
    public void PausedSendToMenu()
    {
        //destroy player so that we dont get multiple of them in future runs 
        //need to be able to save player stats before destroying that instance???
        Destroy(player);
        //destroys pausemenu object
        Destroy(gameObject);
        //setting the time back to 1 so that when you start a new run it wont be freezed
        Time.timeScale = 1.0f;
        //loads the main menu scene
        SceneManager.LoadScene(0);
    }
}
