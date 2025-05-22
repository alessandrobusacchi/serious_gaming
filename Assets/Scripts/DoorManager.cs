using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public GameObject playerObject;
    private PlayerInput playerInput;
    private PopupManager popupManager;

    // to be removed?? need to decide if we need it or not: for me, either we keep this or the doorname just as reference. 
    // we use the other enums for the handling of particle systems
    public enum DoorType
    {
        DoorType1,
        DoorType2,
        DoorType3,
        DoorType4
    }

    // with these we can set different levels of smoke and fire
    public enum SmokeLevel
    {
        None,
        Low,
        Medium,
        High
    }

    public enum FireLevel
    {
        None,
        Low,
        Medium,
        High
    }

    public enum DoorKnobTemperature
    {
        Cold,
        Hot
    }

    public enum DoorTemperature
    {
        Cold,
        Hot
    }

    public enum DoorActions
    {
        OpenFast,
        OpenSlowly,
        GoAway,
        TouchKnob,
        TouchDoor,
        Crouch,
    }

    public string doorName;
    public DoorType doorType;
    public SmokeLevel smokeLevel;
    public FireLevel fireLevel;
    public DoorKnobTemperature doorKnobTemperature;
    public DoorTemperature doorTemperature;
    public bool hazardousMaterials;
    public List<DoorActions> correctActionSequence;

    private List<DoorActions> playerActionSequence = new List<DoorActions>();

    void Start()
    {
        playerInput = playerObject.GetComponent<PlayerInput>();
        popupManager = FindFirstObjectByType<PopupManager>();

        if (smokeLevel != SmokeLevel.None)
        {
            // to extend with levels of smoke and fire
            ActivateSmoke();
        }
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

    // to be extended
    void ActivateSmoke()
    {
        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
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

            if (!PopupManager.Instance.popupPanel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        playerActionSequence.Clear();
    }

    public void OnOptionSelected(int optionIndex)
    {
        // my idea is: we can list the next options (example, first the player touches the knob right?
        // then he needs maybe to select actions that are not in the buttons, so maybe we need another one
        // but we still can pass index 7, 8, 9, and have them the same in the enumerative
        DoorActions selectedAction = (DoorActions)optionIndex;
        playerActionSequence.Add(selectedAction);

        // I also give indication either the choice that he made is correct, so we can think about how to do this
        string text = selectedAction switch
        {
            DoorActions.OpenFast => "Wow, don't open the door too fast.",
            DoorActions.OpenSlowly => "Don't open the door without checking the door knob.",
            DoorActions.GoAway => "Well done, your own safety is most important.\nHowever, there might still be people in the room.",
            DoorActions.TouchKnob => "The doorknob is " + doorKnobTemperature.ToString(),
            DoorActions.TouchDoor => "The door feels " + doorTemperature.ToString(),
            DoorActions.Crouch => "Crouched",
            _ => "Invalid option selected."
        };

        if (popupManager == null)
        {
            popupManager = FindFirstObjectByType<PopupManager>();
        }

        //popupManager.ShowText(text);
        //this was overwritten

        CheckActionSequence(selectedAction, text);

        if (selectedAction == DoorActions.GoAway)
        {
            HandleEscapeAction();
        }
    }

    // actions are handled one per one. if it makes one not correct, he needs to start again
    // if it is correct, he can continue with the next one
    // if it is the last one, successfully done, we can close the interaction and hide the UI
    private void CheckActionSequence(DoorActions lastAction, string text)
    {
        popupManager.SetInactive();
        popupManager.SetActive();
        if (lastAction != DoorActions.GoAway)
        {
            // if the last action is not the one to go away, we check if it is correct
            int currentStep = playerActionSequence.Count - 1;

            if (currentStep >= correctActionSequence.Count ||
                playerActionSequence[currentStep] != correctActionSequence[currentStep])
            {
                popupManager.ShowText("Wrong action. Try again from the beginning.");
                playerActionSequence.Clear();
                //scroll_options.SetActive(false); // erasing sequence of the player and closing the UI
                //this causes the UI to close, but we need to keep it open for the next action (to try again)
                //or we close it, but then we have to call HandleEscapeAction()
                return;
            }

            // correct
            if (playerActionSequence.Count == correctActionSequence.Count)
            {
                popupManager.ShowText("Sequence completed! Well done!");
                HandleEscapeAction(); // interaction is over and close ui
            }
            else
            {
                popupManager.ShowText(text + "\nCorrect action, keep it up...");
            }
        }else
        {
            // if the last action is to go away, we just print the text
            popupManager.ShowText(text);
        }
    }
}
