using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("HuntGame");
    }

    public void ExitGame()
    {
        Debug.Log("Game is exiting...");

        Application.Quit();
    }
}