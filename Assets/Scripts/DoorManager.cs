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
        Option5,
        Option6,
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

            //to fix, block movements of player and camera

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
            imInteracting = false;

            //to fix, enable movements of player and camera

            scroll_options.SetActive(false);

            LevelManager.Instance.StopInteractionWithDoor();

            if (playerInput != null)
            {
                playerInput.enabled = true;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void OnOptionSelected(int optionIndex){
        Debug.Log(optionIndex);
    }
}