﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.UI;

public class TakeMissionContractButton : MonoBehaviour {

    public Text MissionContractButtonText;

    private MissionContractTask missionContractTask;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Setup(MissionContractTask missionContract)
    {
        missionContractTask = missionContract;
        UpdateButtonText();
    }

    public void UpdateButtonText()
    {
        var manMonths = gameManager.GetEngineeringManMonthProgress();
        var duration = Mathf.CeilToInt(missionContractTask.DurationInMonths / (manMonths > 0 ? manMonths : -1)); // Avoid dividing by 0

        if (duration < 0)
            duration = -1;

        MissionContractButtonText.text = 
            "D: " +  duration + 
            "-S: " + (gameManager.GetMissionContractSuccessProbability(missionContractTask) * 100).ToString("0.0") + "%" +
            "-Com: " + missionContractTask.Complexity + 
            "-V: " + missionContractTask.Value;
    }

    public void OnStartMissionContract()
    {
        gameManager.OnStartMissionContract(missionContractTask);
    }
}
