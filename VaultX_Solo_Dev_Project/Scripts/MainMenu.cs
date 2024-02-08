using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MainMenu_()
    {
        SceneManager.LoadScene(0);

    }

    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void LevelOne()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);

    }

    public void LevelTwo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(3);

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
