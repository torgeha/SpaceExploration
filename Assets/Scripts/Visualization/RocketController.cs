using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour {

    private ParticleSystem particleTrail;
    private ParticleSystem particleFlame;

    public float MaxVerticalSpeed = 400f;
    public float MaxHorizontalSpeed = 600f;
    public float AccelerationVertical = 50f;
    public float AccelerationHorizontal = 20f;
    public float Deceleration = 5f;

    private float verticalSpeed = 0f;
    private float horizontalSpeed = 0f;

    private bool shouldLaunchSuccess = false;
    private bool shouldLaunchFailure = false;

    private float heightOfHorizontalAccelerationStart = 2000;

    private float logIntervalSeconds = 2;

    private void Awake()
    {
        particleTrail = transform.Find("ParticleTrail").GetComponentInChildren<ParticleSystem>();
        particleFlame = transform.Find("ParticleFlame").GetComponentInChildren<ParticleSystem>();
        //StartCoroutine("logStuff");
        
    }

    // Update is called once per frame
    void Update ()
    {
		if (shouldLaunchSuccess)
        {
            progressSuccessFlight(Time.deltaTime);
        }

        else if (shouldLaunchFailure)
        {

        }


        // TODO: stop moving and destroy rocket when it is too high
	}

    private IEnumerator logStuff()
    {
        while(isActiveAndEnabled)
        {
            yield return new WaitForSeconds(logIntervalSeconds);
            Debug.Log("VerticalSpeed: " + verticalSpeed);
            Debug.Log("HorizontalSpeed: " + horizontalSpeed);
        }
    }

    private void progressSuccessFlight(float deltatime)
    {
        var position = transform.position;

        if (position.y > heightOfHorizontalAccelerationStart)
        {
            // Add horizontal speed
            if (horizontalSpeed < MaxHorizontalSpeed)
            {
                // Add horizontal speed and decrease vertical speed
                horizontalSpeed = horizontalSpeed + (AccelerationHorizontal * deltatime);
            }
            // Decrease vertical speed
            if (verticalSpeed > 20)
            {
                verticalSpeed = verticalSpeed - (Deceleration * deltatime);
                verticalSpeed = Mathf.Max(0, verticalSpeed); // Do not obtain negative vertical speed
            }
        }
        else
        {
            if (verticalSpeed < MaxVerticalSpeed)
            {
                verticalSpeed = verticalSpeed + (AccelerationVertical * deltatime);
            }
        }

        var newPos = new Vector3(
            transform.position.x + (horizontalSpeed * deltatime), 
            transform.position.y + (verticalSpeed * deltatime), 
            transform.position.z);


        transform.position = newPos;

        // only apply rotation when having horizontal speed
        if (horizontalSpeed > 0)
        {
            var newRotation = Quaternion.FromToRotation(Vector3.up, new Vector3(horizontalSpeed, verticalSpeed, 0));
            transform.rotation = newRotation;
        }
    }

    public void StartSuccessLaunch()
    {
        particleTrail.Play();
        particleFlame.Play();
        IEnumerator coroutine = startLaunchDelayed(3.0f);
        StartCoroutine(coroutine);
    }

    public void StartFailureLaunch()
    {
        shouldLaunchFailure = true;
    }

    private IEnumerator startLaunchDelayed(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        shouldLaunchSuccess = true;
    }
}
