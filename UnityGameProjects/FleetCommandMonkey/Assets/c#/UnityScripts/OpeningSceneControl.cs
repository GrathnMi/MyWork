using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OpeningSceneControl : MonoBehaviour {

    //set in insepector
    public Button buttonBack;
    public Button buttonPlay;
    public Button buttonOptions;
    public Button buttonStatistics;
    public Button buttonOnline;
    public Button buttonSplitscreen;
    public Button buttonVsAI;
    public GameObject panelPlayMenu;
    public GameObject panelMainMenu;

    public NetworkManager manager;
    
    

    // Use this for initialization
    void Start ()
    {
        UIReset();

        manager = GameObject.Find("ServerObj").GetComponent<NetworkManager>();
        if (manager == null)
            Debug.LogError("No network manager set! was it renamed?");

    }
	
	// Update is called once per frame
	void Update () {
	
	}





    //UI functions
    private void UIReset()
    {
        UI_ActivateMainMenu();
        UI_DeactivatePlayMenu();
    }
    //Back button
    public void UI_Back()
    {
        UIReset();
    }
    //Main Menu
    private void UI_ActivateMainMenu()
    {
        buttonPlay.interactable = true;
        buttonOptions.interactable = true;
        buttonStatistics.interactable = true;
    }
    private void UI_DeactivateMainMenu()
    {
        buttonPlay.interactable = false;
        buttonOptions.interactable = false;
        buttonStatistics.interactable = false;
    }
    public void UIPlay()
    {
        UI_ActivatePlayMenu();
        UI_DeactivateMainMenu();
    }
    public void UIOptions()
    {

    }
    public void UIStats()
    {

    }
   
    //play Menu
    private void UI_ActivatePlayMenu()
    {
        UI_DeactivateMainMenu();
        buttonOnline.interactable = true;
        buttonSplitscreen.interactable = true;
        buttonVsAI.interactable = true;
    }
    private void UI_DeactivatePlayMenu()
    {
        buttonOnline.interactable = false;
        buttonSplitscreen.interactable = false;
        buttonVsAI.interactable = false;
    }
    public void UILocalLanHost()
    {
        manager.StartHost();
        manager.networkAddress = "localhost";
    }
    public void UILocalLanClient()
    {
        manager.StartClient();
        manager.networkAddress = "localhost";
    }
    public void UIPlayVsAI()
    {
        manager.StartHost();
    }



}
