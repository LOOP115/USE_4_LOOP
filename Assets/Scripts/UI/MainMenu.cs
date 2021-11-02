using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        InGameMenu.isPause = false;
        LoadSceneAsync();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LoadSceneAsync()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
