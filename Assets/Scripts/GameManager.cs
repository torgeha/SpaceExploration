using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text TurnText;
    public Text FundsText;
    public Text OutputText;
    public Text InventoryText;

    public GameObject MissionContractPanel;
    public Text CurrentMissionContractText;

    public Text EngineeringProficiencyText;
    public Text ScientistProficiencyText;
    public GameObject MainCanvas;
    public GameObject HiresCanvas;
    public GameObject MissionContractButtonPrefab;

    private long funds;
    private int month;

    private const int totalAvailableMissionContracts = 5;

    private List<GameObject> missionContractButtons = new List<GameObject>();

    private List<EngineerStaff> engineers = new List<EngineerStaff>();
    private List<ScientistStaff> scientists = new List<ScientistStaff>();
    private List<MissionContractTask> availableMissionContracts = new List<MissionContractTask>();

    private MissionContractTask currentMissionContract;
    private ResearchTask researchTask;

    private float engineeringProficiencyModifier = 1.0f; // Will increase by doing research tasks
    private float engineeringProductivityModifier = 1.0f; // Will increase by doing research tasks

	// Use this for initialization
	void Start () {

        funds = 100;
        month = 1;

        TurnText.text = "Month: " + month;
        updateFundsText();
        OutputText.text = "New game\n";

        resetAvailableMissionContracts();


    }
	
    public void OnProcessNextRound()
    {
        increaseMonth();
        updateFunds();
        updateResearchProgress();
        updateMissionContractProgress();

        updateOutputText();
    }

    public void OnOpenHireCanvas()
    {
        MainCanvas.SetActive(false);
        HiresCanvas.SetActive(true);
    }

    public void OnOpenMainCanvas()
    {
        HiresCanvas.SetActive(false);
        MainCanvas.SetActive(true);
    }

    public void OnHireScientist()
    {
        var scientist = new ScientistStaff(10, 10, 5);
        scientists.Add(scientist);
        updateInventoryText();

        // TODO: set proficiency modifier
    }

    public void OnHireEngineer()
    {
        var engineer = new EngineerStaff(10, 10, 5);
        engineers.Add(engineer);
        updateInventoryText();

        // Update engineering proficiency modifier
        var engineeringQuality = getEngineeringQuality();
        engineeringProficiencyModifier = 1.0f + (engineeringQuality / 100);
        EngineeringProficiencyText.text = "Engineering proficiency: " + engineeringProficiencyModifier;

        updateMissionContractButtonTexts();
    }

    public void OnStartMissionContract(MissionContractTask mc)
    {
        if (currentMissionContract != null)
        {
            OutputText.text = "Mission contract already in progress, only one allowed at a time.";
            return;
        }

        if (engineers.Count == 0)
        {
            OutputText.text = "Cannot start mission contract without engineers.";
            return;
        }

        currentMissionContract = mc;
        CurrentMissionContractText.text = getCurrentMissionContractString();

        // Remove available contracts
        deleteAvailableMissionContracts();
    }

    public void OnLaunchMission()
    {
        // Can only launch mission if a mission is complete
        if (currentMissionContract == null || !currentMissionContract.IsComplete())
        {
            OutputText.text = "Cannot launch mission without a completed Mission Contract";
            return;
        }

        // Find probability of success
        var successProbability = GetMissionContractSuccessProbability(currentMissionContract);
        Debug.Log("Calciing success...");
        Debug.Log("succprob: " + successProbability);
        var random = Random.Range(0.0f, 1.0f);
        Debug.Log("random: " + random);

        CurrentMissionContractText.text = "No Current Mission Contracts";
        resetAvailableMissionContracts();

        if (random <= successProbability)
        {
            handleMissionContractSuccess();
            return;
        }

        handleMissionContractFailure();
    }

    public float GetMissionContractSuccessProbability(MissionContractTask mc)
    {
        var successProbability = getEngineeringQuality() / (float)mc.Complexity;
        successProbability *= engineeringProficiencyModifier;
        return successProbability;
    }

    private void resetAvailableMissionContracts()
    {
        deleteAvailableMissionContracts();

        for (var i = 0; i < totalAvailableMissionContracts; i++)
        {
            // Create a random mission contract
            var durationRand = Random.Range(1, 10);
            var complexityRand = Random.Range(10, 100);

            var duration = (int)durationRand; // 1-10 months
            var complexity = (int)complexityRand; // 10-100

            var reward = (duration * complexity);
            var rewardRand = (int)Random.Range(reward * -0.5f, reward * 0.5f);
            reward = reward + rewardRand; // based on complexity and duration +- half itself to add some randomness

            var mc = new MissionContractTask(duration, complexity, reward);
            availableMissionContracts.Add(mc);

            // Create buttons
            var missionTaskButton = Instantiate(MissionContractButtonPrefab);
            missionTaskButton.transform.SetParent(MissionContractPanel.transform);
            missionTaskButton.GetComponent<TakeMissionContractButton>().Setup(mc);
            missionContractButtons.Add(missionTaskButton);
        }
    }

    private void deleteAvailableMissionContracts()
    {
        if (missionContractButtons.Count < 1)
            return;

        for (int i = 0; i < totalAvailableMissionContracts; i++)
        {
            Destroy(missionContractButtons[i]);
        }

        missionContractButtons = new List<GameObject>();
        availableMissionContracts = new List<MissionContractTask>();
    }

    private void updateMissionContractButtonTexts()
    {
        foreach(var b in missionContractButtons)
        {
            b.GetComponent<TakeMissionContractButton>().UpdateButtonText();
        }
    }



    private void handleMissionContractSuccess()
    {
        funds += currentMissionContract.Value;
        OutputText.text = "Success! You have been rewarded " + currentMissionContract.Value + " funds!";
        updateFundsText();
        currentMissionContract = null;
    }

    private void handleMissionContractFailure()
    {
        OutputText.text = "Failure! The customer will not pay for failures :(";
        currentMissionContract = null;
    }

    private int getEngineeringQuality()
    {
        // Get the max proficiency of engineers

        // OBS can be done with linq expression?!
        var maxProf = 0;
        foreach(var e in engineers)
        {
            if (e.Proficiency > maxProf)
                maxProf = e.Proficiency;
        }
        return maxProf;
    }

    private void updateInventoryText()
    {
        var inventory = "";
        foreach(var s in scientists)
        {
            inventory += s + "\n";
        }
        foreach(var e in engineers)
        {
            inventory += e + "\n";
        }
        InventoryText.text = inventory;
    }

    private void increaseMonth()
    {
        month++;
        TurnText.text = "Month: " + month;
    }

    private void updateFunds()
    {
        // + monthly income - salary - rent
        var expences = 0;

        expences += getScientistSalaries();
        expences += getEngineerSalaries();

        funds -= expences;

        updateFundsText();
    }

    private int getScientistSalaries()
    {
        var salaries = 0;
        foreach(var scientist in scientists)
        {
            salaries += scientist.Salary;
        }
        return salaries;
    }

    private int getEngineerSalaries()
    {
        var salaries = 0;
        foreach (var engineer in engineers)
        {
            salaries += engineer.Salary;
        }
        return salaries;
    }

    private void updateResearchProgress()
    {
    }

    private void updateMissionContractProgress()
    {
        if (currentMissionContract != null)
        {
            currentMissionContract.ProgressOneMonth();
            CurrentMissionContractText.text = getCurrentMissionContractString();
        }
    }

    private string getCurrentMissionContractString()
    {
        return "MissionContract: Progress" + currentMissionContract.GetProgressPercentage().ToString("0.0") + "%";
    }

    private void updateOutputText()
    {
        var totalSalary = getEngineerSalaries() + getScientistSalaries();

        var report = 
            "-----------------------\n" +
            "New month\n" +
            "-----------------------\n" +
            "Last month stats:\n" +
            "\tFunds earned: TODO\n" +
            "\tSalary paid: " + totalSalary + "\n" +
            "\tResearch done: TODO\n" +
            "\tConstruction done: TODO\n";

        OutputText.text = report;
    }

    private void updateFundsText()
    {
        FundsText.text = "Funds: " + funds;
    }
} 
