using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private ObjectInfo objectInfo;
    private bool isRewinding = false;

    public static Stack<ObjectInfo> objectsToRewind;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        objectsToRewind = new Stack<ObjectInfo>();
    }

    void Update()
    {
        isRewinding = Input.GetKey(KeyCode.Return);
        rb2D.isKinematic = isRewinding; //There's a nice bug where if player tries to move/jump while rewinding they will be able to fuck with the rewind and phase through colliders cuz rb is Kinematic
        if (Input.GetKeyDown(KeyCode.Escape))
            ChangeTimeFlow(Time.timeScale == 0 ? 1 : 0);
        else if (Input.GetKey(KeyCode.LeftShift))
            ChangeTimeFlow(0.5f);
        else if (Input.GetKey(KeyCode.Q))
            ChangeTimeFlow(2, 0.1f);
    }

    void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            objectsToRewind.Clear();
            Time.timeScale = 1;
        }
        else if (isRewinding && objectsToRewind.Count > 0)
        {
            objectInfo = objectsToRewind.Pop();
            transform.position = objectInfo.objectPosition;
            transform.rotation = objectInfo.objectRotation;
            transform.localScale = objectInfo.localScale;
            rb2D.velocity = objectInfo.velocity;
            rb2D.angularVelocity = objectInfo.angularVelocity;
            Time.timeScale = objectsToRewind.Count > 0 ? 3 : 1;
        }
        else if (objectsToRewind.Count == 0 || objectsToRewind.Peek().objectPosition != transform.position || objectsToRewind.Peek().objectRotation != transform.rotation || objectsToRewind.Peek().localScale != transform.localScale)
        {
            objectsToRewind.Push(new ObjectInfo(transform.position, transform.rotation, transform.localScale, rb2D.velocity, rb2D.angularVelocity));
        }
    }

    void ChangeTimeFlow(float endTime, float timeStep = -0.1f)
    {
        Debug.Log(Time.timeScale);
        Debug.Log(endTime);
        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        for (float i = Time.timeScale; i < endTime; i += timeStep)
        {
            Time.timeScale = (float)System.Math.Round((decimal)i, 1);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        rb2D.interpolation = RigidbodyInterpolation2D.None;
    }
}

public struct ObjectInfo
{
    public Vector3 objectPosition { get; set; }
    public Quaternion objectRotation { get; set; }
    public Vector3 localScale { get; set; }
    public Vector3 velocity { get; set; }
    public float angularVelocity { get; set; }

    public ObjectInfo(Vector3 objectPosition, Quaternion objectRotation, Vector3 localScale, Vector3 velocity, float angularVelocity)
    {
        this.objectPosition = objectPosition;
        this.objectRotation = objectRotation;
        this.localScale = localScale;
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }
}