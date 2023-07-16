using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using System.Web;


public class LevelSelectManager : MonoBehaviour
{

    [Header("Active Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;

    private Image buttonImage;
    private Button myButton;

    [Header("Level UI Stuff")]
    public TMP_Text levelText;
    public TMP_Text scoreText;
    private int highScore;

    public int level;

    // This object is used to load the data
    private GameData gameData;

    // Available levels will be filled to board's world
    //private Board board;


    // Start is called before the first frame update
    void Start()
    {
        
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        gameData = FindObjectOfType<GameData>();
        //board = FindObjectOfType<Board>();

        //fillLevelsToWorld();


        myButton.onClick.AddListener(OpenLevel);

        loadData();
        DecideSprite();
        SetHighScoreText();

        levelText.text = level.ToString();


        // Download the remaining levels asap.
        // ***********************************
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
        }
        else
        {
            int lvl = 11;
            string url;

            while (lvl != 26)
            {
                string level_name = lvl.ToString();
                // url string changes after level 15
                if (lvl <= 15)
                {
                    url = "https://row-match.s3.amazonaws.com/levels/RM_A" + level_name;
                }
                else
                {
                    url = "https://row-match.s3.amazonaws.com/levels/RM_B" + (lvl - 15).ToString();
                }
                StartCoroutine(DownloadFile(url, level_name));
                //Debug.Log(level_name);
                lvl++;
            }
        }
        // ***********************************


    }

    // Download file unit. To be called iteratively in co-routine in start method.
    IEnumerator DownloadFile(string url, string level_name)
    {   
        var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        //Debug.Log(uwr.url.ToString());
        string path = Path.Combine(Application.persistentDataPath + "/", level_name);
        Debug.Log(path);
        if(!Directory.Exists(path))
        {

            // This try catch is only for avoiding a dummy error on windows.
            try
            {
                uwr.downloadHandler = new DownloadHandlerFile(path);
            }
            // No need to catch any error or perform any operation
            // Because windows gives a dummy error without it despite all functionality exist.
            catch
            {}
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.LogError(uwr.error);
            //else
            //    Debug.Log("File successfully downloaded and saved to " + path);  
        }
        
    }

    void loadData()
    {
        if (gameData != null)
        {

            // Decide if level is active from saved data
            if (gameData.saveData.isActives.Length >= level)
            {
                if (gameData.saveData.isActives[level - 1] == true)
                {
                    isActive = true;
                }
                else
                {
                    isActive = false;
                }

                highScore = gameData.saveData.highScores[level - 1];
            }
            else
            {
                isActive = false;
            }
        }        
    }


    void SetHighScoreText()
    {
        scoreText.text = highScore.ToString();
    }


    void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
            scoreText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
            scoreText.enabled = false;
        }
    }

    public void OpenLevel()
    {
        PlayerPrefs.SetInt("CurrentLevel",level-1);
        SceneManager.LoadScene("LevelScene");
    }

}
