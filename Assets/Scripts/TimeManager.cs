using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class TimeManager : MonoBehaviour
{
    public static Dictionary<GameObject, Stack<ObjectInfo>> rewindObjects = new Dictionary<GameObject, Stack<ObjectInfo>>();
    public static bool Rewinding = false;

    private static bool isDone = false;
    /*
    void Update()
    {
        Rewinding = Input.GetKey(KeyCode.Return);
        rb2D.isKinematic = Rewinding; //There's a nice bug where if player tries to move/jump while rewinding they will be able to fuck with the rewind and phase through colliders cuz rb is Kinematic
        if (Input.GetKeyDown(KeyCode.Escape))
            ChangeTimeFlow(Time.timeScale == 0 ? 1 : 0);
        else if (Input.GetKey(KeyCode.LeftShift))
            ChangeTimeFlow(0.5f);
        else if (Input.GetKey(KeyCode.Q))
            ChangeTimeFlow(2, 0.1f);
        if 
    }
    */
    private void Awake()
    {
        rewindObjects.Clear();
        foreach (GameObject gameObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (gameObject.layer == 9 || gameObject.layer == 10)
            {
                rewindObjects.Add(gameObject, new Stack<ObjectInfo>());
            }
        }
        rewindObjects.OrderBy(obj => obj.Key);
    }

    private void LateUpdate()
    {
        foreach (GameObject currentGameObject in rewindObjects.Keys)
        {
            Rigidbody2D rb2D = currentGameObject.GetComponent<Rigidbody2D>();
            ObjectInfo objectInfo = rewindObjects[currentGameObject].Count > 0 ? rewindObjects[currentGameObject].Peek() : new ObjectInfo(transform.position, transform.rotation, transform.localScale, rb2D.velocity, rb2D.angularVelocity);
            if (!Rewinding && (rewindObjects[currentGameObject].Count == 0 || objectInfo.objectPosition != transform.position || objectInfo.objectRotation != transform.rotation || objectInfo.localScale != transform.localScale))
            {
                rewindObjects[currentGameObject].Push(new ObjectInfo(currentGameObject.transform.position, currentGameObject.transform.rotation, currentGameObject.transform.localScale, rb2D.velocity, rb2D.angularVelocity));
            }
        }
    }

    void FixedUpdate()
    {
        if (Rewinding && isDone)
        {
            StopRewinding();
        }
        else if (Rewinding && !isDone)
        {
            foreach (KeyValuePair<GameObject, Stack<ObjectInfo>> keyValuePair in rewindObjects)
            {
                isDone = keyValuePair.Value.Count == 0;
                if (isDone)
                {
                    continue;
                }

                ObjectInfo objectInfo = keyValuePair.Value.Pop();
                Rigidbody2D rb2D = keyValuePair.Key.GetComponent<Rigidbody2D>();
                rb2D.isKinematic = true;

                keyValuePair.Key.transform.position = objectInfo.objectPosition;
                keyValuePair.Key.transform.rotation = objectInfo.objectRotation;
                keyValuePair.Key.transform.localScale = objectInfo.localScale;
                rb2D.velocity = objectInfo.velocity;
                rb2D.angularVelocity = objectInfo.angularVelocity;
                rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }
    }

    public static void ChangeTimeFlow(Rigidbody2D rb2D, float endTime, float timeStep = -0.1f)
    {
        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        for (float i = Time.timeScale; i < endTime; i += timeStep)
        {
            Time.timeScale = (float)System.Math.Round((decimal)i, 1);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        rb2D.interpolation = RigidbodyInterpolation2D.None;
    }

    public static void RewindTime() //Probably rewind the time GUI as well?
    {
        //AudioManager.PlayAudio("RewindTime", 2.4f);
        Time.timeScale = 3;
        Rewinding = true;
        isDone = false;
    }

    public static void StopRewinding()
    {
        foreach (KeyValuePair<GameObject, Stack<ObjectInfo>> objectInfos in rewindObjects)
        {
            Rigidbody2D rb2D = objectInfos.Key.GetComponent<Rigidbody2D>();
            rb2D.interpolation = RigidbodyInterpolation2D.None;
            rb2D.isKinematic = false;
            //objectInfos.Value.Clear();
        }
        Rewinding = false;
        isDone = false;
        Time.timeScale = 1;
    }
}

public struct ObjectInfo
{
    public Vector3 objectPosition { get; set; }
    public Quaternion objectRotation { get; set; }
    public Vector3 localScale { get; set; }
    public Vector2 velocity { get; set; }
    public float angularVelocity { get; set; }

    public ObjectInfo(Vector3 objectPosition, Quaternion objectRotation, Vector3 localScale, Vector2 velocity, float angularVelocity)
    {
        this.objectPosition = objectPosition;
        this.objectRotation = objectRotation;
        this.localScale = localScale;
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }
}