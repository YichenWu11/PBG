using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    public EventSystem eventSystem;
    public GameObject StartButton;

    private void Start()
    {
        eventSystem.SetSelectedGameObject(StartButton);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level 0");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}