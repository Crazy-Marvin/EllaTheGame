using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    GameObject waitPanel;
    // Use this for initialization
    void Start () {
        waitPanel = GameObject.FindGameObjectWithTag("WaitPanel");
        waitPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void loadScene(string sceneName)
    {
        waitPanel.SetActive(true);
        StartCoroutine("loadAsyncScene", sceneName);
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    IEnumerator loadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
    }
    public void exitGame()
    {
        Application.Quit();
    }
}
