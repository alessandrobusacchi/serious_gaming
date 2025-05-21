using UnityEngine;
using UnityEngine.InputSystem;

public class DoorManager : MonoBehaviour
{
    public Transform player;
    public float proximityThreshold = 1f;

    public GameObject text_interact;
    public GameObject scroll_options;

    private bool isPlayerClose = false;
    private bool imInteracting = false;
    private bool isInProximtiy = false;

    public GameObject playerObject;
    private PlayerInput playerInput;
    private PopupManager popupManager;

    private enum DoorActions
    {
        OpenFast,
        OpenSlowly,
        GoAway,
        TouchKnob,
        TouchDoor,
        Crouch,
    }

    void Start()
    {
        playerInput = playerObject.GetComponent<PlayerInput>();
        popupManager = FindFirstObjectByType<PopupManager>();
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= proximityThreshold)
        {
            if (!imInteracting)
            {
                text_interact.SetActive(true);
            }
            isPlayerClose = true;
        }
        else if (isPlayerClose)
        {
            text_interact.SetActive(false);
            isPlayerClose = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && isPlayerClose)
        {
            imInteracting = true;

            text_interact.SetActive(false);
            scroll_options.SetActive(true);

            if (playerInput != null)
            {
                playerInput.enabled = false;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            LevelManager.Instance.StartInteractionWithDoor(this);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && imInteracting)
        {
            HandleEscapeAction();
            PopupManager.Instance.SetInactive();
        }
    }

    private void HandleEscapeAction()
    {
        imInteracting = false;
        scroll_options.SetActive(false);

        LevelManager.Instance.StopInteractionWithDoor();

        if (playerInput != null)
        {
            playerInput.enabled = true;
            Debug.Log(PopupManager.Instance.popupPanel.activeSelf);
            if (!PopupManager.Instance.popupPanel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }
    }

    private string GenerateTemperature()
    {
        int i = Random.Range(0, 2);
        if (i == 0)
        {
            return "hot";
            // Handle hot doorknob logic
        }
        else
        {
            return "cold";
            // Handle cold doorknob logic
        }
    }

    public void OnOptionSelected(int optionIndex){

        string text = "";
        switch (optionIndex)
        {
            case (int)DoorActions.OpenFast:
                text = "Wow, don't open the door too fast.";
                break;
            case (int)DoorActions.OpenSlowly:
                text = "Don't open the door without checking the door knob.";
                break;
            case (int)DoorActions.GoAway:
                text = "Well done, your own safety is most important.\nHowever, there might still be people in the room.";
                break;
            case (int)DoorActions.TouchKnob:
                text = "The doorknob is " + GenerateTemperature();
                break;
            case (int)DoorActions.TouchDoor:
                text = "The door feels " + GenerateTemperature();
                break;
            case (int)DoorActions.Crouch:
                text = "Crouched";
                break;
            default:
                text = "Invalid option selected.";
                break;
        }

        if (popupManager == null) {
            Debug.Log("No popup manager set, finding it now");
            popupManager = FindFirstObjectByType<PopupManager>();
        }

        popupManager.ShowText(text);

        if(optionIndex == (int)DoorActions.GoAway)
        {
            HandleEscapeAction();
        }
    }
}