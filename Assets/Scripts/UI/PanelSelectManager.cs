using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSelectManager : MonoBehaviour
{

    [Header("Panel Stuff")]
    public GameObject[] panels; // Level panels, i.e pages of levels.
    public GameObject currentPanel; // Current panel to start the level select popup from where user left.
    public int page;
    public int currentLevel = 0;

    private GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        if (gameData != null)
        {
            for (int i = 0; i < gameData.saveData.isActives.Length; i++)
            {
                if (gameData.saveData.isActives[i])
                {
                    currentLevel = i;
                }
            }
        }
        page = (int) Mathf.Floor(currentLevel / 4);
        currentPanel = panels[page];
        panels[page].SetActive(true);
    }

    public void pageRight()
    {
        if (page < panels.Length - 1)
        {
            currentPanel.SetActive(false);
            page++;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
        }
    }


    public void pageLeft()
    {
        if (page > 0)
        {
            currentPanel.SetActive(false);
            page--;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
        }
    }
}
