using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshPro PointsText;
    public float PlayerInitialSpeed = 10;
    public WeaponTypes weapons;
    public PlayerManager playerManager;
    public LevelData[] levels;
    
    //debug
    [Header("Debug mode")]
    public bool DebugMode;
    private float debugTimer;

    private static GameManager _instance;
    private EntityManager entitySpawner;
    private MusicPlayer musicPlayer;
    private UiManager UiManager;

    

    private SpriteRenderer gameAreaBg;

    private Player player1, player2;

    private GameStates gameState = GameStates.MainMenu;

    private int currentLevelIndex = -1;
    private float timer = 100;
    private float sequenceTimer = 0;
    private bool gameOver = false;
    


    //debug
    [SerializeField] PlayerInput player1Input;
    [SerializeField] PlayerInput player2Input;



    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }


    #region publics

    public GameStates GetGameState()
    {
        return gameState;
    }
    public void NewGame()
    {
        //destroy all objects
        entitySpawner.DestroyAll();
        
        //remove players
        playerManager.DeletePlayers();


        StartGame();
        Resume();
    }
    public void Quit()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
    public void StartGame()
    {
        try
        {
            //only create player 1
            player1 = playerManager.SpawnPlayer();
            player1.SetWeapon(weapons.Weapons[0]);
            UiManager.EnablePlayerPanel(0);
            UiManager.UpdatePlayerPanel(0, playerManager.GetPlayerData(player1));
            UiManager.ResetPlayerPanels();

            LoadLevel(0);
            gameOver = false;
            UiManager.DisplayTouchPadUi();
            UiManager.CloseMenu();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("Error starting game: " + ex.Message, ex);
        }
    }
    public void KillBall(Ball ball)
    {
        entitySpawner.KillObject(ball);

        //check if ball count is 0, win level
        if (entitySpawner.GetBallCount() == 0)
        {
            LevelWon();
        }
    }
    public void DestroyWeapon(WeaponObject weaponObject)
    {
        entitySpawner.KillObject(weaponObject);
    }
    public void DestroyObstacle(Obstacle obstacle)
    {
        entitySpawner.KillObject(obstacle);
    }
    public Ball SplitBall(Ball ball, Vector2 velocity)
    {
        var clone = entitySpawner.SpawnEntity(ball, ball.transform.position, "Ball");

        ball.SetVelocity(new Vector2(GameDefaults.BallStartVelocity.x, velocity.y));

        clone.SetVelocity(new Vector2(-GameDefaults.BallStartVelocity.x, velocity.y));
        clone.SetSize(ball.GetSize());

        return clone;
    }
    public void PlayerHit(Player player)
    {
        playerManager.PlayerHit(player);
        
        UiManager.UpdatePlayerPanel(GetPlayerIndex(player), playerManager.GetPlayerData(player));

        LevelLost();
        
        if (playerManager.GetPlayerData(player).Lives == 0)
        {
            playerManager.RemovePlayer(player);
            Destroy(player.gameObject);
        }
    }
    public void ScorePlayer(Player player, Vector2 position)
    {
        //randomly give points on ball hit
        if (Random.Range(0f, 1f) > 0.7f)
        {
            var points = 300;
            var tPoints = entitySpawner.SpawnPointBanner(PointsText, position, points);

            playerManager.AddScore(player, points);
            var playerData = playerManager.GetPlayerData(player);
            var playerIndex = GetPlayerIndex(player);
            UiManager.UpdatePlayerPanel(playerIndex, playerData);

        }
    }
    public void Resume()
    {
        gameState = GameStates.Level;
        Time.timeScale = 1;
        UiManager.CloseMenu();
    }
    public void MobileLeftTapped()
    {
        player1Input.X = -1;
    }
    public void MobileRightTapped()
    {
        player1Input.X = 1;
    }
    public void MobileFireTapped()
    {
        player1Input.Fire = true;
    }

    #endregion


    #region unity messages

    private void Awake()
    {
        _instance = this;
        entitySpawner = GetComponent<EntityManager>();
        musicPlayer = GetComponent<MusicPlayer>();

    }
    void Start()
    {
        InitializeGame();
        LoadMenu(GameStates.MainMenu);
    }
    private void Update()
    {
        if (DebugMode)
        {
            //Debug purposes only!!!
            DebugGame(); 
        }
        //acting as state machine
        switch (gameState)
        {
            case GameStates.Sequence:
            case GameStates.ScoreScreen:
                //timer
                UpdateSequenceTimer();
                break;
            case GameStates.Level:
                PlayLevel();
                break;
        }
    }
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!Utils.IsPlatform(RuntimePlatform.WindowsPlayer))
        {
            return;
        }

        if (type == LogType.Error || type == LogType.Warning || type == LogType.Exception)
        {
            var output = logString;
            var stack = stackTrace;
            var path = Application.dataPath + "\\Errors.log";

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("Error log:");
                }
            }
            else
            {
                using (var sw = File.AppendText(path))
                {
                    sw.Write(output);
                    sw.WriteLine(stack);
                }
            }
        }
    }
    private void DebugGame()
    {
        if (gameState == GameStates.Level)
        {
            //Advance levels
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ClearLevel();
                if (currentLevelIndex < levels.Length - 1)
                {
                    currentLevelIndex++;
                    LoadLevel(currentLevelIndex);
                }
            }
        }
        else if (gameState == GameStates.MainMenu)
        {
            debugTimer -= Time.deltaTime;
            if (debugTimer <= 0)
            {
                NewGame();
            }
        }
    }

    #endregion


    private void InitializeGame()
    {
        UiManager = transform.Find("PangUi").GetComponent<UiManager>();
        gameAreaBg = GameObject.Find("BackImage").GetComponent<SpriteRenderer>();
    }
    private void ResetPlayerPositions()
    {
        if (player1 != null)
        {
            player1.transform.position = GameDefaults.PlayerStartPositions[0]; 
        }
        if (player2 != null)
        {
            player2.transform.position = GameDefaults.PlayerStartPositions[1];
        }
    }
    private void PlayLevel()
    {
        //handle inputs based on device
        if (!Utils.ExecuteForPlatform(RuntimePlatform.Android, HandleMobileInputs))
        {
            HandlePCInputs();
        }

        UpdatePlayerActions(player1, player1Input);
        UpdatePlayerActions(player2, player2Input);

        //timer
        UpdateLevelTimer();
    }
    private void UpdateLevelTimer()
    {
        timer -= Time.deltaTime;
        UiManager.UpdateTimerText((int)timer);
        if (timer <= 0)
        {
            LevelLost();
        }
    }
    private void UpdateSequenceTimer()
    {
        //for sequences, use the unscaled timer
        sequenceTimer -= Time.deltaTime;
        if (sequenceTimer <= 0)
        {
            CheckTransition();
        }
    }
    private void Pause()
    {
        Time.timeScale = 0;

        LoadMenu(GameStates.PauseMenu);
    }
    private void CheckTransition()
    {
        //decide whether to transition to next/current level, or main menu
        if (gameOver)
        {
            EndGame();
        }
        else
        {
            UiManager.CloseScoreScreens();
            LoadLevel(currentLevelIndex);
        }
    }

    private void LoadMenu(GameStates state)
    {
        gameState = state;
        UiManager.LoadMenu(state);

        if (DebugMode)
        {
            debugTimer = 5; 
        }
    }
    private void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        //check if no levels left
        if (levels.Length <= levelIndex)
        {
            EndGame();
            return;
        }

        var level = levels[currentLevelIndex];
        ResetPlayerPositions();
        SetBackground(level.Background);
        musicPlayer.PlayLevelMusic();
        entitySpawner.SpawnLevelEntities(level);
        UiManager.DisplayGamePanel();
        timer = 100;
        gameState = GameStates.Level;

    }
    private void LevelWon()
    {
        //play animation of win,
        //Display score
        sequenceTimer = 5;
        gameState = GameStates.ScoreScreen;
        UiManager.DisplayScoreScreen(true);
        musicPlayer.PlayWin();
        ClearLevel();
        //advance current level
        currentLevelIndex++;
    }
    private void LevelLost()
    {
        //do one time update of player actions to reset the character forces
        ResetPlayerActions();

        //do: animation of dying
        if (playerManager.PlayersAlive() == 0)
        {
            sequenceTimer = 5;
            gameState = GameStates.ScoreScreen;
            UiManager.DisplayScoreScreen(false);
            gameOver = true;
        }
        else
        {
            sequenceTimer = 5;
            gameState = GameStates.Sequence;
        }

        musicPlayer.PlayLose();


        ClearLevel();
    }
    private void ClearLevel()
    {
        //destroy all objects
        entitySpawner.DestroyAll();
    }

    private void EndGame()
    {
        playerManager.DeletePlayers();

        LoadMenu(GameStates.MainMenu);
    }

    private void SetBackground(Sprite background)
    {
        gameAreaBg.sprite = background;
    }
    private void ClearInput(PlayerInput input)
    {
        input.X = 0;
        input.Fire = false;
    }
    private void ResetPlayerActions()
    {
        ClearInput(player1Input);
        UpdatePlayerActions(player1, player1Input);
        ClearInput(player2Input);
        UpdatePlayerActions(player2, player2Input);
    }
    private void HandlePCInputs()
    {
        //players 1, 2 inputs
        GetPlayer1PCInputs();
        if (player2)
        {
            GetPlayer2PCInputs();
        }
        else
        {
            //if player2 is not in yet

            //return adds player2
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                AddSecondPlayer();
            }

        }

        //anyone hits esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    private void AddSecondPlayer()
    {
        player2 = playerManager.SpawnPlayer();
        player2.SetWeapon(weapons.Weapons[0]);
        UiManager.EnablePlayerPanel(1);
        UiManager.UpdatePlayerPanel(1, playerManager.GetPlayerData(player2));
    }

    private void GetPlayer1PCInputs()
    {
        //player 1
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            player1Input.X = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            player1Input.X = 1;
        }
        else
        {
            player1Input.X = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player1Input.Fire = true;
        }
        else
        {
            player1Input.Fire = false;
        }
    }
    private void GetPlayer2PCInputs()
    {
        //player 2
        if (Input.GetKey(KeyCode.A))
        {
            player2Input.X = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            player2Input.X = 1;
        }
        else
        {
            player2Input.X = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player2Input.Fire = true;
        }
        else
        {
            player2Input.Fire = false;
        }
    }

    private void HandleMobileInputs()
    {
        player1Input.X = Input.GetAxisRaw("Horizontal");
        player1Input.Fire = Mathf.Abs(Input.GetAxisRaw("Submit")) > 0;
    }
    
    private void UpdatePlayerActions(Player player, PlayerInput input)
    {
        if (player)
        {
            player.Move(Vector3.right * input.X);
            player.UpdateCooldownTimer();
            if (input.Fire)
            {
                entitySpawner.SpawnWeaponObject(player);
                player.FireWeapon();
            }
        }
    }
    private int GetPlayerIndex(Player player)
    {
        return player == this.player1 ? 0 : 1;
    }
}
