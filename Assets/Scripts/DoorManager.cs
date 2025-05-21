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

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
        Debug.Log(optionIndex);
        switch (optionIndex)
        {
            case (int)DoorActions.OpenFast:
                Debug.Log("Wow, don't open the door too fast");
                break;
            case (int)DoorActions.OpenSlowly:
                Debug.Log("Don't open the door without checking the door knob");
                break;
            case (int)DoorActions.GoAway:
                Debug.Log("Well done, your own safety is most important");
                Debug.Log("However, there might still be people in the room");
                HandleEscapeAction();
                break;
            case (int)DoorActions.TouchKnob:
                Debug.Log("The doorknob is " + GenerateTemperature());
                break;
            case (int)DoorActions.TouchDoor:
                Debug.Log("The door feels " + GenerateTemperature());
                break;
            case (int)DoorActions.Crouch:
                Debug.Log("Crouched");
                break;
            default:
                Debug.LogWarning("Invalid option selected.");
                break;
        }

    }
}