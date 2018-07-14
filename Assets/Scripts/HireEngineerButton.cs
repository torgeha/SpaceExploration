using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class HireEngineerButton : MonoBehaviour {

    public Text HireEngineerButtonText;

    private EngineerStaff engineerStaff;
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

    public void Setup(EngineerStaff engineer)
    {
        engineerStaff = engineer;
        UpdateButtonText();
    }

    public void UpdateButtonText()
    {

        Debug.Log("Setting text on eng button");
        HireEngineerButtonText.text =
            "Prof: " + engineerStaff.Proficiency +
            " - Prod: " + engineerStaff.Productivity.ToString("0.00") +
            " - Sal: " + engineerStaff.Salary;
    }


    public void OnHireEngineeringStaff()
    {
        gameManager.OnHireEngineer(engineerStaff);
        Destroy(gameObject); // Remove button from available hires
    }
}
