using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System;
using UnityEngine.UI;

public class gScript : MonoBehaviour
{


    public float speed = 15.0f;
    public float jumpSpeed = 80.0f;

    public float speedH = 40.0f;
    public float speedV = 40.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Vector3 moveDirection = Vector3.zero;

    public Transform cam;

    public Vector3 gravityDirection = new Vector3(0, 0, 1);
    public float gravity = 9.8f;
    public bool swap = false;
    public bool grounded = false;

    private float timeSinceChangeGravity = 1.0f;

    private Quaternion wantedRotation;
    private float camRotationSpeed = 100f;

    public bool isTouch = true;

    private int collisionCount = 0;

    private Vector3 lastPos;

    private int hitWall = 0;

    private float fallV = 0.0f;

    //public mainGameScript mainS;

    public string lookGui = "";

    public Text lookItem;

    public Canvas lookCanvas;

    public Transform mySpawn;

    public bool IsNotColliding
    {
        get { return collisionCount == 0; }
    }

    public float timeSinceGrounded = 0;

    void respawn()
    {
        transform.position = mySpawn.position;
        gravityDirection = mySpawn.up;
    }

    void spawnCheck()
    {


        timeSinceGrounded += Time.deltaTime;

        if (grounded)
        {
            timeSinceGrounded = 0;
        }


        if (timeSinceGrounded > 7)
        {
            respawn();
        }

    }


    // Use this for initialization
    void Start()
    {


        lastPos = transform.position;
        wantedRotation = transform.rotation;
        respawn();
    }

    void keys()
    {

        if (Input.GetKey("space"))
        {
            if (grounded)
            {
                jump();
                grounded = false;
            }

        }

    }



    void checkCollideDo(ContactPoint contact)
    {

    }

    void checkTriggerDo(Collider collision)
    {
        //if (collision.transform.GetComponent<gravityTurn>() != null)
        //{
        //    //Vector3 targetUp = contact.otherCollider.transform.up;

        //    if (timeSinceChangeGravity > 0.5f)
        //    {
        //        Vector3 targetUp = collision.transform.GetComponent<gravityTurn>().getMe(gravityDirection);
        //        swapGrav(targetUp);


        //    }

        //}
    }


    void swapGrav(Vector3 targetUp)
    {
        if (Vector3.Distance(targetUp, gravityDirection) > 0.01)
        {



            gravityDirection = targetUp;
            timeSinceChangeGravity = 0.0f;
            camRotationSpeed = 1f;
        }
    }


    void OnTriggerEnter(Collider collision)
    {

        checkTriggerDo(collision);
    }

