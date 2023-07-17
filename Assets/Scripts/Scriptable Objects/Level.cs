using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TODO: read those variables from files
// If 11th level file exists, then generate level object of it and place it under world object.


[CreateAssetMenu (fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{

    // Especially I let them public to see the world at runtime at editor for correctness of levels.
    [Header("Level infos")]
    public int width;
    public int height;
    public int level_no;
    public int maxMoves;

    [Header("Dots for Level")]
    private string inputString;

    // Used in board class to setup the board for the corresponding levels dots
    public int[,] dots2D;


    // Default constructor with parameters
    public void Init(int level_no, int width, int height, int maxMoves, string inputString)
    {
        this.level_no = level_no;
        this.width = width;
        this.height = height;
        this.maxMoves = maxMoves;
        this.inputString = inputString;
    }


    // Level Instance creator function
    public static Level CreateInstance(int level_no,int width, int height, int maxMoves, string inputString)
    {
        var lvl = ScriptableObject.CreateInstance<Level>();
        lvl.Init(level_no ,width, height, maxMoves, inputString);
        return lvl;
    }


    // Those indexes are how I placed a dot in board object, you can check from editor
    private IDictionary<string, int> candyDictionary = new Dictionary<string, int>()
    {
        {"b",0 },
        {"g",1 },
        {"r",2 },
        {"y",3 },

    };


    // Create a 2D dot array to be used in board class for initialization and call it in the awake of board
    public int[,] Make2DArray()
    {

        dots2D = new int[height, width];
        string[] dotsString = inputString.Split(',');

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                dots2D[i, j] = candyDictionary[dotsString[i * width + j]];
            }
        }
        return dots2D;
    }
}