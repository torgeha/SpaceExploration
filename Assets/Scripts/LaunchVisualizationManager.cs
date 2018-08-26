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

    private const float successReportDelay = 25;
    private const float failureReportDelay = 6;

	// Use this for initialization
	void Start () {

        launchTowerController = LaunchTower.GetComponent<LaunchTowerController>();

        //VisualizeMissionStart();
        //VisualizeMissionProgress(0.3f);
        //VisualizeMissionProgress(0.6f);
        //VisualizeMissionProgress(0.91f);

        //VisualizeFailure();

        //VisualizeSuccess();
    }
	
	// Update is called once per frame
	void Update () {

        // Handle camera rotation
        if (rocketController != null && rocketController.shouldLaunchSuccess)
        {
            updateCameraRotation(Time.deltaTime);
        }
        else if (rocketController != null && rocketController.shouldResetCameraRotation)
        {
            resetCameraRotation(Time.deltaTime);
            launchTowerController.ResetArm();
        }
		
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

        if (progress > 29 && !rocketBottom.activeSelf)
        {
            rocketBottom.SetActive(true);
        }
        if (progress > 59 && !rocketMid.activeSelf)
        {
            rocketMid.SetActive(true);
        }
        if (progress > 90 && !rocketTop.activeSelf)
        {
            rocketTop.SetActive(true);
        }
    }

    public void VisualizeSuccess()
    {
        // Rocket flies up

        rocketController.StartSuccessLaunch();
        launchTowerController.RetractArm();

        IEnumerator coroutine = showLaunchReportDelayed(successReportDelay, true);
        StartCoroutine(coroutine);
    }

    public void VisualizeFailure()
    {
        // rocket crashes
        rocketController.StartFailureLaunch();

        IEnumerator coroutine = showLaunchReportDelayed(failureReportDelay, false);
        StartCoroutine(coroutine);
    }

    private void updateCameraRotation(float deltatime)
    {
        var camera = Camera.main;

        var targetPosition = new Vector3(
            0,
            rocketParent.transform.position.y,
            rocketParent.transform.position.z);

        var targetRotation = Quaternion.LookRotation(targetPosition - camera.transform.position);

        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, deltatime);
    }

    private void resetCameraRotation(float deltatime)
    {
        var camera = Camera.main;
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, Quaternion.identity, deltatime);
    }

    private IEnumerator showLaunchReportDelayed(float delayInSeconds, bool success)
    {
        yield return new WaitForSeconds(delayInSeconds);
        if (success)
        {
            gameManager.HandleMissionContractSuccess();
        }
        else
        {
            gameManager.HandleMissionContractFailure();
        }
    }
}
