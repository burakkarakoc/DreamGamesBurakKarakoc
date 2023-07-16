using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum GameState
/*
 * Is used to ensure game is not playable in the check process, 
 * namely, swapping one after another without detection process.
 * 
 * Also used for stop the game after movement is finished.
 */
{
    wait,
    move,
    win,
    lose
}


public class Board : MonoBehaviour
{

    [Header("Level Stuff")]
    public World world;
    public int level;

    // Sprite to be changed to if a row matched
    public Sprite mSprite;

    // Should be reached from Dot class to see wheter a movement can be done or not
    public GameState currentState = GameState.move;

    [Header("Board Dimensions")]
    public int width; // Width of the board
    public int height; // Height of the board

    //private EndGameManager endGameManager;
    private int maxMoves;

    [Header("Prefabs")]
    public GameObject TilePrefab; // TilePrefab will be used to fill the background of the tiles of the board
    private BackgroundTile[,] allTiles;


    public GameObject[] dots;
    public GameObject[,] allDots;
    private int[,] dotsArr;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            level = PlayerPrefs.GetInt("CurrentLevel");
        }
        if (world != null)
        {
            world.fillLevelsToWorld();
            if (world.levels[level] != null)
            {
                width = world.levels[level].width;
                height = world.levels[level].height;
                maxMoves = world.levels[level].maxMoves;
                dotsArr = world.levels[level].Make2DArray();
            }
        }
        else
        {
            Debug.Log("World could not be found!!!!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];

        Setup();
    }

    // Setup process for setting up the board
    private void Setup()
    {
        // Loop over 2-D board to instantiate the board objects
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 Position = new Vector2(i, j);

                GameObject backgroundTile = Instantiate(TilePrefab, Position, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + "," + j + ")";

                int dotToUse = dotsArr[j, i];

                // This block is for random geenration of candies without any 3 adjacent match.
                //int dotToUse = Random.Range(0, dots.Length);
                //// Creating a board without matches
                //int maxIter = 0; // check for infinite loop
                //while (MatchesAt(i, j, dots[dotToUse]) && maxIter <= 100)
                //{
                //    dotToUse = Random.Range(0, dots.Length);
                //    maxIter++;
                //}
                //maxIter = 0;
                //Debug.Log(dots[dotToUse]);

                GameObject dot = Instantiate(dots[dotToUse], Position, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "(" + i + "," + j + ")";
                allDots[i, j] = dot;
            }
        }
    }


    public void Update()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j].GetComponent<Dot>().isMatched)
                {
                    SpriteRenderer matchSprite = allDots[i, j].GetComponent<Dot>().GetComponent<SpriteRenderer>();
                    matchSprite.sprite = mSprite;
                    matchSprite.color = new Color(0, 1, 1, 1);
                }
            }
        }
    }
}
//    private bool MatchesAt(int column, int row, GameObject piece)
//    /*
//     * 
//     * This function ensures that there won't be any 3 adjacent same items 
//     * will occur in the board to increase player experience.
//     * 
//     * args:
//     * 
//     * int column => i location
//     * int row => j location
//     * GameObject piece => piece we are checking
//     * 
//     * returns: 
//     * 
//     * if a match exits => true 
//     * otherwise => false 
//     * 
//     */
//    {
//        // Normal case
//        if (column > 1 && row > 1)
//        {
//            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
//            {
//                return true;
//            }
//            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2 ].tag == piece.tag)
//            {
//                return true;
//            }
//        }

//        // Edge case
//        else if (column <= 1 || row <= 1)
//        {

//            if (row > 1)
//            {
//                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
//                {
//                    return true;
//                }
//            }
//            if (column > 1)
//            {
//                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
//                {
//                    return true;
//                }
//            }

//        }
//        return false;
//    }
//}
