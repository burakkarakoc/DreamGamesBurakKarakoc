using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class EndGameManager : MonoBehaviour
{

    // Need to be reached from dot class to update
    public TMP_Text movesText;
    private int moves;

    private ScoreManager smanager;
    private Board board;

    private BackToMainScene back;

    // Panel is given as parameter in the editor
    public GameObject celebrationPanel;

    // Game data of the user for that level to set win conditions
    private GameData gameData;
    private int highestScoreBefore = 0;

    // Two conditions for finish a game;
    // isDeadLock will be updated in dot class to check whether a logical move left to match a row to finish the game here in end game manager.
    public bool isDeadlock = false;
    private bool lastMove = false;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        smanager = FindObjectOfType<ScoreManager>();
        back = FindObjectOfType<BackToMainScene>();
        gameData = FindObjectOfType<GameData>();
        SetMaxMove();
        loadHighScoreBefore();
    }

    // Update is called once per frame
    void Update()
    {
        movesText.text = moves.ToString();

        if (moves == 0)
        {
            lastMove = true;
        }
    }


    // Set max move according to a level
    private void SetMaxMove()
    {
        if (board.world != null)
        {
            if (board.world.levels[board.level] != null)
            {
                moves = board.world.levels[board.level].maxMoves;
            }
        }
    }

    void loadHighScoreBefore()
    {

        if (gameData.saveData != null)
        {
            // Decide if level is active from saved data
            if (gameData.saveData.isActives.Length >= board.level)
            {
                if (gameData.saveData.isActives[board.level] == true)
                {
                    highestScoreBefore = gameData.saveData.highScores[board.level ];
                }
                else
                {
                    highestScoreBefore = 0;
                }
            }
        }
    }


    public void IsLevelFinished()
    {
        // If moves ran out, or there are no logical move left, finish the game in an appropriate way.
        if (lastMove || isDeadlock)
        {
            
            if (smanager.score > highestScoreBefore)
            {
                // Update high score
                smanager.UpdateHighestScore();

                // Update the game state
                board.currentState = GameState.win;

                // Show celebration panel
                celebrationPanel.SetActive(true);
            }

            // If level losed
            else
            { 
                Debug.Log("Game Lose");
                Debug.Log(smanager.score);
                Debug.Log("***********");
                Debug.Log("Pass the highest score of: " + PlayerPrefs.GetInt("HighScore", 0));
                board.currentState = GameState.lose;
                back.LoseOK();
            }
        }
    }


    // Should be reached from dot class and decremented by 1 if a move happens
    public void DecrementMove()
    {
        --moves;
        if (moves < 1)
        {
            moves = 0; // Reset the seen remaining moves to 0 instead of negative
        }
    }
}