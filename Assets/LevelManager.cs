using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public DoorManager currentDoor;  // Door being interacted with
    private HashSet<DoorManager> completedDoors = new HashSet<DoorManager>();
    private int peopleSaved = 0;

    public float levelDuration = 90f; // seconds
    private float timeRemaining;
    private bool isGameOver = false;

    public TextMeshProUGUI timerText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        timeRemaining = levelDuration;
    }

    void Update()
    {
        if (isGameOver) return;

        timeRemaining -= Time.deltaTime;

        if (timerText != null)
            timerText.text = FormatTime(timeRemaining);

        if (timeRemaining <= 0f)
        {
            GameOver();
        }
    }

    private string FormatTime(float time)
    {
        time = Mathf.Max(0, time);
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:D2}:{seconds:D2}";
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");

        // You can show a UI here or reload the level
        // For now, let's reload the current scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartInteractionWithDoor(DoorManager door)
    {
        currentDoor = door;
    }

    public void StopInteractionWithDoor()
    {
        currentDoor = null;
    }

    public void CompleteDoor(DoorManager door)
    {
        if (!completedDoors.Contains(door))
        {
            completedDoors.Add(door);
            Debug.Log("Door completed!");
        }
    }

    public void SavePerson()
    {
        peopleSaved++;
        Debug.Log($"People saved: {peopleSaved}");
    }

    public void SelectDoorOption(int optionIndex)
    {
        if (currentDoor != null)
        {
            Debug.Log(optionIndex);
            currentDoor.OnOptionSelected(optionIndex);
        }
        else
        {
            Debug.LogWarning("No door is currently being interacted with.");
        }
    }

    public int GetPeopleSaved() => peopleSaved;
    public bool IsDoorCompleted(DoorManager door) => completedDoors.Contains(door);
}
