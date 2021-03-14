using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    Canvas Canvas;
    TextMeshProUGUI tmpTimer;
    GameObject mainMenuPanel;
    GameObject androidTouchPanel;
    GameObject gamePanel;
    GameObject scoreScreen;
    GameObject winImage;
    GameObject loseImage;
    PlayerPanelController[] PlayerPanels;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
    }
    // Start is called before the first frame update
    void Start()
    {
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        gamePanel = GameObject.Find("GamePanel");
        androidTouchPanel = GameObject.Find("AndroidTouchPanel");
        tmpTimer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        scoreScreen = GameObject.Find("ScoreScreen");
        winImage = GameObject.Find("WinImage");
        loseImage = GameObject.Find("LoseImage");

        PlayerPanels = FindObjectsOfType<PlayerPanelController>();
    }
    

    public void UpdateTimerText(int timer)
    {
        tmpTimer.text = timer.ToString();
    }

    public void LoadMenu(GameStates state)
    {
        gamePanel.SetActive(false);
        scoreScreen.SetActive(false);
        androidTouchPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        mainMenuPanel.GetComponent<UiMenu>().SetState(state);
    }
    public void CloseMenu()
    {
        mainMenuPanel.SetActive(false);
    }
    public void DisplayScoreScreen(bool win)
    {
        gamePanel.SetActive(false);
        scoreScreen.SetActive(true);
        winImage.SetActive(win);
        loseImage.SetActive(!win);
    }
    public void CloseScoreScreens()
    {
        scoreScreen.SetActive(false);
    }
    public void DisplayTouchPadUi()
    {
        androidTouchPanel.SetActive(Utils.IsAndroid());
    }
    public void DisplayGamePanel()
    {
        gamePanel.SetActive(true);
        androidTouchPanel.SetActive(Utils.IsAndroid());
    }

    public void UpdatePlayerPanel(int index, PlayerData data)
    {
        Debug.Log("updating player " + index);
        var panel = PlayerPanels[index];
        if (panel && panel.isActiveAndEnabled)
        {
            panel.SetLives(data.Lives);
            panel.SetName(data.SystemName);
            panel.SetScore(data.Score); 
        }
    }
    public void EnablePlayerPanel(int index, bool enable = true)
    {
        PlayerPanels[index].gameObject.SetActive(enable);
    }

    public void ResetPlayerPanels()
    {
        foreach (var panel in PlayerPanels)
        {
            if (panel!=null && panel.isActiveAndEnabled)
            {
                panel.ClearAll();
                panel.gameObject.SetActive(false);
            }
        }
    }
}
