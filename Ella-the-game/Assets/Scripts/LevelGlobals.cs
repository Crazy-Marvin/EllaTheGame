using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGlobals : MonoBehaviour {
    private int gameSpeed;
    private int score;
	// Use this for initialization
	void Start () {
        gameSpeed = 1;
    }
	
	// Update is called once per frame
	void Update () {
  
          Time.timeScale = gameSpeed;
        
	}
    public void setScore(int score)
    {
        this.score = score;

    }
    public int getScore()
    {
        return this.score;
    }
    public void toggleGamePaused()
    {
        if (gameSpeed == 1)
        {
            gameSpeed = 0;
        }
        else
        {
            gameSpeed = 1;
        }
    }
    public bool isGamePaused()
    {
        if (gameSpeed == 1)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }
    public void exitGame()
    {
        Application.Quit();
    }
    public void loadMainMenu()
    {

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    IEnumerator loadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
    }
    public void replay()
    {
        StartCoroutine("loadAsyncScene", SceneManager.GetActiveScene().name);
    }
    public void saveScore()
    {
        var tmpScore = PlayerPrefs.GetInt("playerGlobalScore");
        tmpScore += this.score;
        PlayerPrefs.SetInt("playerGlobalScore", tmpScore);
    }

}
