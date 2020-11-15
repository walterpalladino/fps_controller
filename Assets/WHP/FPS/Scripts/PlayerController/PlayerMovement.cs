using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController cc;


    [Tooltip("How fast the player falls when not standing on anything.")]
    [SerializeField]
    private float gravity = 20.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;


    public float movementSpeed;
    public float rotationSpeed;


    private Vector3 moveDirection = Vector3.zero;
    private float velocityY = 0.0f;

    [SerializeField]
    private float distanceToLadder = 0.1f;
    [SerializeField]
    private LayerMask ladderLayerMask;
    [SerializeField]
    private bool onLadder;
    [SerializeField]
    private float onLadderSpeed = 1f;


    [SerializeField]
    private float mouseSensitivity;
    [SerializeField]
    private float xAxisClamp = 85.0f;
    private float mouseY = 0.0f;

    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        {
            Cursor.lockState = CursorLockMode.Locked;
            camera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        {
            CheckOnLadder();
            Move();
            Rotate();
        }
    }

    void Move()
    {
        moveDirection = Vector3.zero;

        // Apply gravity
        if (!onLadder)
        {
            if (cc.isGrounded)
            {
                velocityY = 0.0f;
                if (Input.GetButton("Jump"))
                {
                    velocityY = Mathf.Sqrt(2.0f * gravity * jumpHeight);
                }
            }
            else
            {
                velocityY -= gravity * Time.deltaTime;
            }

        } else
        {
            velocityY = 0.0f;
        }


        float speed = movementSpeed;

        if (Input.GetKey(KeyCode.W))
        {
            if (onLadder)
            {
                velocityY = onLadderSpeed;
            } else
            {
                moveDirection += transform.forward;
            }
        } else
        if (Input.GetKey(KeyCode.S))
        {
            if (onLadder && !cc.isGrounded)
            {
                velocityY = -onLadderSpeed;
            } 
            else
            {
                moveDirection -= transform.forward;
            }
                
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right ;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right ;
        }

        moveDirection = moveDirection.normalized;

        cc.Move((moveDirection * speed + new Vector3(0, velocityY, 0)) * Time.deltaTime);
    }

    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0.0f, mouseX, 0.0f));

        mouseY += Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -xAxisClamp, xAxisClamp);

        camera.transform.localEulerAngles = new Vector3(-mouseY, 0, 0);
    }

    void CheckOnLadder ()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + new Vector3(0, .3f, 0);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rayOrigin, transform.TransformDirection(Vector3.forward), out hit, distanceToLadder, ladderLayerMask))
        {
            Debug.DrawRay(rayOrigin, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            onLadder = true;
        }
        else
        {
            Debug.DrawRay(rayOrigin, transform.TransformDirection(Vector3.forward) * 10, Color.red);
            Debug.Log("Did not Hit");
            onLadder = false;
        }
    }

    
}
