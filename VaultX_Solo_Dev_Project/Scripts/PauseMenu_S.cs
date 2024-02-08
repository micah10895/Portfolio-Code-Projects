using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu_S : MonoBehaviour
{

    public Canvas pauseMenu;
    public bool isPaused;

    // Start is called before the first frame update
    private void Start()
    {
        pauseMenu = GetComponent<Canvas>();
        pauseMenu.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.enabled = true;
        Time.timeScale = 0f; 
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void MainMenuQuit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
