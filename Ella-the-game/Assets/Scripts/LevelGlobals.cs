using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGlobals : MonoBehaviour {

	void Start () {
        GameManager.Instance.ExecuteGameStartEvent();
    }
	
	void Update () {
        
	}
    /// <summary>
    /// ////////////////////////Scene Management
    /// </summary>
    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
    }
    public void ReplayGame()
    {
        StartCoroutine("LoadAsyncScene", SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExecutePauseEvent()
    {
        GameManager.Instance.ExecuteGamePauseEvent();
    }

    public void ExecuteResumeEvent()
    {
        GameManager.Instance.ExecuteGameResumeEvent();
    }
}
