using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchTowerController : MonoBehaviour {

    public float RetractionSpeed = 30;
    public float RotationSpeed = 50;

    private float maxRetractionTranslation = 40;
    private float maxRetractionRotationDegrees = 90;

    private GameObject towerArm;
    private bool shouldRetractArm = false;
    private bool shouldResetArm = false;

    private float cumRetracted = 0;
    private float cumRotatedDegrees = 0;

    private Vector3 initialTowerArmPosition = new Vector3(30, 30, 0);
    private Quaternion initialTowerArmRotation = Quaternion.identity;
    private float cumRetractionTime = 0;

	// Use this for initialization
	void Start () {
        towerArm = transform.Find("TowerArm").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
        if (shouldRetractArm)
        {
            var retractionDelta = (RetractionSpeed * Time.deltaTime);
            var rotationDelta = (RotationSpeed * Time.deltaTime);
            var retract = towerArm.transform.position.x - retractionDelta;
            //var rotate = towerArm.transform.eulerAngles.z + rotationDelta;

            var newPosition = new Vector3(retract, towerArm.transform.position.y, towerArm.transform.position.z);
            var newRotation = new Vector3(0, 0, rotationDelta);

            Debug.Log("euler: " + towerArm.transform.eulerAngles);
            Debug.Log("rotationDelta: " + rotationDelta);
            //Debug.Log("newpos: " + newPosition);
            Debug.Log("newRotation: " + newRotation);

            cumRetracted += retractionDelta;
            cumRotatedDegrees += rotationDelta;

            towerArm.transform.position = newPosition;
            towerArm.transform.Rotate(newRotation);

            //Debug.Log("cumretracted: " + cumRetracted);
            Debug.Log("cumrotatied" + cumRotatedDegrees);

            if (cumRetracted > maxRetractionTranslation || cumRotatedDegrees > maxRetractionRotationDegrees)
            {
                shouldRetractArm = false;
            }
        }
        //else if (shouldResetArm)
        //{
        //    cumRetractionTime += Time.deltaTime;
        //    towerArm.transform.localPosition = Vector3.Lerp(towerArm.transform.position, initialTowerArmPosition, cumRetractionTime);
        //    towerArm.transform.rotation = Quaternion.Lerp(towerArm.transform.rotation, initialTowerArmRotation, cumRetractionTime);

        //    if (cumRetractionTime >= 1.0f)
        //    {
        //        shouldResetArm = false;
        //        Debug.Log("done resetting");
        //    }
            
        //}

	}

    public void RetractArm()
    {
        IEnumerator coroutine = delayRetractArm(3.0f);
        StartCoroutine(coroutine);
    }

    public void ResetArm()
    {
        if (towerArm.transform.localPosition != initialTowerArmPosition)
        {
            towerArm.transform.localPosition = initialTowerArmPosition;
            towerArm.transform.rotation = initialTowerArmRotation;
        }
    }

    private IEnumerator delayRetractArm(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        shouldRetractArm = true;
    }
}
