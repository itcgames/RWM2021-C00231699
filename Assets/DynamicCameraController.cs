using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraController : MonoBehaviour
{
    public GameObject Player;
    public Vector2 PermanentOffset = new Vector2(-1,0);
    public float MaxSpeed = 1;
    public float CameraDelay = 0;
    public float CameraAcceleration = 1;

    private Vector3 lastPlayerPosition;
    private bool CaughtPlayer = true;
    private bool runOnce = false;
    private float timerCounter = 0;
    private Vector2 catchUpVelocity;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentPlayerPosition = Player.transform.position;
        currentPlayerPosition = currentPlayerPosition - new Vector3(PermanentOffset.x, PermanentOffset.y, 0);
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
    void Update()
    {
        setCamera();
    }

    void setCamera()
    {
        Vector3 currentCameraPosition = transform.position;
        Vector3 currentPlayerPosition = Player.transform.position;
        Vector3 differenceVector = currentPlayerPosition - lastPlayerPosition;
        Vector3 newCamPos = new Vector3(0,0,0);

        Vector2 newDiff = new Vector2(differenceVector.x, differenceVector.y);
        if (newDiff.magnitude > 1 && runOnce == false)
        {
            runOnce = true;
            timerCounter = CameraDelay;
            CaughtPlayer = false;
            catchUpVelocity = new Vector2(0.001f, 0.001f);
        }

        if(true == CaughtPlayer)
        {
            runOnce = false;
            catchUpVelocity = new Vector2(0, 0);
            //currentPlayerPosition = currentPlayerPosition - new Vector3(PermanentOffset.x, PermanentOffset.y, 0);
            //newCamPos.x = currentPlayerPosition.x;
            //newCamPos.y = currentPlayerPosition.y;
            newCamPos.z = transform.position.z;
        }

        else if(false == CaughtPlayer)
        {
            timerCounter -= 0.01f;
            if(timerCounter <= 0.0f)
            {
                newDiff.Normalize();

                // Up to a max speed- get magnitude and compare it
                if ((catchUpVelocity * CameraAcceleration).magnitude < MaxSpeed)
                {
                    catchUpVelocity = catchUpVelocity * CameraAcceleration;  // have a rolling velocity,  its set to 0, then increases based on the user added Acceleration
                }
                newDiff = newDiff + catchUpVelocity;
                newCamPos.x = currentCameraPosition.x + differenceVector.x;
                newCamPos.y = currentPlayerPosition.y + differenceVector.y;
                newCamPos.z = transform.position.z;
            }
        }

        transform.position = newCamPos;
        lastPlayerPosition = Player.transform.position;
    }
}
