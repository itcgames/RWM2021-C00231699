using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Author: Robin Meyler -> 2020/2021


public class DynamicCameraController : MonoBehaviour
{
    // General
    public GameObject Player;

    // Point of focus
    public bool usePointOfFocus = false;
    public GameObject PointOfFocus;

    // Camera locking
    public bool lockXAxis = false;
    public bool lockYAxis = false;

    // Boundary
    public bool useBoundaryPositions = false;
    public Vector2 boundaryTopLeft;
    public Vector2 boundaryBottomRight;

    // Follow codes
    public bool useLazyCamera = false;
    [Range(0, 100)] public float followSpeedPercentage;
    private float inverseSpeed;

    // Shake Code
    public bool useCameraShake = false;
    [Range(0, 1)] public float trauma;
    [Range(0, 1)] public float traumaReductionRate;
    private float shake;
    public float maxPositionOffset;
    public float maxAngleOffset;
    private Vector3 lockedCameraPostion;

    // Player velocity 
    public bool velocityAdjust = false;
    public float minVelocityAdjust;

    public void Contruct(GameObject player, GameObject enemy = null)
    {
        Player = player;
        PointOfFocus = enemy;
    }

    // Start is called before the first frame update
    public void Start()
    {
        lockedCameraPostion = transform.position;
        transform.position = new Vector3(lockedCameraPostion.x, lockedCameraPostion.y, transform.position.z);
        followSpeedPercentage = followSpeedPercentage / 100;
        inverseSpeed = 1.0f - followSpeedPercentage;
    }

    // Update is called once per frame
    public void Update()
    {
        // Lazy Camera Following
        if (useLazyCamera == true)
        {
            follow();       // Follow player based on inputs
        }
        else
        {
            transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);
        }

        // Player velocity
        if(velocityAdjust == true && useLazyCamera == false)
        {
            adjustForVelocity();
        }


        // Evaluate Point og focus
        if (usePointOfFocus == true)
        {
            if (PointOfFocus != null)
            {
                evaluatePointOfFocus();
            }
        }

        // Boundary Check
        if (useBoundaryPositions == true)
        {
            boundaryCheck();
        }

        // Camera Shake
        if (useCameraShake == true)
        {
            applyCameraShake();     // Apply shake on trauma
        }
    }

    public void follow()
    {
        // Asymototic smoothing
        Vector3 currentCameraPosition = transform.position;
        Vector3 currentPlayerPosition = Player.transform.position;

        // Start position
        float newX = lockedCameraPostion.x;
        float newY = lockedCameraPostion.y;

        // If axis is not locked
        if (lockXAxis == false)
        {
            newX = transform.position.x;        // Start at current position
            float targetX = currentPlayerPosition.x;        // Player
            newX = (inverseSpeed * newX) + (followSpeedPercentage * targetX);      // Move to it by %
        }
        if (lockYAxis == false)
        {
            newY = transform.position.y;
            float targetY = currentPlayerPosition.y;
            newY = (inverseSpeed * newY) + (followSpeedPercentage * targetY);
        }

        // Apply new positions
        currentCameraPosition = new Vector3(newX, newY, transform.position.z);
        transform.position = currentCameraPosition;
    }

    public void boundaryCheck()
    {
        Vector3 newCamPos = transform.position;
        if (newCamPos.x < boundaryTopLeft.x)
            newCamPos.x = boundaryTopLeft.x;
        if (newCamPos.y > boundaryTopLeft.y)
            newCamPos.y = boundaryTopLeft.y;
        if (newCamPos.x > boundaryBottomRight.x)
            newCamPos.x = boundaryBottomRight.x;
        if (newCamPos.y < boundaryBottomRight.y)
            newCamPos.y = boundaryBottomRight.y;
        transform.position = newCamPos;
    }
    public void applyCameraShake()
    {
        // Decrement the shake (Stop it shaking forever)
        trauma -= (traumaReductionRate / 100);

        // Keep in range
        if (trauma > 1.0f)
        {
            trauma = 1.0f;
        }
        else if (trauma < 0.0f)
        {
            trauma = 0.0f;
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
        Vector3 newCameraAngle = currentCameraAngle + new Vector3(0, 0, offsetAngle);
        Vector3 newCameraPosition = currentCameraPosition + new Vector3(offsetX, offsetY, 0);

        // Apply
        transform.position = newCameraPosition;
        transform.eulerAngles = newCameraAngle;
    }

    public void evaluatePointOfFocus()
    {
        Vector3 focusPos = PointOfFocus.transform.position;
        Vector3 currentPos = Player.transform.position;
        Vector3 difference = focusPos - currentPos;

        // Size of the ortographic camera
        float height = 2f * GetComponent<Camera>().orthographicSize;
        float width = height * GetComponent<Camera>().aspect;

        Vector3 allowedDistance = new Vector3(width - (0.4f * width), height - (0.4f * height), 0);

        // If the player is close enough to the point of focus
        if (difference.sqrMagnitude < allowedDistance.sqrMagnitude)
        {
            if (useLazyCamera == true)
            {
                smoothAdjustment(difference);       // Smoother adjustment
            }
            else
            {
                transform.position = new Vector3(Player.transform.position.x + difference.x / 2, Player.transform.position.y + difference.y / 2, transform.position.z);     // Snappy adjustment
            }
        }
    }
    public void smoothAdjustment(Vector3 t_difference)
    {
        // Figorue out where we want to be
        Vector3 finalPosition = new Vector3(Player.transform.position.x + t_difference.x / 2, Player.transform.position.y + t_difference.y / 2, transform.position.z);

        // Move toward it with the same speed as the lazy cam for consistancy
        float newX = (inverseSpeed * transform.position.x) + (followSpeedPercentage * finalPosition.x);      // Move to it by %
        float newY = (inverseSpeed * transform.position.y) + (followSpeedPercentage * finalPosition.y);

        // Apply new value
        transform.position = new Vector3(newX, newY, transform.position.z);
    }


    public void adjustForVelocity()
    {
        // Get the player velocity
        Vector3 velocityOfPlayer = Player.GetComponent<Rigidbody2D>().velocity;

        // If the players velocity is higher the minimum threshold 
        if(Mathf.Abs(velocityOfPlayer.x) > minVelocityAdjust)
        {
            float adjustValue =  minVelocityAdjust / Mathf.Abs(velocityOfPlayer.x);
            Vector3 newPosition = transform.position + new Vector3(velocityOfPlayer.x * (1 -adjustValue), 0, 0);
            if(transform.position.x != newPosition.x)
            {
                float newX = (0.80f * transform.position.x) + (0.2f * newPosition.x);      // Move to it by %
                float newY = (0.80f * transform.position.y) + (0.2f * newPosition.y);
                transform.position = new Vector3(newX, newY, transform.position.z);
            }          
        }
    }

    public void smoothVelocityAdjustment()
    {

    }
}