    void OnCollisionEnter(Collision collision)
    {
        collisionCount++;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.contacts.Length > 0)
            {
                isTouch = true;
                if (Vector3.Dot(contact.normal, transform.up) > 0.5)
                {
                    grounded = true;
                    checkCollideDo(contact);
                }
            }
        }

    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.contacts.Length > 0)
            {
                if (Vector3.Dot(contact.normal, transform.up) > 0.5)
                {
                    grounded = true;
                }
            }
        }

    }



    void OnCollisionExit(Collision other)
    {
        collisionCount--;
        grounded = false;

    }

    private void jump()
    {

        fallV = -jumpSpeed;

        //Vector3 counterGravity;
        //counterGravity = 1 * transform.up * gravity * jumpSpeed;
        //transform.Translate(counterGravity * Time.deltaTime);
        //GetComponent<Rigidbody>().AddForce(counterGravity, ForceMode.Acceleration);
    }

    private void addFall()
    {
        fallV += gravity * Time.deltaTime;

        if (!grounded)
        {
            Vector3 counterGravity;
            counterGravity = -1 * gravityDirection * fallV;

            transform.Translate(counterGravity * Time.deltaTime, Space.World);
            //GetComponent<Rigidbody>().AddForce(counterGravity, ForceMode.Acceleration);
            //GetComponent<PlatformEffector2D>().AddForce(counterGravity, ForceMode.Acceleration);

        }
        else
        {
            fallV = 0;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }


    /*
        static MethodInfo _clearConsoleMethod;
        static MethodInfo clearConsoleMethod
        {
            get
            {
                if (_clearConsoleMethod == null)
                {
                    Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
                    Type logEntries = assembly.GetType("UnityEditor.LogEntries");
                    _clearConsoleMethod = logEntries.GetMethod("Clear");
                }
                return _clearConsoleMethod;
            }
        }

        public static void clearLog()
        {
            clearConsoleMethod.Invoke(new object(), null);
        }
    */

    private void moveC()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");


        //moveDirection = transform.TransformDirection(moveDirection);

        Vector3 rotation = new Vector3(90, 0, 0);

        Quaternion r = Quaternion.Euler(rotation);
        //Quaternion q = Quaternion.LookRotation(gravityDirection,Vector3.up);

        Quaternion q = Quaternion.LookRotation(gravityDirection, Vector3.up) * r;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));



        moveDirection = moveDirection * speed * Time.deltaTime;



        Vector3 r2 = new Vector3(0, yaw, 0);

        Quaternion q2 = Quaternion.Euler(r2);


        //transform.eulerAngles = rotation;
        transform.rotation = q * q2;

        //print(transform.rotation);




        //then rotate

        //cam.rotation = transform.rotation * Quaternion.Euler(new Vector3(pitch, 0, 0.0f));

        //Quaternion wantedRotation = transform.rotation * Quaternion.Euler(new Vector3(pitch, 0, 0.0f));

        float maxCamRot = 100f;
        if (camRotationSpeed < maxCamRot)
        {
            if (timeSinceChangeGravity < 0.9f)
            {
                camRotationSpeed += 7f * Time.deltaTime;
            }
            else
            {
                camRotationSpeed += 50f * Time.deltaTime;

            }

            if (camRotationSpeed > maxCamRot)
            {
                camRotationSpeed = maxCamRot;
            }
        }

        //print(timeSinceChangeGravity);
        wantedRotation = Quaternion.Lerp(wantedRotation, transform.rotation, Time.deltaTime * camRotationSpeed);
        timeSinceChangeGravity += Time.deltaTime;



        //cam.rotation = Quaternion.Lerp(cam.rotation, finalRotation, Time.time * camSpeed);
        float radC = transform.localScale.x;
        float radA = 2.2f * radC;

        if (grounded)
        {
            RaycastHit hit;
            Vector3 d = moveDirection;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(d), out hit, moveDirection.magnitude * radA))
            {
                //GetComponent<Rigidbody>().velocity = Vector3.zero;
                hitWall = 1;

                if (collisionCount < 3)
                {
                    //print((moveDirection.magnitude / hit.distance));
                    //transform.Translate(moveDirection * (moveDirection.magnitude/hit.distance));
                    //Vector3 forceCross = Vector3.Cross(moveDirection, hit.normal);
                    //forceCross = Vector3.Cross(hit.normal, forceCross);
                    transform.Translate(moveDirection);
                    //transform.Translate(forceCross);
                }
            }
            else
            {
                transform.Translate(moveDirection);
            }
        }
        else
        {
            RaycastHit hit;
            Vector3 d = moveDirection;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(d), out hit, moveDirection.magnitude * radA))
            {
                //GetComponent<Rigidbody>().velocity = Vector3.zero;
                hitWall = 1;
                //if (collisionCount == 1)
                //{
                //print((moveDirection.magnitude / hit.distance));
                //transform.Translate(moveDirection * (moveDirection.magnitude/hit.distance));
                Vector3 forceCross = Vector3.Cross(moveDirection, hit.normal);
                forceCross = Vector3.Cross(hit.normal, forceCross);

                transform.Translate(forceCross);
                //}
            }
            else
            {
                transform.Translate(moveDirection);
            }

        }

        /*transform.Translate(moveDirection);

        if (!grounded && !IsNotColliding)
        {

        }*/

        lastPos = transform.position;


        Quaternion finalRotation = wantedRotation * Quaternion.Euler(new Vector3(pitch, 0, 0.0f));
        cam.rotation = finalRotation;
        cam.position = transform.position + transform.up * 1;
        addFall();
    }



    void OnGUI()
    {
        if (lookGui != "")
        {
            lookItem.text = lookGui;
            /*//Text m_Text.text;
            var w = 250;
            var h = 300;
            var rect = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);
            GUI.Label(rect, lookGui);*/
        }
    }

    void lookRay()
    {
        lookGui = "";
        RaycastHit hit;

        Vector3 fwd = cam.forward;

        //Debug.DrawRay(cam.position, fwd*10, Color.green,1);
        //lookCanvas.enabled = false;

        if (Physics.Raycast(cam.position, fwd, out hit, 10))
        {
            //clearLog();//
            //print("Found an object - distance: " + hit.distance);
            //if (hit.transform.GetComponent<npcScript>() != null)
            //{
            //    lookCanvas.enabled = true;

            //    var s = hit.transform.GetComponent<npcScript>();
            //    lookGui = "Talk";
            //    lookItem.text = lookGui;
            //}

        }

        //if (Physics.Raycast(transform.position, transform.TransformDirection(d), out hit, moveDirection.magnitude * radA))
        //{
        //}
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        spawnCheck();
        lookRay();
        keys();
        moveC();



    }
}
