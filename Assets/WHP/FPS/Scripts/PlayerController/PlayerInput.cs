using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public float horizontal;
    public float vertical;
    public bool jump;
    public bool crouch;
    public bool run;
    public bool dash;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
        jump = Input.GetKeyDown(KeyCode.Space);

        crouch = Input.GetKeyDown(KeyCode.C);
        
        run = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        dash = Input.GetKeyDown(KeyCode.F);

    }
}
