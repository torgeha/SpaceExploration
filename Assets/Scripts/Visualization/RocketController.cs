using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour {

    private ParticleSystem particleTrail;
    private ParticleSystem particleFlame;
    private ParticleSystem particleExplosion;

    public float MaxVerticalSpeed = 400f;
    public float MaxHorizontalSpeed = 600f;
    public float AccelerationVertical = 50f;
    public float AccelerationHorizontal = 20f;
    public float Deceleration = 5f;

    private float verticalSpeed = 0f;
    private float horizontalSpeed = 0f;

    public bool shouldLaunchSuccess = false;
    public bool shouldLaunchFailure = false;
    public bool shouldResetCameraRotation = false;

    private float heightOfHorizontalAccelerationStart = 2000;

    private float logIntervalSeconds = 2;

    private void Awake()
    {
        particleTrail = transform.Find("ParticleTrail").GetComponentInChildren<ParticleSystem>();
        particleFlame = transform.Find("ParticleFlame").GetComponentInChildren<ParticleSystem>();
        particleExplosion = transform.Find("ParticleExplosion").GetComponentInChildren<ParticleSystem>();
        //StartCoroutine("logStuff");
        
    }

    // Update is called once per frame
    void Update ()
    {
		if (shouldLaunchSuccess)
        {
            progressSuccessFlight(Time.deltaTime);
            //updateCameraRotation(Time.deltaTime);
            checkIfDone();
        }

        else if (shouldLaunchFailure)
        {

        }
	}

    private void checkIfDone()
    {
        if (transform.position.x > 13000)
        {
            shouldLaunchSuccess = false;
            shouldResetCameraRotation = true;
            StartCoroutine("deleteSelfAfterDelay");
        }

        // TODO: check if failure is done, reset camera position.
    } 

    private IEnumerator deleteSelfAfterDelay()
    {
        yield return new WaitForSeconds(5.0f);

        //make sure camera is reset before deleting self
        Camera.main.transform.rotation = Quaternion.identity;
        Destroy(gameObject);
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
        IEnumerator coroutine = startLaunchDelayed(3.0f, true);
        StartCoroutine(coroutine);
    }

    public void StartFailureLaunch()
    {
        particleTrail.Play();
        particleFlame.Play();
        IEnumerator coroutine = startLaunchDelayed(3.0f, false);
        StartCoroutine(coroutine);
    }

    private IEnumerator startLaunchDelayed(float delayInSeconds, bool success)
    {
        yield return new WaitForSeconds(delayInSeconds);

        if (success == true)
        {
            shouldLaunchSuccess = true;
        }
        else
        {
            particleFlame.Stop();
            particleTrail.Stop();
            particleExplosion.Play();

            Destroy(gameObject, particleExplosion.main.duration + 0.5f);
        }
    }
}
