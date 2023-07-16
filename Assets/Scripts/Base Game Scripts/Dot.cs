using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum DotState
/*
 * Is used to ensure game is not playable in the check process, 
 * namely, swapping one after another without detection process.
 * 
 * Also used for stop the game after movement is finished.
 */
{
    normal,
    matched
}

public class Dot : MonoBehaviour
{
    // Variable declarations
    [Header("Variables")]
    private Vector2 firstTouch;
    private Vector2 lastTouch;

    private DotState dotState = DotState.normal;

    public bool isMatched = false;

    public float swipeAngle = 0;
    public float swipeResist = 1f; // To check a swipe is big enough to increase player experience.

    public int row;
    public int column;
    public int targetX;
    public int targetY;
    
    private Board board;
    private GameObject otherDot;
    private Vector2 tempTargetPos;

    //public Sprite mSprite;

    private ScoreManager scoreManager;
    private EndGameManager egManager;

    private static bool[] matchedRows;

    private int deadBoards = 0;



    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        scoreManager = FindObjectOfType<ScoreManager>();
        egManager = FindObjectOfType<EndGameManager>();

        matchedRows = new bool[board.height];

        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }


    // Update is called once per frame
    void Update()
        /*
         * Match finding is not inside Update method, which is extremely memory intensive, 
         * instead I have implemented it in a co-routine only called after if a swap happened.
         * 
         * This way the game is much more efficient and less resource dependent...
         */
    {

        // Displays matches on the scene
        if (isMatched)
        {
            this.dotState = DotState.matched;

            //SpriteRenderer matchSprite = this.GetComponent<Dot>().GetComponent<SpriteRenderer>();
            //matchSprite.sprite = mSprite;

            //matchSprite.color = new Color(0f, 0f, 0f,0f);
        }

        // Target positions set for change operation
        targetX = column;
        targetY = row;

        // Horizontal movement
        if (Mathf.Abs(targetX - transform.position.x) > .1) // Move towards the target if a swipe is big enough
        {
            tempTargetPos = new Vector2(targetX, transform.position.y);
            //transform.position = Vector2.Lerp(transform.position, tempTargetPos, .4f);

            if (transform != null)
            {
                transform.DOMove(tempTargetPos, .5f);
            }
            
        }
        else // Directly set the position
        {
            tempTargetPos = new Vector2(targetX, transform.position.y);
            transform.position = tempTargetPos;
            board.allDots[column, row] = this.gameObject;
        }

        // Vertical movement
        if (Mathf.Abs(targetY - transform.position.y) > .1) // Move towards the target if a swipe is big enough
        {
            tempTargetPos = new Vector2(transform.position.x, targetY);
            //transform.position = Vector2.Lerp(transform.position, tempTargetPos, .4f);

            if (transform != null)
            {
                transform.DOMove(tempTargetPos, .5f);
            }

        }

        else // Directly set the position
        {
            tempTargetPos = new Vector2(transform.position.x, targetY);
            transform.position = tempTargetPos;
            board.allDots[column, row] = this.gameObject;
        }
    }


    // 
    private IEnumerator CheckMoveCo()
    /*
     * This co-routine ensures an efficient game, it replaces match finding from update method and 
     * in turn, with a logic, match finding operation is only looked for when a swap performed.
     * 
     * For further seperation of classes, match finder could be another class that 
     * reaches to public allDots list of the board class and could perform in the same way. 
     * However, for simplicity and since it is only one function, I wrote this here as well. 
     * 
     * Apart from efficiency, it is to check the row matches after a swap happened.
     * Ensures a matched item of a row can not be moved in any way.
     * Called after a piece moved(check MovePiece() method).
     */
    {
        yield return new WaitForSeconds(.2f);


        // Namely, if there is a swap, and the swap cannot be performed if any of side is matched
        if (otherDot != null && this.dotState == DotState.normal && otherDot.GetComponent<Dot>().dotState == DotState.normal)
        {

            // Initialize them as true.
            // The logic is if there is no item that breaks the rule of match, than that row is matched.
            bool rowMatchAtMovedDot = true;
            bool rowMatchAtOtherDot = true;

            int otherDotsRow = otherDot.GetComponent<Dot>().row;

            // Iterate along the row to check whether a row match or not
            for (int i = 0; i < board.width; i++)
            {
                
                // Check for the actual dots row
                if(board.allDots[i,row].tag != tag)
                {
                    rowMatchAtMovedDot = false;
                }

                // Check for the other dots row
                if (board.allDots[i, otherDotsRow].tag != otherDot.tag)
                {
                    rowMatchAtOtherDot = false;
                }
            }

            // If row match happened in the row of actual swiped dot
            if (rowMatchAtMovedDot)
            {

                //this.dotState = DotState.matched;
                scoreManager.IncreaseScore(tag);

                // Initialize all dots in that row as matched
                for (int i = 0; i < board.width; i++)
                {
                    board.allDots[i, row].GetComponent<Dot>().isMatched = true;
                }

                matchedRows[targetY] = true;

                //bool[] array = { false, false, true, false, false, true, false };
                List<List<int>> gaps = FindGaps(matchedRows); // list donduruyo

                foreach (List<int> gap in gaps)
                {

                    if (DeadlockDetector(gap))
                    {
                        deadBoards++;
                    }

                }

                Debug.Log(deadBoards);
                Debug.Log(gaps.Count);

                if (deadBoards == gaps.Count)
                {
                    egManager.isDeadlock = true;
                }

                //Debug.Log(targetX);
                //Debug.Log(targetY);
                Debug.Log("ROWMATCH @ " + row + " MOVED DOT!!");
            }


            // If row match happened in the row of other dot swapped with
            if (rowMatchAtOtherDot)
            {

                //otherDot.GetComponent<Dot>().dotState = DotState.matched;
                scoreManager.IncreaseScore(otherDot.tag);

                // Initialize all dots in that row as matched
                for (int i = 0; i < board.width; i++)
                {
                    board.allDots[i, otherDotsRow].GetComponent<Dot>().isMatched = true;
                }

                matchedRows[otherDotsRow] = true;

                //bool[] array = { false, false, true, false, false, true, false };
                List<List<int>> gaps = FindGaps(matchedRows); // list donduruyo

                foreach (List<int> gap in gaps)
                {

                    if (DeadlockDetector(gap))
                    {
                        deadBoards++;
                    }

                }

                if (deadBoards == gaps.Count)
                {
                    egManager.isDeadlock = true;
                }

                Debug.Log(deadBoards);
                Debug.Log(gaps.Count);

                //Debug.Log(otherDotsRow);
                //int otherDotscolumn = otherDot.GetComponent<Dot>().column;
                //Debug.Log(otherDotscolumn);
                Debug.Log("ROWMATCH @ " + otherDotsRow + " OTHER DOT!!");
            }

            // Release the reference to other dot since the movement is done
            otherDot = null;
        }

        // Reset the game state to playable.
        yield return new WaitForSeconds(.2f);
        board.currentState = GameState.move;
        egManager.IsLevelFinished();
    }


    // Function that looks for deadlock between matched and given y value.
    private bool DeadlockDetector(List<int> gap)
    {
        int red_ct = 0;
        int blue_ct = 0;
        int yellow_ct = 0;
        int green_ct = 0;

        for (int i = gap[0]; i <= gap[gap.Count-1]; i++)
        {
            for (int j = 0; j < board.width; j++)
            {
                if (board.allDots[j, i].tag == "Blue Dot")
                {
                    blue_ct++;
                }
                else if (board.allDots[j, i].tag == "Red Dot")
                {
                    red_ct++;
                }
                else if (board.allDots[j, i].tag == "Green Dot")
                {
                    green_ct++;
                }
                else if (board.allDots[j, i].tag == "Yellow Dot")
                {
                    yellow_ct++;
                }
            }
        }

        if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
        {
            //egManager.isDeadlock = true;
            return true;
        }
        else
        {
            red_ct = 0;
            blue_ct = 0;
            yellow_ct = 0;
            green_ct = 0;
            return false;
        }

        /*
        // Edge case
        if (targetY == 0)
        {

            // Upper check
            for (int i = targetY+1; i < boardY; i++)
            {
                for (int j = 0; j < board.width; j++)
                {
                    if (board.allDots[j, i].tag == "Blue Dot")
                    {
                        blue_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Red Dot")
                    {
                        red_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Green Dot")
                    {
                        green_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Yellow Dot")
                    {
                        yellow_ct++;
                    }
                }
            }
            if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
            {
                logicalMoveLeftUpper = false;
                egManager.isDeadlock = true;
            }
            else
            {
                red_ct = 0;
                blue_ct = 0;
                yellow_ct = 0;
                green_ct = 0;
            }

        }

        // Other edge case 
        else if (targetY == board.height - 1)
        {
            // Lower check
            for (int i = boardY; i < targetY; i++)
            {
                for (int j = 0; j < board.width; j++)
                {
                    if (board.allDots[j, i].tag == "Blue Dot")
                    {
                        blue_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Red Dot")
                    {
                        red_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Green Dot")
                    {
                        green_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Yellow Dot")
                    {
                        yellow_ct++;
                    }
                }
            }
            if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
            {
                logicalMoveLeftUpper = false;
                egManager.isDeadlock = true;
            }
            else
            {
                red_ct = 0;
                blue_ct = 0;
                yellow_ct = 0;
                green_ct = 0;
            }

        }

        // Normal Case
        else
        {
            // Upper check
            for (int i = targetY+1; i < boardY; i++)
            {
                for (int j = 0; j < board.width; j++)
                {
                    if (board.allDots[j, i].tag == "Blue Dot")
                    {
                        blue_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Red Dot")
                    {
                        red_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Green Dot")
                    {
                        green_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Yellow Dot")
                    {
                        yellow_ct++;
                    }
                }
            }
            if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
            {
                logicalMoveLeftUpper = false;
                red_ct = 0;
                blue_ct = 0;
                yellow_ct = 0;
                green_ct = 0;
            }

            else
            {
                Debug.Log(red_ct + " " + blue_ct + " " + yellow_ct + " " + green_ct);
                red_ct = 0;
                blue_ct = 0;
                yellow_ct = 0;
                green_ct = 0;
            }

            // Lower check
            for (int i = boardY; i < targetY; i++)
            {
                for (int j = 0; j < board.width; j++)
                {
                    if (board.allDots[j, i].tag == "Blue Dot")
                    {
                        blue_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Red Dot")
                    {
                        red_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Green Dot")
                    {
                        green_ct++;
                    }
                    else if (board.allDots[j, i].tag == "Yellow Dot")
                    {
                        yellow_ct++;
                    }
                }
            }
            if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
            {
                logicalMoveLeftLower = false;
                red_ct = 0;
                blue_ct = 0;
                yellow_ct = 0;
                green_ct = 0;
            }
            else
            {
                red_ct = 0;
                blue_ct = 0;
                yellow_ct = 0;
                green_ct = 0;
            }
            if (!logicalMoveLeftUpper && !logicalMoveLeftLower)
            {
                egManager.isDeadlock = true;
            }
        }
        */
    }


    public static List<List<int>> FindGaps(bool[] arr)
    {
        List<List<int>> gaps = new List<List<int>>();
        List<int> currentGap = new List<int>();

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i])
            {
                if (currentGap.Count > 0)
                {
                    gaps.Add(new List<int>(currentGap));
                    currentGap.Clear();
                }
            }
            else
            {
                currentGap.Add(i);
            }
        }

        if (currentGap.Count > 0)
        {
            gaps.Add(new List<int>(currentGap));
        }

        return gaps;
    }



    // Obtains first touch position
    private void OnMouseDown()
    {
        // Record the touch to start the movement process only if the game is playable at that time.
        if (board.currentState == GameState.move)
        {
            firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    // Obtains last touch position
    private void OnMouseUp()
    {
        // Only record the last touch if the game is playable at that time (for consistency).
        if (board.currentState == GameState.move)
        {
            lastTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle(); // Obtain swipe angle to perform swap operation in appropriate direction
        }
    }


    // Calculates angle and performs swap operation accordingly
    void CalculateAngle()
    {
        if (Mathf.Abs(lastTouch.y - firstTouch.y) > swipeResist || Mathf.Abs(lastTouch.x - firstTouch.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(lastTouch.y - firstTouch.y, lastTouch.x - firstTouch.x) * Mathf.Rad2Deg;
            if (board.currentState == GameState.move)
            {
                MovePieces(); // Swap operation
            }
            board.currentState = GameState.wait;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }


    // Swap operations
    void MovePieces()
    {

        // if current tapped dot is not a matched one. wrap this to whole!!!!
        if (!isMatched)
        {

            // Right swipe
            if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
            {
                // Other Dot's positioning process
                otherDot = board.allDots[column + 1, row];

                // Do not let a matched item to move
                if (!otherDot.GetComponent<Dot>().isMatched)
                {
                    egManager.DecrementMove();
                    // Other dots positioning
                    otherDot.GetComponent<Dot>().column -= 1;

                    // Current Dot's positioning process
                    column += 1;
                }
            }

            // Up swipe
            else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
            {

                // Other Dot's positioning process
                otherDot = board.allDots[column, row + 1];

                if (!otherDot.GetComponent<Dot>().isMatched)
                {
                    egManager.DecrementMove();
                    // Other dots positioning
                    otherDot.GetComponent<Dot>().row -= 1;

                    // Current Dot's positioning process
                    row += 1;
                }
            }


            // Left swipe
            else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
            {
                // Other Dot's positioning process
                otherDot = board.allDots[column - 1, row];
                if (!otherDot.GetComponent<Dot>().isMatched)
                {
                    egManager.DecrementMove();
                    // Other dots positioning
                    otherDot.GetComponent<Dot>().column += 1;

                    // Current Dot's positioning process
                    column -= 1;
                }
            }

            // Down swipe
            else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
            {
                // Other Dot's positioning process
                otherDot = board.allDots[column, row - 1];
                if (!otherDot.GetComponent<Dot>().isMatched)
                {
                    egManager.DecrementMove();
                    // Other dots positioning
                    otherDot.GetComponent<Dot>().row += 1;

                    // Current Dot's positioning process
                    row -= 1;
                }
            }
        }

        // Co-routine called after movement checks
        StartCoroutine(CheckMoveCo());
    }
}



/*
                int red_ct = 0;
                int blue_ct = 0;
                int yellow_ct = 0;
                int green_ct = 0;

                // Normal case
                if (targetX != 0 || targetX != board.height - 1)
                {
                    // upper grid
                    for (int i = 0; i < targetX; i++)
                    {
                        if () { }
                        red_ct++;
                        blue_ct++;
                        yellow_ct++;
                        green_ct++;
                    }
                    if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
                    {
                        egManager.logicalMoveLeftLower = false;
                    }
                    else
                    {
                        red_ct = 0;
                        blue_ct = 0;
                        yellow_ct = 0;
                        green_ct = 0;
                    }
                    // lower grid
                    for (int i = targetX; i < board.height; i++)
                    {
                        red_ct++;
                        blue_ct++;
                        yellow_ct++;
                        green_ct++;
                    }
                    if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
                    {
                        egManager.logicalMoveLeftLower = false;
                    }
                    else
                    {
                        red_ct = 0;
                        blue_ct = 0;
                        yellow_ct = 0;
                        green_ct = 0;
                    }
                }

                // Lower edge case
                else if (targetX == 0)
                {

                    // upper grid
                    for (int i = 0; i < targetX; i++)
                    {
                        if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
                        {
                            egManager.logicalMoveLeftUpper = false;
                        }
                        else
                        {
                            red_ct = 0;
                            blue_ct = 0;
                            yellow_ct = 0;
                            green_ct = 0;
                        }
                    }
                }

                // upper edge case
                else if (targetX == board.height - 1)
                {
                    // lower grid
                    for (int i = targetX; i < board.height; i++)
                    {
                        if (red_ct < board.width && blue_ct < board.width && yellow_ct < board.width && green_ct < board.width)
                        {
                            egManager.logicalMoveLeftLower = false;
                        }
                        else
                        {
                            red_ct = 0;
                            blue_ct = 0;
                            yellow_ct = 0;
                            green_ct = 0;
                        }
                    }
                }*/