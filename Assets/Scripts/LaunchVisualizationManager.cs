using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchVisualizationManager : MonoBehaviour {

    public GameObject VisualizationParent;
    public GameObject RocketPrefab;
    public GameObject LaunchTower;

    private GameManager gameManager;

    private GameObject rocketParent;
    private GameObject rocketTop;
    private GameObject rocketMid;
    private GameObject rocketBottom;

    private RocketController rocketController;
    private LaunchTowerController launchTowerController;

	// Use this for initialization
	void Start () {

        launchTowerController = LaunchTower.GetComponent<LaunchTowerController>();

        VisualizeMissionStart();
        VisualizeMissionProgress(0.3f);
        VisualizeMissionProgress(0.6f);
        VisualizeMissionProgress(0.91f);

        VisualizeSuccess();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Setup(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void VisualizeMissionStart()
    {
        var camera = Camera.main;
        // Init rocket visuals
        rocketParent = Instantiate(RocketPrefab, VisualizationParent.transform, false);
        rocketBottom = rocketParent.transform.Find("RocketBottom").gameObject;
        rocketMid = rocketParent.transform.Find("RocketMid").gameObject;
        rocketTop = rocketParent.transform.Find("RocketTop").gameObject;

        rocketController = rocketParent.GetComponent<RocketController>();
    }

    public void VisualizeMissionProgress(float progress)
    {
        // add more parts to rocket based on progress

        if (progress > 0.29 && !rocketBottom.activeSelf)
        {
            rocketBottom.SetActive(true);
        }
        if (progress > 0.59 && !rocketMid.activeSelf)
        {
            rocketMid.SetActive(true);
        }
        if (progress > 0.9 && !rocketTop.activeSelf)
        {
            rocketTop.SetActive(true);
        }
    }

    public void VisualizeSuccess()
    {
        // Rocket flies up

        rocketController.StartSuccessLaunch();
        launchTowerController.RetractArm();

        // TODO: call gamemanager to trigger launch report
    }

    public void VisualizeFailure()
    {
        // rocket crashes
        rocketController.StartFailureLaunch();

        // TODO: call gamemanager to trigger launch report
    }
}
