using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FX_MenuCreation : MonoBehaviour {

    public GameObject buildBeemEmitter;

    private bool startEffect;

    private List<int> pixelList = new List<int>();
    private int currentPixelFromList = 0;

    //unity functions
    void Start ()
    {
        //Create a list of pixels on the screen
        for (int i = 0; i < Screen.height; i++)
        {
            pixelList.Add(i);
        }
        //"randomize" the list. gos though the list swopping the position of the numbers randomly
        for (int i = 0; i < pixelList.Count; i++)
        {
            int temp = pixelList[i];
            int rndNum = Random.Range(i, pixelList.Count);

            pixelList[i] = pixelList[rndNum];
            pixelList[rndNum] = temp;
        }
	}
	
	void Update ()
    {
        if (startEffect)
        {
            CreateLine(currentPixelFromList);
            currentPixelFromList++;
            if (currentPixelFromList == pixelList.Count)
            {
                effectFinished();
            }
        }
    }



    //custom functions
    public void StartEffect()
    {
        Debug.Log("starting effect");
        startEffect = true;


    }
    //Creates the line
    private void CreateLine(int n)
    {
        HorizontalEffect();
        
    }

    private void HorizontalEffect()
    {

    }
    private GameObject GetEmitter()
    {
        GameObject Obj = (GameObject)Instantiate(buildBeemEmitter);


        return Obj;
    }
    private void effectFinished()
    {
        startEffect = false;
        Debug.Log("finishing effect");
    }





}
