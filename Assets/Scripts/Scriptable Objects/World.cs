using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;



[CreateAssetMenu (fileName = "World", menuName = "World")]
public class World : ScriptableObject
{
    // World has levels to feed the scene...
    public Level[] levels;


    // Fill the available levels to world
    public void fillLevelsToWorld() 
    {
      
        DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] Files = d.GetFiles();

        this.levels = new Level[25];
        firstTenLevelsStatic(); // First ten levels should always be there when game starts

        for (int i = 0; i < Files.Length; i++)
        {
            if (Files[i].Name != "player.dat" && Files[i].Name != ".DS_Store")
            {

            using (StreamReader reader = new StreamReader(Application.persistentDataPath + "/" + Files[i].Name))
                {
                //Debug.Log(Application.persistentDataPath + Files[i].Name);
                string level_no = "";
                string _width = "";
                string _height = "";
                string _maxMoves = "";
                string dots = "";
                string line;
                int line_ct = 0;
                //Debug.Log("File Read Phase started!!!");
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line_ct == 0)
                        {
                            level_no = line.Substring(14);
                        }
                        if (line_ct == 1)
                        {
                            _width = line.Substring(12);
                        }
                        if (line_ct == 2)
                        {
                            _height = line.Substring(13);
                        }
                        if (line_ct == 3)
                        {
                            _maxMoves = line.Substring(12);
                        }
                        if (line_ct == 4)
                        {

                            // Remove this condition if you want all of the levels to be dynamically downloaded.
                            if (int.Parse(level_no) > 10 && int.Parse(level_no) <= 25)
                            {
                                dots = line.Substring(6);
                                //Debug.Log(level_no + " " + _width + " " + _height + " " + _maxMoves);
                                this.levels[int.Parse(level_no) - 1] = (Level.CreateInstance(int.Parse(level_no), int.Parse(_width), int.Parse(_height), int.Parse(_maxMoves), dots));
                            }
                        }
                    line_ct++;
                    }
                }
            }
        }
    }









    // Static first ten levels
    public void firstTenLevelsStatic()
    {
        this.levels[0] = Level.CreateInstance(1, 5, 7, 20, "b,b,y,b,b,g,y,g,r,b,y,g,r,g,g,b,b,g,b,y,r,r,g,g,y,g,g,y,y,b,y,b,b,y,b");
        this.levels[1] = Level.CreateInstance(2, 7, 5, 18, "r,r,b,b,y,b,r,r,y,g,y,y,g,b,r,b,r,y,r,y,y,g,y,r,y,b,g,r,b,r,g,g,g,y,g");
        this.levels[2] = Level.CreateInstance(3, 8, 6, 23, "g,r,y,r,r,g,r,g,y,y,b,b,y,g,r,r,y,g,b,g,y,g,r,y,r,y,g,b,g,y,g,y,r,g,g,y,b,g,b,r,g,b,r,b,g,g,y,g");
        this.levels[3] = Level.CreateInstance(4, 5, 5, 30, "r,r,y,r,r,r,r,b,b,y,g,y,g,y,g,y,b,b,g,g,b,b,b,b,r");
        this.levels[4] = Level.CreateInstance(5, 6, 9, 24, "g,r,y,r,y,y,r,r,g,y,y,r,y,g,g,g,y,g,r,y,b,b,b,g,g,b,g,g,b,g,g,y,r,b,g,b,r,b,b,y,r,y,g,g,b,g,b,b,y,r,b,b,g,g");
        this.levels[5] = Level.CreateInstance(6, 4, 9, 26, "r,g,r,g,r,b,r,b,r,y,y,r,y,y,g,b,y,y,y,b,b,y,g,g,b,r,b,r,y,r,g,r,r,b,g,b");
        this.levels[6] = Level.CreateInstance(7, 7, 9, 23, "y,b,g,r,y,g,b,g,g,r,b,r,b,y,y,g,g,r,g,b,r,b,b,g,y,g,b,b,r,b,g,r,b,r,r,g,b,b,g,b,g,y,y,b,y,g,g,r,b,g,g,y,g,g,y,g,y,r,b,b,b,r,y");
        this.levels[7] = Level.CreateInstance(8, 5, 8, 30, "r,g,b,r,r,b,b,g,r,y,b,y,b,g,g,b,b,b,b,r,r,y,r,g,y,g,g,g,g,r,b,b,b,r,y,b,y,b,r,r");
        this.levels[8] = Level.CreateInstance(9, 6, 5, 19, "y,g,r,b,r,b,b,b,b,b,b,r,g,y,y,y,r,r,g,y,r,b,g,b,r,r,g,r,y,r");
        this.levels[9] = Level.CreateInstance(10, 8, 9, 21, "g,b,y,y,b,g,r,r,g,g,b,g,y,b,g,y,g,b,y,r,b,r,y,g,b,g,r,b,g,g,y,b,g,g,y,y,b,g,y,g,y,r,y,b,b,g,b,g,r,r,g,b,b,y,y,g,g,y,r,r,g,y,b,r,g,y,g,y,g,r,r,b");

    }

}

