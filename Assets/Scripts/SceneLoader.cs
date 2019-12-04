using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    private GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void LoadGameWindow()
    {
        SceneManager.LoadScene("Game Window");
    }

    public void LoadMainMenu()
    {
        gm.SafeGameAbort();
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
