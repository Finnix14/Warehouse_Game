using UnityEngine;

public class FN_HeadBob : MonoBehaviour
{
    [Header("References")]
    public Transform tr_groundCheck;
    public Rigidbody pc_rb;

    [Header("Bobbing Settings")]
    public float fl_bobSpeed = 14f;
    public float fl_verticalBobAmount = 0.05f;
    public float fl_horizontalSwayAmount = 0.05f;
    public float fl_movementThreshold = 0.1f;

    private float fl_defaultYPos;
    private float fl_defaultXPos;
    private float fl_timer = 0f;

    void Start()
    {
        Vector3 startPos = transform.localPosition;
        fl_defaultYPos = startPos.y;
        fl_defaultXPos = startPos.x;
    }

    void Update()
    {
        ApplyHeadBob();
    }

    void ApplyHeadBob()
    {
        Vector3 localVelocity = tr_groundCheck.InverseTransformDirection(pc_rb.velocity);
        Vector2 horizontalVelocity = new Vector2(localVelocity.x, localVelocity.z);

        if (horizontalVelocity.magnitude > fl_movementThreshold && IsGrounded())
        {
            fl_timer += Time.deltaTime * fl_bobSpeed;

            float newY = fl_defaultYPos + Mathf.Sin(fl_timer) * fl_verticalBobAmount;
            float newX = fl_defaultXPos + Mathf.Cos(fl_timer * 0.5f) * fl_horizontalSwayAmount; // half speed sway

            transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
        }
        else
        {
            // reset sway and bob
            fl_timer = 0f;

            float newY = Mathf.Lerp(transform.localPosition.y, fl_defaultYPos, Time.deltaTime * fl_bobSpeed);
            float newX = Mathf.Lerp(transform.localPosition.x, fl_defaultXPos, Time.deltaTime * fl_bobSpeed);

            transform.localPosition = new Vector3(newX, newY, transform.localPosition.z);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(tr_groundCheck.position, Vector3.down, 1.1f);
    }
}
