using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{

    [SerializeField]
    private float bobbingAmount = 0.1f;

    private Camera playerCamera;
    private FPSPlayerMotor playerMotor;

    private float defaultPosY;

    private float timer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        playerMotor = GetComponentInParent<FPSPlayerMotor>();

        defaultPosY = playerCamera.transform.localPosition.y;

    }

    // Update is called once per frame
    void Update()
    {
        DoHeadBob();
    }

    void DoHeadBob()
    {
        if (playerMotor.EnableUpdate && Mathf.Abs(playerMotor.movementSpeed) > 0.1f)
        {
            //Player is moving
            UpdateHeadBob();
        }
        else
        {
            //Idle Position
            ResetHeadBob();
        }
    }

    private void UpdateHeadBob()
    {
        timer += Time.deltaTime * playerMotor.movementSpeed;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, playerCamera.transform.localPosition.z);
    }

    public void ResetHeadBob()
    {
        timer = 0;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, Mathf.Lerp(playerCamera.transform.localPosition.y, defaultPosY, Time.deltaTime * playerMotor.movementSpeed), playerCamera.transform.localPosition.z);
    }


}
