using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{   
    [SerializeField] private GameObject _pauseMenu;

    public void PlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        _pauseMenu.SetActive(true); 
    }

    public void ContinueButton()
    {
        Time.timeScale = 1;
        _pauseMenu.SetActive(false); 
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
