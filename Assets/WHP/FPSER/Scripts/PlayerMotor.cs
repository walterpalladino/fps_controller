using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{

    private PlayerInput playerInput;
    private CharacterController characterController;


    [Header("Movement")]
    [SerializeField]
    private float walkForwardSpeed = 7.5f;
    [SerializeField]
    private float runSpeed = 10.0f;
    [SerializeField]
    private float walkBackwardSpeed = 4.0f;
    [SerializeField]
    private float strifeSpeed = 4.0f;

    [SerializeField]
    private float turnSpeed = 150f;


    [Header("Slope Control")]
    [SerializeField] private float slopeForce = 200;
    //[SerializeField] private float slopeForceRayLength;
    //[SerializeField] private bool onSlope;
    [SerializeField] private bool onWalkableSlope;
    [SerializeField] private float slopeAngle;


    [Header("Airborne Control")]
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool isFalling;

    [SerializeField]
    private float jumpHeight = 2.0f;

    [SerializeField]
    private float jumpSpeed = 8.0F;
    [SerializeField]
    private float gravity = 20.0F;
    [SerializeField]
    private float distToGround = 0.01f;
    private float rayDistance = 10.0f;
    [SerializeField]
    private float intervalBetweenJumps = 1.0f;
    private bool jumping;


    private Vector3 moveDirection = Vector3.zero;
    private float verticalSpeed = 0.0f;
    [SerializeField]
    private float lastJump = 0.0f;

    public float movementSpeed = 0.0f;



    [Header("Dash")]
    [SerializeField]
    private bool dashEnabled = false;
    [SerializeField]
    private float dashSpeed = 15.0f;
    [SerializeField]
    private float maxDashTime = 1.0f;
    private float currentDashTime;
    private bool dashActive;
    private Vector3 lastMoveDirection;


    [Header("Crouch")]
    [SerializeField]
    private bool isCrouched = false;
    [SerializeField]
    private float crouchHeight = 1.2f;
    [SerializeField]
    private float standHeight = 2f;
    [SerializeField]
    private float crouchSpeed;

    [SerializeField]
    private Transform cameraAnchor;
    private float cameraAnchorDefaultY;
    [SerializeField]
    private bool animating;
    [SerializeField]
    private float crouchSmooth = 5.0f;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        
        characterController = GetComponent<CharacterController>();
        standHeight = characterController.height;

        //audioSource = GetComponent<AudioSource>();

        currentDashTime = maxDashTime;

        cameraAnchorDefaultY = cameraAnchor.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {

        MovePlayer();

    }


    private void MovePlayer()
    {
        movementSpeed = 0.0f;

        isGrounded = IsGrounded();

        if (IsGrounded())
        {
            //  Reset Movement        
            verticalSpeed = -0.1f;  //  Add a minumum value to be sure controller keeps grounded

            //  Dashing is time controlled
            if (dashActive)
            {
                if (currentDashTime < maxDashTime)
                {
                    currentDashTime += Time.deltaTime;
                    moveDirection = lastMoveDirection * dashSpeed;
                }
                else
                {
                    dashActive = false;
                }
            }
            else
            {
                if (playerInput.crouch && !animating)
                {
                    if (isCrouched)
                    {
                        //  Verification to stand
                        float raycastRange = standHeight - crouchHeight + 0.1f;
                        Vector3 rayCastOrigin = new Vector3 (transform.position.x, transform.position.y + crouchHeight, transform.position.z);
                        //Debug.DrawRay(rayCastOrigin, Vector3.up * raycastRange, Color.red);
                        if (!Physics.Raycast(rayCastOrigin, Vector3.up, raycastRange))
                        {
                            isCrouched = false;
                            animating = true;
                        }
                           
                    }
                    else
                    {
                        isCrouched = true;
                        animating = true;
                    }

                }

                Crouch();

                moveDirection = new Vector3(playerInput.horizontal, 0.0f, playerInput.vertical).normalized;
                movementSpeed = GetMovementSpeed();
                lastMoveDirection = moveDirection;
                moveDirection *= movementSpeed;
            }

            //  Check if can jump
            if (CanJump())
            {
                lastJump = Time.realtimeSinceStartup;
                verticalSpeed = CalculateJumpVerticalSpeed();
                jumping = true;
            }

            isFalling = false;
        }
        else
        {
            isFalling = (verticalSpeed < 0.0f);

            //  If falling while dashing, do not fall at dash speed, instead fall at run speed
            if (dashActive)
            {
                moveDirection = lastMoveDirection * runSpeed;
            }
            dashActive = false;

            //  Check if was crouched when start falling
            if (isCrouched) { 
                if (animating)
                {
                    animating = false;
                    //  Set to the defaults
                    characterController.height = standHeight;
                    characterController.center = new Vector3(0, standHeight / 2.0f, 0);
                    cameraAnchor.localPosition = new Vector3(0, cameraAnchorDefaultY, 0);
                }
                isCrouched = false;
            }
        }


        if (onWalkableSlope && !jumping)
        {
            //  Replace gravity to force to stick to the slope
            verticalSpeed -= slopeForce * Time.deltaTime;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }
        
        //  Check if it is now falling and hit the ground to complete the jump cycle
        if (jumping && isGrounded && verticalSpeed < 0.0f)
        {
            jumping = false;
        }

        //verticalSpeed -= gravity * Time.deltaTime;
        moveDirection.y = verticalSpeed;

        //  Apply Movement
        characterController.Move(moveDirection * Time.deltaTime);

    }

    
    void Crouch ()
    {
        if (!animating) return;

        float cameraAnchortargetY;

        if (isCrouched)
        {
            characterController.height = Mathf.Lerp(characterController.height, crouchHeight, Time.deltaTime * crouchSmooth);
            characterController.center = Vector3.Lerp(characterController.center, new Vector3(0, crouchHeight / 2.0f, 0), Time.deltaTime * crouchSmooth);

            cameraAnchortargetY = crouchHeight - (standHeight - cameraAnchorDefaultY);

        }
        else
        {

            characterController.height = Mathf.Lerp(characterController.height, standHeight, Time.deltaTime * crouchSmooth);
            characterController.center = Vector3.Lerp(characterController.center, new Vector3(0, standHeight / 2.0f, 0), Time.deltaTime * crouchSmooth);

            cameraAnchortargetY = cameraAnchorDefaultY;

        }

        cameraAnchor.localPosition = Vector3.Lerp(cameraAnchor.localPosition, new Vector3(0, cameraAnchortargetY, 0), Time.deltaTime * crouchSmooth);
        if (Mathf.Abs(cameraAnchor.localPosition.y - cameraAnchortargetY) < 0.01f)
        {
            cameraAnchor.localPosition = new Vector3(0, cameraAnchortargetY, 0);
            animating = false;
        }
    }

    float GetMovementSpeed ()
    {

        if (playerInput.run)
        {

            if (dashEnabled && playerInput.dash)
            {
                currentDashTime = 0.0f;
                dashActive = true;
                return dashSpeed;
            }
            else
            {
                return runSpeed;
            }

        }
        else
        {
            //  Walking Backwards
            if (playerInput.vertical < 0)
            {
                return walkBackwardSpeed;
            }
            //  Walking Ahead or to the Sides
            else if ((playerInput.vertical > 0) || (playerInput.horizontal != 0))
            {
                return walkForwardSpeed;
            } 
            else
            {
                return 0.0f;
            }
        }
    }

    private bool CanJump ()
    {
        return (playerInput.jump) && (playerInput.vertical >= 0) && (Time.realtimeSinceStartup > (lastJump + intervalBetweenJumps)) && !dashActive;
    }
    /*
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + characterController.radius * Vector3.up, characterController.radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position - rayDistance * Vector3.up, characterController.radius);
    }*/
    /*
    public Vector3 hitNormal;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
        Debug.Log("Hit something");
    }
    */
    
    //public float hitDistance;

    

    bool IsGrounded()
    {
        onWalkableSlope = false;

        RaycastHit hitInfo;
        Vector3 rayOrigin = transform.position + new Vector3(0, characterController.radius, 0);
        Vector3 rayDirection = -Vector3.up;
        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, rayDistance))
        {
            slopeAngle = Vector3.Angle(transform.up, hitInfo.normal); // The angle of the slope is the angle between up and the normal of the slope
            if ((hitInfo.normal != Vector3.up) && (slopeAngle <= characterController.slopeLimit))
            {
                onWalkableSlope = true;
            }
        }


        return characterController.isGrounded;

        /*
        RaycastHit hitInfo;
        Vector3 rayOrigin = transform.position + new Vector3(0, characterController.radius, 0);
        Vector3 rayDirection = -Vector3.up;

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, rayDistance))
        //if (Physics.SphereCast( rayOrigin, characterController.radius, rayDirection, out hitInfo, rayDistance ))
        {
            //return true;
            slopeAngle = Vector3.Angle(transform.up, hitInfo.normal); // The angle of the slope is the angle between up and the normal of the slope

            if ((hitInfo.normal != Vector3.up) && (slopeAngle <= characterController.slopeLimit))
            {
                onWalkableSlope = true;
            }

            hitDistance = hitInfo.distance;

            //Debug.Log("Hit:" + hitInfo.transform.gameObject.name + " at:" + hitInfo.distance);
            if (hitInfo.distance <= distToGround)
            {
                //Debug.Log("Did Hit Ground");
                return true;
            }
            else
            {
                //Debug.Log("Something bellow but is far");
                return false;
            }
            
        }
        else
        {
            //Debug.Log("Did Not Hit");
            return false;

        }


        //        if (Physics.Raycast(rayOrigin, rayDirection, out hit, distToGround + 0.1f))
        if (Physics.SphereCast(
            rayOrigin,
            0.20f,
            rayDirection,
            out hitInfo,
            1.0f
        ))
        {
            Debug.Log("Hit:" + hitInfo.transform.gameObject.name);
            if (hitInfo.distance <= 0.15f)
            {
                Debug.Log("Did Hit Ground");
                return true;
            }
            else
            {
                Debug.Log("Something bellow but is far");
                return false;

            }
        }
        else
        {
            Debug.Log("Did Not Hit");
            return false;
        }
        */
    }

    private float CalculateJumpVerticalSpeed ()
    {
        return Mathf.Sqrt(2.0f * gravity * jumpHeight);
    }

    public bool IsDashActive ()
    {
        return dashActive;
    }
}
