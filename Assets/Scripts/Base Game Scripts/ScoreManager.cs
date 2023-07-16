using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;




public class ScoreManager : MonoBehaviour
{

    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public int score;

    private IDictionary<string, int> scoreDictionary = new Dictionary<string, int>()
    {
        {"Red Dot",100 },
        {"Green Dot",150 },
        {"Blue Dot",200 },
        {"Yellow Dot",250 },
    };

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        //CheckHighestScore();
    }


    // Public because score increase will be called from another script
    public void IncreaseScore(string tag)
    {
        score += scoreDictionary[tag];
    }


    void CheckHighestScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }


    // Call if necessary from end game manager
    public void UpdateHighestScore()
    {
        PlayerPrefs.SetInt("HighScore", score);
        highScoreText.text = $"{PlayerPrefs.GetInt("HighScore", 0)}";
    }

}
