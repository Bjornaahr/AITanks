using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{ 
    Rigidbody playerModel;                                                      // The player rigidBody object
    Camera cameraElement;                                                       // The camera
    // Public values related to camera movement
    public float jumpingSpeed = 20.0f;                                          // jumping speed
    public float walkingAcceleration = 30.0f, runningAcceleration = 45.0f;      // Acceleration of movement
    public float cameraAngleX = 0, cameraAngleY = 0;                            // Starting orientation of camera
    public float horizontalAngularSpeed = 1.5f, verticalAngularSpeed = 1.5f;    // vertical and horizontal rotaiton speeds
    public float minimumVerticalTilt = -60,  maximumVerticalTilt = 90;          // Angular tilt limits
    public float maximumWalkingVelocity = 5.0f, maximumRunningVelocity = 10.0f; // Maximum velocity
    public float maxZoom = 70.0f, minZoom = 60.0f;                              // Zoom values
    public bool reverseVertical = false;                                        // bool determining if vertical inout should be reversed
    public float zoomSpeed = 15.0f;                                             // Speed of zooming
    public bool isRunning;                                                      // Bool indicating if they are running

    // private values
    float currentZoom;

    // Start is called before the first frame update
    void Start()
    {
        // Gets elements
        playerModel = GetComponent<Rigidbody>();
        cameraElement = GetComponent<Camera>();
        currentZoom = minZoom;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets x and y angle of direction and updates the relevant values
        cameraAngleX += Input.GetAxis("Mouse X");   // Adds horizontal mouse movement
        float newAngle = cameraAngleY + (reverseVertical ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y"));

        if (newAngle > maximumVerticalTilt)
            cameraAngleY = maximumVerticalTilt;
        else if (newAngle < minimumVerticalTilt)
            cameraAngleY = minimumVerticalTilt;
        else cameraAngleY = newAngle;

        // -- MOVEMENT --

        // Checks if the character is running
        isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // Ignores y direction for movement
        cameraElement.transform.rotation = Quaternion.Euler(0, cameraAngleX, 0);
        
        // figures out which speed should be used once rather than 
        float currentSpeed;

        // sets current speed and zoom
        if (isRunning)
        {
            currentSpeed = runningAcceleration;

            if (currentZoom < maxZoom)
            {
                currentZoom += zoomSpeed * Time.deltaTime;
                if (currentZoom > maxZoom)
                    currentZoom = maxZoom;
            }
        }
        else
        {
            currentSpeed = walkingAcceleration;

            if (currentZoom > minZoom)
            {
                currentZoom -= zoomSpeed * Time.deltaTime;
                if (currentZoom < minZoom)
                    currentZoom = minZoom;
            }
        }

        cameraElement.fieldOfView = currentZoom;    // Updates zoom value

        // Gets inputs and updates speed
        if (Input.GetKey(KeyCode.W))                                    // Forwards
        {
            playerModel.velocity += currentSpeed * transform.forward * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))                                    // Backwards
        {
            playerModel.velocity -= currentSpeed * transform.forward * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))                                    // Left
        {
            playerModel.velocity -= currentSpeed * transform.right * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))                                    // Right
        {
            playerModel.velocity += currentSpeed * transform.right * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space)) // up
        {
            playerModel.velocity += jumpingSpeed * transform.up * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl)) // down
        {
            playerModel.velocity -= jumpingSpeed * transform.up * Time.deltaTime;
        }
        // wasd is currently NOT being detected, I have forgotten why that is, google it is

        // Sets y value into temporary value
        float yValue = playerModel.velocity.y;          // Gets y
        Vector3 tempZeroValue = playerModel.velocity;   // Gets full velocity value
        tempZeroValue.y = 0;                            // sets y to 0 in temp value

        // Checks if maximum velocity have been reached and sets acceleration to 0 if it is larger then 0 already
        if (tempZeroValue.magnitude > (isRunning ? maximumRunningVelocity : maximumWalkingVelocity))
        {
            tempZeroValue = tempZeroValue.normalized * (isRunning ? maximumRunningVelocity : maximumWalkingVelocity);   // Sets speed to max if it is above maximum
        }

        tempZeroValue.y = yValue;               // Re-integrates the y Valye
        playerModel.velocity = tempZeroValue;   // sets velocity to the updated velocity

        // Updates camera angle
        cameraElement.transform.rotation = Quaternion.Euler(cameraAngleY, cameraAngleX, 0);
    }
}
