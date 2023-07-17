using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    private static bool initDone = true; 

    public GameObject startPanel;
    public GameObject levelsPanel;
    private Button playButton;


    // Start is called before the first frame update
    void Start()
    {
        // Do not open starting page each time a level finishes
        if (initDone)
        {
            playButton = GetComponent<Button>();
            startPanel.SetActive(true);
            levelsPanel.SetActive(false);
            playButton.onClick.AddListener(Play);
        }
        else
        {
            startPanel.SetActive(false);
            levelsPanel.SetActive(true);
        }
        
    }

    // Invoked when play button clicked
    private void Play()
    {
        initDone = false;
        startPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }
}
