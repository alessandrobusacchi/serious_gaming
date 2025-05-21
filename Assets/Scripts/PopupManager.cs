using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    private TextMeshProUGUI TMPText;

    public static PopupManager Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Start()
    {
        if( TMPText == null)
        {
            TMPText = popupPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        TMPText.text = "";
        SetInactive();
    }
    public void SetActive()
    {
        popupPanel.SetActive(true);
    }

    public void SetInactive()
    {
        popupPanel.SetActive(false);
        if( TMPText.text == "Well done, your own safety is most important.\nHowever, there might still be people in the room.")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    } 

    public void ShowText(string message)
    {
        if (TMPText == null)
        {
            Debug.Log("No text element set, getting it now");
            TMPText = popupPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        TMPText.text = message;
        SetActive();
    }
}
