using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject menuGO;
    public GameObject quitGO;

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        menuGO.SetActive(false);
        quitGO.SetActive(true);
    }

    public void QuitYes()
    {
        Application.Quit();
    }

    public void QuitNo()
    {
        quitGO.SetActive(false);
        menuGO.SetActive(true);
    }
}