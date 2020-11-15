using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField]
    private float clampValue = 60.0f;
    [SerializeField]
    private float mouseSensitivity = 150.0f;
    [SerializeField]
    private float lookSpeed = 3;

    [SerializeField]
    private bool lockCursor = true;

    private Transform cameraTransform;
    private Vector2 rotation = Vector2.zero;

    [SerializeField]
    private bool enableRotation;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        if (lockCursor)
        {
            LockCursor();
        }
        enableRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableRotation) {
            rotation.y += Input.GetAxis("Mouse X");
        }
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -clampValue / lookSpeed, clampValue / lookSpeed);

        transform.eulerAngles = new Vector2(0,rotation.y) * lookSpeed;
        cameraTransform.localRotation = Quaternion.Euler(rotation.x * lookSpeed, 0, 0);
    }

    private void LockCursor()
    {
        //		Cursor.lockState = CursorLockMode.Locked;

        //  Reset and hide cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ToggleRotation(bool enableRotation)
    {
        this.enableRotation = enableRotation;
    }
}