using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiMenu : MonoBehaviour
{
    GameManager gameManager;

    Transform background;

    Button resume;
    Button newGame;
    Button quit;
    TextMeshProUGUI title;

    public void SetState(GameStates state)
    {
        if (state == GameStates.MainMenu)
        {
            title.text = "Main menu";
            background.gameObject.SetActive(true);
            EnableElement(resume, false);
            EnableElement(newGame);
        }
        else if (state == GameStates.PauseMenu)
        {
            title.text = "Pause menu";
            background.gameObject.SetActive(false);
            EnableElement(resume);
            EnableElement(newGame);
        }
    }

    private void Awake()
    {
        Initialize();
    }
    private void Start()
    {
        gameManager = GameManager.Instance;
    }



    private void Initialize()
    {
        resume = GameObject.Find("Resume").GetComponent<Button>();
        newGame = GameObject.Find("NewGame").GetComponent<Button>();
        quit = GameObject.Find("Quit").GetComponent<Button>();
        title = GameObject.Find("MenuTitle").GetComponent<TextMeshProUGUI>();

        background = transform.Find("Background");

        //SetEventHandlers();
    }
    private void SetEventHandlers()
    {
        resume.onClick.AddListener(() =>
        {
            gameManager.Resume();
        });

        newGame.onClick.AddListener(() =>
        {
            gameManager.NewGame();
        });

        quit.onClick.AddListener(() =>
        {
            gameManager.Quit();
        });
    }
    private void EnableElement(Component button, bool enable = true)
    {
        button.gameObject.SetActive(enable);
    }
}
