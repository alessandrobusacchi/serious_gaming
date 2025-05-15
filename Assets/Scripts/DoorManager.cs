using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Transform player;
    public float proximityThreshold = 1f;

    public GameObject text_interact;
    public GameObject scroll_options;

    private bool isPlayerClose = false;
    private bool imInteracting = false;


    private enum DoorActions
    {
        OpenFast,
        OpenSlowly,
        GoAway,
        TouchKnob,
        Option5,
        Option6,
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
            if(!imInteracting)
                text_interact.SetActive(true);
            isPlayerClose = true;
        }
        else
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
        }
    }

    public void OnOptionSelected(int optionIndex){
        Debug.Log(optionIndex);
    }
}