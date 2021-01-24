using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraController : MonoBehaviour
{
    public GameObject Player;
    public Vector2 PermanentOffset = new Vector2(-1,0);
    public float MaxSpeed = 1;
    public float CameraDelay = 0;
    public bool AccelBasedOnDistance = false;
    public float CameraAcceleration = 1;
    public bool lockXAxis = false;
    public bool lockYAxis = false;
    public bool useBoundaryPositions = false;
    public Vector2 boundaryTopLeft;
    public Vector2 boundaryBottomRight;

    private Vector3 lastPlayerPosition;
    private bool CaughtPlayer = true;
    private bool runOnce = false;
    private float timerCounter = 0;
    private float catchUpVelocity;

    // Shake Code
    public bool useCameraShake;
    [Range(0, 1)] public float trauma;
    [Range(0, 1)] public float traumaReductionRate;
    private float shake;
    public float maxPositionOffset;
    public float maxAngleOffset;



    public void Contruct(GameObject player, float cameraDelay, float maxSpeed)
    {
        Player = player;
        CameraDelay = cameraDelay;
        MaxSpeed = maxSpeed;
    }
    // Start is called before the first frame update
    public void Start()
    {
        Vector3 currentPlayerPosition = Player.transform.position;
        //currentPlayerPosition = currentPlayerPosition - new Vector3(PermanentOffset.x, PermanentOffset.y, 0);
        Vector3 newCamPos;
        newCamPos.x = currentPlayerPosition.x;
        newCamPos.y = currentPlayerPosition.y;
        newCamPos.z = transform.position.z;
        transform.position = newCamPos;
        lastPlayerPosition = Player.transform.position;
        CaughtPlayer = true;
        runOnce = true;
        timerCounter = CameraDelay;
    }

    // Update is called once per frame
    public void Update()
    {
        setCamera();
        if (useCameraShake == true)
        {
            applyCameraShake();
        }
    }

    public void setCamera()
    {
        Vector3 currentCameraPosition = transform.position;
        Vector3 currentPlayerPosition = Player.transform.position;
        Vector3 differenceVector = currentPlayerPosition - currentCameraPosition;
        Vector3 newCamPos = currentCameraPosition;

        Vector2 newDiff = new Vector2(differenceVector.x, differenceVector.y);
        if(CameraDelay <= 0)
        {
            if(lockXAxis == false)
                newCamPos.x = currentPlayerPosition.x;
            if (lockYAxis == false)
                newCamPos.y = currentPlayerPosition.y;
        }
        else if (newDiff.magnitude < 0.05f)
        {
            if (lockXAxis == false)
                newCamPos.x = currentPlayerPosition.x;
            if (lockYAxis == false)
                newCamPos.y = currentPlayerPosition.y;
            runOnce = false;
            CaughtPlayer = true;
        }
        else if (newDiff.magnitude > 0.05f && runOnce == false)
        {
            runOnce = true;
            timerCounter = CameraDelay;
            CaughtPlayer = false;
            catchUpVelocity = 0;
        }
       
     
        if(false == CaughtPlayer)
        {
            timerCounter -= 0.01f;
            if(timerCounter <= 0.0f)
            {
                newDiff.Normalize();

                if ((catchUpVelocity + CameraAcceleration) <= MaxSpeed)
                {
                    catchUpVelocity += CameraAcceleration;
                }
                newDiff = newDiff * catchUpVelocity;

                if (lockXAxis == false)
                    newCamPos.x = currentCameraPosition.x + newDiff.x/1000;
                if (lockYAxis == false)
                    newCamPos.y = currentCameraPosition.y + newDiff.y/1000;
            }
        }

        if(useBoundaryPositions == true)
        {
            if(newCamPos.x < boundaryTopLeft.x)
                newCamPos.x = boundaryTopLeft.x;
            if (newCamPos.y > boundaryTopLeft.y)
                newCamPos.y = boundaryTopLeft.y;

            if (newCamPos.x > boundaryBottomRight.x)
                newCamPos.x = boundaryBottomRight.x;
            if (newCamPos.y < boundaryBottomRight.y)
                newCamPos.y = boundaryBottomRight.y;
        }

        newCamPos.z = transform.position.z;
        transform.position = newCamPos;
        lastPlayerPosition = Player.transform.position;
    }

    public float getVel()
    {
        return catchUpVelocity;    
    }


    public void applyCameraShake()
    {
        if(trauma > 1)
        {
            trauma = 1.0f;
        }
        // Decrement the shake (Stop it shaking forever)
        trauma -= (traumaReductionRate/100);
        if(trauma < 0)
        {
            trauma = 0;
        }

        // Shake is equal to the traum squared to give a smooth curve of shake
        shake = trauma * trauma;

        // Reset Camera rotation
        transform.eulerAngles = new Vector3(0, 0, 0);

        // Determine how much to offset the rotationa and translation
        float offsetAngle = maxAngleOffset * shake * Random.Range(-0.5f, 0.5f); 
        float offsetX = maxPositionOffset * shake * Random.Range(-0.5f, 0.5f);
        float offsetY = maxPositionOffset * shake * Random.Range(-1.0f, 1.0f); 
   
        // Get current state
        Vector3 currentCameraAngle = transform.eulerAngles;
        Vector3 currentCameraPosition = transform.position;

        // Current state + shake
        Vector3 newCameraAngle = currentCameraAngle + new Vector3( 0, 0, offsetAngle);
        Vector3 newCameraPosition = currentCameraPosition + new Vector3(offsetX, offsetY, 0);

        // Apply
        transform.position = newCameraPosition;
        transform.eulerAngles = newCameraAngle;
    }
}
