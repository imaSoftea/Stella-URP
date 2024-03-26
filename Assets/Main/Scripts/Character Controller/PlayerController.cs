using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    public enum MovementState
    {
        Default,
        Gliding
    }

    // Essentials
    public Rigidbody rb;
    public MovementState state;
    private bool touchingGround = true;
    public float speedBuffer = 2.0f;
    private Vector3 previousPosition;


    // Default Movement
    public float initialAcceleration = 5.0f;
    public float maxSpeedMin = 16.0f;
    private float maxSpeed;
    public float gravityValue = -9.81f;
    public float jumpHeight = 2.0f;
    public Transform cameraTransform;
    private bool jumping = false;

    // Gliding
    public float wallDetectionDistance = 1f;
    public float glideAcceleration = 3.0f;
    public LayerMask wallLayerMask; 
    private Vector3 wallNormal;
    private float maxGlideAngle = 30.0f;
    private float wallForce = 2.0f;

    // UI
    public TextMeshProUGUI speedText;

    private void Start()
    {
        maxSpeed = maxSpeedMin;
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, gravityValue, 0);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        StartCoroutine(UpdateUI());
    }

    void Update()
    {

        touchingGround = (Physics.Raycast(transform.position, -Vector3.up, 1.1f));
        DetermineState();

        switch(state)
        {
            case MovementState.Default:
                DefaultMovement();
                break;
            case MovementState.Gliding:
                Glide();
                break;
        }

        MaxSpeedCheck();
    }

    void DefaultMovement()
    {
        Vector3 move = new Vector3(0, 0, Input.GetAxis("Vertical"));
        move.Normalize();

        if (touchingGround && !jumping)
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumping = true;
                StartCoroutine(ResetJump());
                float jumpForce = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
        }

        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        if(CheckAngle(flatForward, new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized, 70))
        {
            float rotationAmount = Input.GetAxis("Horizontal") * 160 * Time.deltaTime;
            Quaternion rotation = Quaternion.Euler(0, rotationAmount, 0);
            rb.velocity = rotation * rb.velocity;
        }

        move = flatForward * move.z;

        rb.AddForce(move * initialAcceleration * 10 * Time.deltaTime);
    }

    void Glide()
    {

        if(jumping) return;

        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
            StartCoroutine(ResetJump());
            float jumpForce = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            rb.AddForce((Vector3.up + wallNormal).normalized * jumpForce, ForceMode.VelocityChange);
        }

        // Perform additional raycasts to detect wall curvature
        DetectWallCurvature();

        // Calculate the direction vector on the XZ plane orthogonal to the wall normal
        Vector3 directionOnXZPlane = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 orthogonalToWallOnXZ = Vector3.Cross(wallNormal, Vector3.up).normalized;
        Vector3 glideDirection = Vector3.Project(directionOnXZPlane, orthogonalToWallOnXZ).normalized;

        float glideSpeed = rb.velocity.magnitude + glideAcceleration * Time.deltaTime;
        maxSpeed += glideAcceleration * Time.deltaTime;

        // Update the Rigidbody's velocity, maintaining the current vertical velocity
        rb.velocity = new Vector3(glideDirection.x * glideSpeed, 0, glideDirection.z * glideSpeed) + (-1 * wallForce * wallNormal);
    }

    void DetectWallCurvature()
    {
        RaycastHit hitInfo;
        float detectionRadius = 5.0f; // Adjust based on character size and expected wall curvature
        int raysToCast = 5; // Number of rays to cast in a semicircle facing the movement direction

        Vector3 direction = rb.velocity.normalized;
        Vector3 startDirection = Quaternion.AngleAxis(-45, Vector3.up) * direction; // Start from -45 degrees to the movement direction

        Vector3 averageNormal = Vector3.zero;
        int hitCount = 0;

        for (int i = 0; i < raysToCast; i++)
        {
            Vector3 rayDirection = Quaternion.AngleAxis((90.0f / (raysToCast - 1)) * i, Vector3.up) * startDirection;
            if (Physics.Raycast(transform.position, rayDirection, out hitInfo, detectionRadius, wallLayerMask))
            {
                averageNormal += hitInfo.normal;
                hitCount++;
                Debug.DrawRay(transform.position, rayDirection * detectionRadius, Color.red); // For visualization
            }
        }

        if (hitCount > 0)
        {
            wallNormal = (averageNormal / hitCount).normalized;
        }
    }

    void DetermineState()
    {

        /* Checking For Glide */
        
        Vector3 forwardVector = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;
        // Rotate the forward vector 90 degrees clockwise around the y-axis to get the right vector
        Vector3 rightVector = Quaternion.Euler(0, 90, 0) * forwardVector;
        // Rotate the forward vector 90 degrees counterclockwise around the y-axis to get the left vector
        Vector3 leftVector = Quaternion.Euler(0, -90, 0) * forwardVector;

        Vector3 currentPosition = transform.position;
        Vector3 movementDirection = currentPosition - previousPosition; // Calculate the direction of movement
        float movementDistance = movementDirection.magnitude; // The distance moved since the last frame
        movementDirection.Normalize(); // Normalize to get a direction vector
        
        // Adjust 'numRays' as needed for accuracy vs performance
        int numRays = 5;
        float distanceBetweenRays = movementDistance / (numRays - 1);

        for (int i = 0; i < numRays; i++)
        {
            // Calculate ray origins based on the previous position and movement direction
            Vector3 rayOrigin = previousPosition + (distanceBetweenRays * i * movementDirection);

            Debug.DrawRay(rayOrigin, rightVector, Color.red, 2.0f);
            Debug.DrawRay(rayOrigin, leftVector, Color.red, 2.0f);

            // Check to the right
            if (!touchingGround && Physics.Raycast(rayOrigin, rightVector, out RaycastHit hit, wallDetectionDistance, wallLayerMask))
            {
                if (CheckAngle(leftVector, hit.normal, maxGlideAngle))
                {
                    state = MovementState.Gliding;
                    wallNormal = hit.normal;
                    
                    previousPosition = currentPosition;
                    return;
                }
            }

            // Check to the left
            if (!touchingGround && Physics.Raycast(rayOrigin, leftVector, out hit, wallDetectionDistance, wallLayerMask))
            {
                if (CheckAngle(rightVector, hit.normal, maxGlideAngle))
                {
                    state = MovementState.Gliding;
                    wallNormal = hit.normal;

                    previousPosition = currentPosition;
                    return;
                }
            }
        }
        previousPosition = currentPosition;

        /* Default State */
        state = MovementState.Default;
    }

    bool CheckAngle(Vector3 cast, Vector3  normal, float angle)
    {
        // Convert the angle to radians for Mathf.Cos
        float maxAngleCosine = Mathf.Cos(angle * Mathf.Deg2Rad);

        // Calculate the dot product between the hit normal and the direction vector
        float dotProduct = Vector3.Dot(normal.normalized, cast.normalized);

        // Check if the dot product is greater than the cosine of the maximum angle
        return dotProduct >= maxAngleCosine;
    }

    void MaxSpeedCheck()
    {
        Vector3 restrictedVelocity = new Vector3(rb.velocity.x, 0 , rb.velocity.z);
        if(restrictedVelocity.magnitude > maxSpeed)
        {
            restrictedVelocity.Normalize();
            restrictedVelocity *= maxSpeed;
            rb.velocity =  new Vector3(restrictedVelocity.x, rb.velocity.y, restrictedVelocity.z);
        }
        else if (restrictedVelocity.magnitude < (maxSpeed - speedBuffer))
        {
            maxSpeed = speedBuffer + restrictedVelocity.magnitude;
            if(maxSpeed < maxSpeedMin)
            {
                maxSpeed = maxSpeedMin;
            }
        }   
    }

    IEnumerator UpdateUI()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            float speedInMilesPerHour = rb.velocity.magnitude * 2.23694f; 
            speedText.text = speedInMilesPerHour.ToString("0");
        }
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.2f);
        jumping = false;
    }
}