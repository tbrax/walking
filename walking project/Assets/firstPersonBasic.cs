using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class firstPersonBasic : MonoBehaviour
{
    public float speed = 10.0f;

    public Camera m_Camera;
    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private CharacterController m_CharacterController;
    private Vector3 m_MoveDir = Vector3.zero;
    private CollisionFlags m_CollisionFlags;
    private Vector2 m_Input;
    public float gravitySpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }



    void moveDirection()
    {
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");
        m_Input = new Vector2(horizontal, vertical);

        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                           m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        //Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
        //                   m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x * speed;

        m_MoveDir.z = desiredMove.z * speed;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection();
        
        //transform.Translate(straffe, 0, translation);

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        Vector3 r2 = new Vector3(0, yaw, 0);

        Quaternion q2 = Quaternion.Euler(r2);

        transform.rotation = q2;
        if (m_CharacterController.isGrounded)
        {
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_MoveDir.y = 0f;
        }
        else
        {
            m_MoveDir += Physics.gravity * gravitySpeed * Time.fixedDeltaTime;
        }

        Quaternion finalRotation = Quaternion.Euler(new Vector3(pitch, 0, 0.0f));
        m_Camera.transform.rotation = transform.rotation * finalRotation;

        m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

    }
}


