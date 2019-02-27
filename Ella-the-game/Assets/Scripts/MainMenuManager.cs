using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuManager : MonoBehaviour {
    public GameObject waitPanel, goBtn;
    [SerializeField]
    private Button puppyBtn, adultBtn, seniorBtn;
    private int difficulty;
    public VideoPlayer videoPlayer;
    public VideoClip defaultClip;
    string selectedLevel = "";
    // Use this for initialization
    void Start () {
        GameManager.Instance.gameState = GameManager.GameState.MainMenu;
        waitPanel = GameObject.FindGameObjectWithTag("WaitPanel");
        waitPanel.SetActive(false);
        //Difficulty Settup
        difficulty = PlayerPrefs.GetInt("difficulty");
        if (difficulty == 0)
        {
            puppyBtn.GetComponent<Image>().color = new Color32(103, 0, 0, 255);
        }
        else if (difficulty == 1)
        {
            adultBtn.GetComponent<Image>().color = new Color32(103, 0, 0, 255);
        }
        else
        {
            seniorBtn.GetComponent<Image>().color = new Color32(103, 0, 0, 255);
        }
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    public void loadScene()
    {
        Time.timeScale = 1;
        StartCoroutine("loadAsyncScene", selectedLevel);
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
    public void setDifficulty(GameObject Btn)
    {
 
       
        if (Btn.tag == "puppyDiff")
        {
            PlayerPrefs.SetInt("difficulty", 0);
            puppyBtn.GetComponent<Image>().color = new Color32(103, 0, 0, 255);
            adultBtn.GetComponent<Image>().color = new Color32(24, 24, 24, 255);
            seniorBtn.GetComponent<Image>().color = new Color32(24, 24, 24, 255);
        }
        else if (Btn.tag == "adultDiff")
        {
            PlayerPrefs.SetInt("difficulty", 1);
            puppyBtn.GetComponent<Image>().color = new Color32(24, 24, 24, 255);
            adultBtn.GetComponent<Image>().color = new Color32(103, 0, 0, 255);
            seniorBtn.GetComponent<Image>().color = new Color32(24, 24, 24, 255);
        }
        else
        {
            PlayerPrefs.SetInt("difficulty", 2);
            puppyBtn.GetComponent<Image>().color = new Color32(24, 24, 24, 255);
            adultBtn.GetComponent<Image>().color = new Color32(24, 24, 24, 255);
            seniorBtn.GetComponent<Image>().color = new Color32(103, 0, 0, 255);
        }
    }
    public void SelectLevel(string sceneName)
    {
        selectedLevel = sceneName;
        goBtn.SetActive(true);
        
    }
    public void PlayDemoVideo(VideoClip clip)
    {
        videoPlayer.clip = clip;
    }
}
