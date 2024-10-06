using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum SheepStatus
{
    Alive,
    Dead,
    Success
}

public enum LevelState
{
    Playing,
    LevelDone,
    GameDone,
    GameOver
}

public class LevelManager : MonoBehaviour
{
    public SheepStatus[] sheepStatuses;
    public SceneAsset nextLevel;
    public GameObject levelDonePanel;
    public GameObject gameDonePanel;
    public GameObject gameOverPanel;
    private SlideInController levelDoneScreen;
    private SlideInController failScreen;
    private SlideInController gameDoneScreen;
    public BoxCollider2D levelBounds;
    private BoxCollider2D endZone;
    private SheepController[] sheepControllers;
    private int initialSheepCount = 0;
    private int deadSheepCount = 0;
    private int successSheepCount = 0;
    private bool readyForNextLevel = false;
    private LevelState levelState = LevelState.Playing;
    private CameraController cameraController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelBounds = GetComponent<BoxCollider2D>();
        endZone = GameObject.Find("End Zone").GetComponent<BoxCollider2D>();
        cameraController = FindFirstObjectByType<CameraController>();
        sheepControllers = FindObjectsByType<SheepController>(FindObjectsSortMode.None);
        initialSheepCount = sheepControllers.Length;

        levelDoneScreen = levelDonePanel.GetComponent<SlideInController>();
        gameDoneScreen = gameDonePanel.GetComponent<SlideInController>();
        failScreen = gameOverPanel.GetComponent<SlideInController>();
    }

    public void UpdateSheepStatuses()
    {
        if (levelState != LevelState.Playing)
        {
            return;
        }
        deadSheepCount = sheepControllers.Count(sheepController => !sheepController.isAlive);
        successSheepCount = sheepControllers.Count(sheepController => sheepController.isSuccess);
        sheepStatuses = sheepControllers
            .Select(sheepController =>
            {
                if (!sheepController.isAlive)
                {
                    return SheepStatus.Dead;
                }
                else if (sheepController.isSuccess)
                {
                    return SheepStatus.Success;
                }
                else
                {
                    return SheepStatus.Alive;
                }
            })
            .ToArray();
        if (
            sheepStatuses.All(sheepStatus => sheepStatus != SheepStatus.Alive)
            && sheepStatuses.Any(sheepStatus => sheepStatus == SheepStatus.Success)
        )
        {
            cameraController.isEnabled = false;
            if (nextLevel != null)
            {
                levelDoneScreen.SlideIn(() => readyForNextLevel = true);
                levelState = LevelState.LevelDone;
            }
            else
            {
                gameDoneScreen.SlideIn();
                levelState = LevelState.GameDone;
            }
        }
        else if (sheepStatuses.All(sheepStatus => sheepStatus != SheepStatus.Alive))
        {
            FailLevel();
        }
    }

    private void FailLevel()
    {
        cameraController.isEnabled = false;
        failScreen.SlideIn(() => readyForNextLevel = true);
        levelState = LevelState.GameOver;
    }

    public void OnPlayerDeath()
    {
        if (levelState == LevelState.Playing)
        {
            FailLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // restart level if any key is pressed once ready for next level
        if (levelState != LevelState.GameOver && readyForNextLevel && Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextLevel.name);
        }
        else if (levelState == LevelState.GameOver && Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
