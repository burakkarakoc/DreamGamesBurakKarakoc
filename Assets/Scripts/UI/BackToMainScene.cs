using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BackToMainScene : MonoBehaviour
{

    private GameData gameData;
    private Board board;
    private ScoreManager smanager;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
        smanager = FindObjectOfType<ScoreManager>();
    }

    public void WinOK()
    {
        if (gameData != null)
        {
            // Unlock the upcoming level
            gameData.saveData.isActives[board.level + 1] = true;

            // Update the highscore if necessary
            if (smanager.score > gameData.saveData.highScores[board.level])
            {
                gameData.saveData.highScores[board.level] = smanager.score;
            }

            // Save that level game data
            gameData.Save();
        }

        // Pop back to main scene
        SceneManager.LoadScene("MainScene");
    }


    // Do not update anything, just pop back to main scene
    public void LoseOK()
    {
        SceneManager.LoadScene("MainScene");
    }
}
