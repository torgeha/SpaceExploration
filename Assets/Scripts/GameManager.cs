using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject MainCanvas;
    public GameObject HiresCanvas;
    public GameObject MissionsCanvas;
    public GameObject LaunchSummaryCanvas;

    public Text TurnText;
    public Text FundsText;
    public Text ExpencesText;
    
    public Text OverviewText;
    public Text HiresText;

    public GameObject MissionsAvailablePanel;
    public GameObject EngineerHiresAvailablePanel;
    public Text CurrentMissionSummaryText;
    public Text CurrentMissionFullText;

    //public GameObject MissionContractButtonPrefab;
    public GameObject AvailableMissionRowPanel;
    //public GameObject HireEngineerButtonPrefab;
    public GameObject HireEngineerRowPanel;

    private LaunchVisualizationManager launchVisualizationManager;

    private long funds;
    private int month;

    private const int totalAvailableMissionContracts = 5;
    private const int totalAvailableEngineersForHire = 5;

    private List<GameObject> missionContractRows = new List<GameObject>();
    private List<GameObject> hireEngineerRows = new List<GameObject>();

    private List<EngineerStaff> hiredEngineers = new List<EngineerStaff>();
    private List<ScientistStaff> hiredScientists = new List<ScientistStaff>();
    private List<MissionContractTask> availableMissionContracts = new List<MissionContractTask>();
    private List<EngineerStaff> availableEngineersForHire = new List<EngineerStaff>();

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
        OverviewText.text = "New game\n";

        resetAvailableMissionContracts();
        resetAvailableEngineersForHire();

        launchVisualizationManager = GetComponentInChildren<LaunchVisualizationManager>();
        launchVisualizationManager.Setup(this);
    }
	
    public void OnProcessNextRound()
    {
        increaseMonth();
        updateFunds();
        updateResearchProgress();
        updateMissionContractProgress();

        resetAvailableEngineersForHire();

        updateOutputText();
    }

    public void OnOpenHireCanvas()
    {
        MainCanvas.SetActive(false);
        HiresCanvas.SetActive(true);
    }

    public void OnOpenMissionCanvas()
    {
        MainCanvas.SetActive(false);
        MissionsCanvas.SetActive(true);
    }

    public void OnOpenMainCanvas()
    {
        HiresCanvas.SetActive(false);
        MissionsCanvas.SetActive(false);
        LaunchSummaryCanvas.SetActive(false);
        MainCanvas.SetActive(true);
    }

    public void OnHireScientist()
    {
        var scientist = new ScientistStaff(10, 10, 5);
        hiredScientists.Add(scientist);
        updateHiresText();

        // TODO: set proficiency modifier
    }

    public void OnHireEngineer(EngineerStaff engineer)
    {
        hiredEngineers.Add(engineer);
        availableEngineersForHire.Remove(engineer);

        updateHiresText();
        CurrentMissionFullText.text = getCurrentMissionFullString();
        ExpencesText.text = "Expenses: " + getMonthlySalaryExpenses();
        // Update engineering proficiency modifier
        //var engineeringQuality = getEngineeringMaxProficiency();
        //EngineeringProficiencyText.text = "Eng prof: " + engineeringProficiencyModifier;

        updateMissionContractButtonTexts();
    }

    public void OnStartMissionContract(MissionContractTask mc)
    {
        if (currentMissionContract != null)
        {
            OverviewText.text = "Mission contract already in progress, only one allowed at a time.";
            return;
        }

        if (hiredEngineers.Count == 0)
        {
            OverviewText.text = "Cannot start mission contract without engineers.";
            return;
        }

        currentMissionContract = mc;
        CurrentMissionSummaryText.text = getCurrentMissionSummaryString();
        CurrentMissionFullText.text = getCurrentMissionFullString();

        // Remove available contracts
        deleteAvailableMissionContracts();
    }

    public void OnLaunchMission()
    {
        // Can only launch mission if a mission is complete
        if (currentMissionContract == null || !currentMissionContract.IsComplete())
        {
            OverviewText.text = "Cannot launch mission without a completed Mission Contract";
            return;
        }

        // Find probability of success
        var successProbability = GetMissionContractSuccessProbability(currentMissionContract);
        Debug.Log("Calciing success...");
        Debug.Log("succprob: " + successProbability);
        var random = Random.Range(0.0f, 1.0f);
        Debug.Log("random: " + random);

        CurrentMissionSummaryText.text = "No Current Mission Contracts";
        CurrentMissionFullText.text = "No Current Mission Contracts";
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
        var successProbability = getEngineeringMaxProficiency() / (float)mc.Complexity;
        successProbability *= engineeringProficiencyModifier;
        return successProbability;
    }

    public float GetEngineeringManMonthProgress()
    {
        // Get man months, based on poductivity of engineers
        var manMonths = 0.0f;
        foreach (var eng in hiredEngineers)
        {
            manMonths += eng.Productivity;
        }

        return manMonths;
    }

    private void resetAvailableEngineersForHire()
    {
        deleteHireableEngineers();

        for (var i = 0; i < totalAvailableEngineersForHire; i++)
        {
            // Create a random engineer
            var proficiencyRand = Random.Range(10, 100);
            var productivityRand = Random.Range(0.5f, 2.0f); // productivity of 1 means this engineer delivers 1 man month in 1 month

            var proficiency = (int)proficiencyRand; // 10-100 proficiency
            var productivity = productivityRand;

            var salary = (proficiency * productivity);
            var salaryRand = (int)Random.Range(salary * -0.5f, salary * 0.5f);
            salary = salary + salaryRand; // based on proficiency and productivity +- half itself to add some randomness

            var eng = new EngineerStaff(proficiency, productivity, (int)salary);
            availableEngineersForHire.Add(eng);

            // Create hire engineer row
            var hireEngineerRow = Instantiate(HireEngineerRowPanel);
            hireEngineerRow.transform.SetParent(EngineerHiresAvailablePanel.transform, false);
            hireEngineerRow.GetComponentInChildren<HireEngineerButton>().Setup(eng);
            hireEngineerRows.Add(hireEngineerRow);
        }
    }

    private void deleteHireableEngineers()
    {
        if (hireEngineerRows.Count < 1)
            return;

        for (int i = 0; i < totalAvailableEngineersForHire; i++)
        {
            Destroy(hireEngineerRows[i]);
        }

        hireEngineerRows = new List<GameObject>();
        availableEngineersForHire = new List<EngineerStaff>();
    }

    private void resetAvailableMissionContracts()
    {
        deleteAvailableMissionContracts();

        for (var i = 0; i < totalAvailableMissionContracts; i++)
        {
            // Create a random mission contract
            var durationRand = Random.Range(1, 50); 
            var complexityRand = Random.Range(10, 100);

            var duration = (int)durationRand; // 1-50 man-months (based on productivity of engineers)
            var complexity = (int)complexityRand; // 10-100

            var reward = (duration * complexity);
            var rewardRand = (int)Random.Range(reward * -0.5f, reward * 0.5f);
            reward = reward + rewardRand; // based on complexity and duration +- half itself to add some randomness

            var mc = new MissionContractTask(duration, complexity, reward);
            availableMissionContracts.Add(mc);

            // Create mission rows
            var missionTaskRow = Instantiate(AvailableMissionRowPanel);
            missionTaskRow.transform.SetParent(MissionsAvailablePanel.transform, false);
            missionTaskRow.GetComponentInChildren<TakeMissionContractButton>().Setup(mc);
            missionContractRows.Add(missionTaskRow);
        }
    }

    private void deleteAvailableMissionContracts()
    {
        if (missionContractRows.Count < 1)
            return;

        for (int i = 0; i < totalAvailableMissionContracts; i++)
        {
            Destroy(missionContractRows[i]);
        }

        missionContractRows = new List<GameObject>();
        availableMissionContracts = new List<MissionContractTask>();
    }

    private void updateMissionContractButtonTexts()
    {
        foreach(var row in missionContractRows)
        {
            row.GetComponentInChildren<TakeMissionContractButton>().UpdateButtonText();
        }
    }

    private void handleMissionContractSuccess()
    {
        funds += currentMissionContract.Value;
        OverviewText.text = "Success! You have been rewarded " + currentMissionContract.Value + " funds!";
        updateFundsText();
        currentMissionContract = null;
    }

    private void handleMissionContractFailure()
    {
        OverviewText.text = "Failure! The customer will not pay for failures :(";
        currentMissionContract = null;
    }

    private int getEngineeringMaxProficiency()
    {
        // Get the max proficiency of engineers

        // OBS can be done with linq expression?!
        var maxProf = 0;
        foreach(var e in hiredEngineers)
        {
            if (e.Proficiency > maxProf)
                maxProf = e.Proficiency;
        }
        return maxProf;
    }

    private void updateHiresText()
    {
        var hiresString = "";
        foreach(var s in hiredScientists)
        {
            hiresString += s + "\n";
        }
        for(var i = 0; i < hiredEngineers.Count; i++)
        {
            hiresString += "E" + (i + 1) + ": " + hiredEngineers[i] + "\n";
        }

        HiresText.text = hiresString;
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
        foreach(var scientist in hiredScientists)
        {
            salaries += scientist.Salary;
        }
        return salaries;
    }

    private int getEngineerSalaries()
    {
        var salaries = 0;
        foreach (var engineer in hiredEngineers)
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
            var manMonthsProgressed = GetEngineeringManMonthProgress();
            Debug.Log("Engineering man months progressed: " + manMonthsProgressed);
            currentMissionContract.AddMonthsToProgress(manMonthsProgressed);
            CurrentMissionSummaryText.text = getCurrentMissionSummaryString();
            CurrentMissionFullText.text = getCurrentMissionFullString();
        }
    }

    private int getMonthlySalaryExpenses()
    {
        var expenses = 0;
        foreach(var e in hiredEngineers)
        {
            expenses += e.Salary;
        }
        return expenses;
    }

    private string getCurrentMissionSummaryString()
    {
        return "MissionContract: Progress " + currentMissionContract.GetProgressPercentage().ToString("0.0") + "%";
    }

    private string getCurrentMissionFullString()
    {
        if (currentMissionContract == null)
        {
            return "No mission in progress...";
        }

        var s =
            "Progress in man-months: " + currentMissionContract.ProgressInMonths.ToString("0.0") + "\n" +
            "Duration: " + currentMissionContract.DurationInMonths + "\n" +
            "Complexity: " + currentMissionContract.Complexity + "\n" +
            "Probability of success: " + (GetMissionContractSuccessProbability(currentMissionContract) * 100).ToString("0.0") + "%" + "\n" +
            "Payment if succsessful: " + currentMissionContract.Value;
        return s;
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

        OverviewText.text = report;
    }

    private void updateFundsText()
    {
        FundsText.text = "Funds: " + funds;
    }
} 
