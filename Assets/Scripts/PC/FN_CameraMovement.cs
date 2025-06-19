using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FN_CameraMovement : MonoBehaviour
{
    public float fl_mouseSensitivity = 500f;
    public bool bl_LockCursor = true;
    public Camera mainCamera;
    public Transform tr_target;

    public Vector2 pitch_MinMax = new Vector2(-40, 85);
    private float fl_yaw;
    private float fl_pitch;
    private Vector3 currentRotation;
    private Vector3 v3_rotationSmoothVelocity;
    public float fl_rotationSmoothTime = 0.12f;
    public Transform orientation = null;

    void Start()
    {

        mainCamera = Camera.main;

        if (bl_LockCursor) // lock cursor if enabled
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            transform.rotation = Quaternion.Euler(fl_pitch, fl_yaw, transform.rotation.eulerAngles.z);
        }
    }

   
    void Update()
    {
        if(mainCamera != null)
        {
            fl_yaw += Input.GetAxis("Mouse X") * fl_mouseSensitivity;
            fl_pitch -= Input.GetAxis("Mouse Y") * fl_mouseSensitivity;
            fl_pitch = Mathf.Clamp(fl_pitch, pitch_MinMax.x, pitch_MinMax.y);

            // smooth camera rotation
            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(fl_pitch, fl_yaw), ref v3_rotationSmoothVelocity, fl_rotationSmoothTime);
            transform.eulerAngles = currentRotation;

         //   mainCamera.transform.rotation = Quaternion.Euler(fl_pitch, fl_yaw,0);
               orientation.transform.rotation = Quaternion.Euler(0, fl_yaw, 0);
        }
    }
}
