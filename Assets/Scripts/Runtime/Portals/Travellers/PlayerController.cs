using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可传送、基于刚体的第一人称控制器
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : PortalTraveller
{
    public float walkSpeed = 4;
    public float jumpImpulse = 10;
    public float jumpCD = 0.15f;

    public bool lockCursor = true;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2(-60, 85);
    public float rotationSmoothTime = 0.1f;
    public float rayLength = 0.2f;

    public float yaw;
    public float pitch;

    private Rigidbody playerRigidbody;
    private Camera cam;

    private Vector3 inputDir;
    private float smoothYaw;
    private float smoothPitch;
    private float yawSmoothV;
    private float pitchSmoothV;

    private bool jumping;
    private bool isGrounded;
    private float lastGroundedTime;
    private bool disabled = false;

    private void Start()
    {
        cam = Camera.main;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        playerRigidbody = GetComponent<Rigidbody>();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
    }

    private void Update()
    {
        if (ESCMenu.Instance != null)
        {
            disabled = ESCMenu.Instance.escMenu.activeSelf || ESCMenu.Instance.winMenu.activeSelf;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if(!disabled)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            disabled = !disabled;
        }

        if (disabled)
        {
            return;
        }

        if (transform.position.y < -100)
        {
            OnDeath();
        }

        #region 移动、跳跃、转向
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = new Vector3(input.x, 0, input.y).normalized;

        if (isGrounded)
        {
            lastGroundedTime = Time.time;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
                if (timeSinceLastTouchedGround < jumpCD)
                {
                    jumping = true;
                }
            }
        }

        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        yaw += mX * mouseSensitivity;
        pitch -= mY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        //不限制smoothPitch可能会报以下错误
        //transform.localRotation assign attempt for 'Main Camera' is not valid. Input rotation is { NaN, NaN, NaN, NaN }.
        smoothPitch = Mathf.Clamp(smoothPitch, pitchMinMax.x, pitchMinMax.y);

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;

        //Quaternion rotationYaw = Quaternion.AngleAxis(smoothYaw, Vector3.up);
        //Quaternion rotationPitch = Quaternion.AngleAxis(smoothPitch, Vector3.right);

        //transform.rotation = rotationYaw;
        //cam.transform.localRotation = rotationPitch;

        //不平滑
        //transform.eulerAngles = Vector3.up * yaw;
        //cam.transform.localEulerAngles = Vector3.right * pitch;
        #endregion

    }

    private void FixedUpdate()
    {
        if (disabled)
        {
            return;
        }

        CheckGrounded();

        Vector3 oldVelocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
        Vector3 velocity = transform.TransformDirection(inputDir) * walkSpeed - oldVelocity;

        playerRigidbody.AddForce(velocity, ForceMode.VelocityChange);

        if(jumping)
        {
            jumping = false;
            playerRigidbody.AddForce(new Vector3(0f, jumpImpulse, 0f), ForceMode.Impulse);
        }
    }

    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        isGrounded = Physics.Raycast(ray, out hit, rayLength);
    }

    public void OnDeath()
    {
        if(ESCMenu.Instance != null)
        {
            ESCMenu.Instance.OpenESCMenu();
            ESCMenu.Instance.resumeButton.gameObject.SetActive(false);
        }
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        var eulerRot = rot.eulerAngles;
        var deltaYaw = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
        yaw += deltaYaw;
        smoothYaw += deltaYaw;
        transform.eulerAngles = Vector3.up * smoothYaw;
        //var m = toPortal.localToWorldMatrix * fromPortal.worldToLocalMatrix * cam.transform.localToWorldMatrix;
        //var camEulerRot = m.rotation.eulerAngles;
        //var deltaPitch = Mathf.DeltaAngle(smoothPitch, camEulerRot.x);
        //pitch += deltaPitch;
        //smoothPitch += deltaPitch;
        //cam.transform.localEulerAngles = Vector3.right * smoothPitch;
        playerRigidbody.velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(playerRigidbody.velocity));
        playerRigidbody.angularVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(playerRigidbody.angularVelocity));
    }
}
