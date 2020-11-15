using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimbController : MonoBehaviour
{
    [Header("Ladder")]
    [SerializeField]
    private float distanceToLadder = 0.1f;
    [SerializeField]
    private LayerMask ladderLayerMask;

    [SerializeField]
    private float climbingSpeed = 2.0f;

    [SerializeField]
    private bool isClimbingLadder = false;
    [SerializeField] 
    private bool isCollidingWithLadder = false;
    


    private CharacterController characterController;
    private FPSPlayerMotor playermotor;
    private Transform ladderTransform;
    private PlayerInput playerInput;
    private MouseLook mouseLook;

    [SerializeField]
    private Vector3 ladderSlope;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playermotor = GetComponent<FPSPlayerMotor>();
        playerInput = GetComponent<PlayerInput>();
        mouseLook = GetComponent<MouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        Check();
    }

    private void Check()
    {
        if (isClimbingLadder)
        {
            if (characterController.isGrounded)
            {
                if (playerInput.vertical < 0.0f)
                {
                    Debug.Log("Leaving Ladder");
                    ToggleLadderClimbing(false);
                    return;
                }
            }

            if (!isCollidingWithLadder || playerInput.jump)
            {
                ToggleLadderClimbing(false);
                return;
            }
        }
        else if (isCollidingWithLadder)
        {

            //Debug.Log("isCollidingWithLadder. Angle: " + Vector3.Dot(-ladderTransform.forward, transform.forward));
            if (Vector3.Dot(-ladderTransform.forward, transform.forward) >= 0.9f &&
                (playerInput.vertical > .0f || !characterController.isGrounded))
            {
                Debug.Log("Climbing");
                ToggleLadderClimbing(true);
            }
        }
    }

    private void FixedUpdate()
    {
        //CheckOnLadder();

        if (isClimbingLadder)
        {
//            characterController.Move(Vector3.up * playerInput.vertical * climbingSpeed * Time.deltaTime);
            characterController.Move(ladderSlope * playerInput.vertical * climbingSpeed * Time.deltaTime);
        }
    }

    private void ToggleLadderClimbing(bool isEnabled)
    {
        isClimbingLadder = isEnabled;
        playermotor.EnableUpdate = !isEnabled;
        mouseLook.ToggleRotation(!isEnabled);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter - Other Tag:" + other.gameObject.tag);
        if (other.gameObject.tag == "Ladder")
        {
            isCollidingWithLadder = true;
            ladderTransform = other.transform;
            ladderSlope = ladderTransform.up;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit - Other Tag:" + other.gameObject.tag);
        if (other.gameObject.tag == "Ladder")
        {
            isCollidingWithLadder = false;
        }
    }

    /*
    void CheckOnLadder()
    {

        RaycastHit hit;
//        Vector3 rayOrigin = transform.position + new Vector3(0, .05f, 0);
        Vector3 rayOrigin = transform.position + transform.up * 0.05f + transform.forward * 0.5f;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rayOrigin, transform.TransformDirection(Vector3.forward), out hit, distanceToLadder, ladderLayerMask))
        {
            Debug.DrawRay(rayOrigin, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit a Ladder");
            isCollidingWithLadder = true;
            ladderTransform = hit.transform;
        }
        else
        {
            
            //  Check if in front of a ladder
            rayOrigin = transform.position + new Vector3(0.0f, characterController.radius, 0.0f) + transform.forward * (characterController.radius + 0.1f);
            Vector3 rayDirection = -Vector3.up;
            Debug.DrawRay(rayOrigin, transform.TransformDirection(rayDirection) * (characterController.radius + 0.1f), Color.green);
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, (characterController.radius + 0.1f)))
            {
                Debug.Log("Hit in front of ladder");
                onLadder = true;
            }
            

            Debug.DrawRay(rayOrigin, transform.TransformDirection(Vector3.forward) * 10, Color.red);
            //Debug.Log("Did not Hit a Ladder");
            isCollidingWithLadder = false;
        }

        rayOrigin = transform.position + transform.up * 0.2f + transform.forward * 0.5f;
        if (Physics.Raycast(rayOrigin, transform.TransformDirection(Vector3.forward), out hit, distanceToLadder, ladderLayerMask))
        {
            isLeavingLadder = false;
        }
        else
        {
            isLeavingLadder = true;
        }
    }
        */
}
