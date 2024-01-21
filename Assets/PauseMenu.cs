using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] PlayerInput input; 

    public void Pause()
    {
        input.DeactivateInput();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Resume()
    {
        input.ActivateInput();
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Toggle()
    {
        if(pauseMenu.activeSelf) {
            Resume();
        } else {
            Pause();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Score.SetScore(0);
    }

}
