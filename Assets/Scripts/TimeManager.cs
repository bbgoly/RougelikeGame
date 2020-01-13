    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class TimeManager : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private static bool isRewinding = false;
    private static bool isDone = false;

    public static Dictionary<GameObject, Stack<ObjectInfo>> rewindObjects = new Dictionary<GameObject, Stack<ObjectInfo>>();

    /*
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
        foreach(GameObject currentGameObject in rewindObjects.Keys)
        {
            ObjectInfo objectInfo = rewindObjects[currentGameObject].Count > 0 ?  rewindObjects[currentGameObject].Peek() : null;
            if (Input.GetKey(KeyCode.Return) || isRewinding)
            {
                isRewinding = true;
                Enemy enemy = currentGameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.enabled = false;
                }
                currentGameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                Time.timeScale = 3;
            }
            else if (objectInfo && (rewindObjects[currentGameObject].Count == 0 || objectInfo.objectPosition != transform.position || objectInfo.objectRotation != transform.rotation || objectInfo.localScale != transform.localScale))
            {
                rb2D = currentGameObject.GetComponent<Rigidbody2D>();
                rewindObjects[currentGameObject].Push(new ObjectInfo(currentGameObject.transform.position, currentGameObject.transform.rotation, currentGameObject.transform.localScale, rb2D.velocity, rb2D.angularVelocity));
            }
        }
    }

    void FixedUpdate()
    {
        
        if (isRewinding && isDone)
        {
            foreach (KeyValuePair<GameObject, Stack<ObjectInfo>> objectInfos in rewindObjects)
            {
                rb2D = objectInfos.Key.GetComponent<Rigidbody2D>();
                rb2D.interpolation = RigidbodyInterpolation2D.None;
                rb2D.isKinematic = false;
                objectInfos.Value.Clear();

                Enemy enemy = objectInfos.Key.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.enabled = true;
                }
            }
            isRewinding = false;
            isDone = false;
            Time.timeScale = 1;
        }
        else if (isRewinding && !isDone)
        {
            foreach (KeyValuePair<GameObject, Stack<ObjectInfo>> keyValuePair in rewindObjects)
            {
                isDone = keyValuePair.Value.Count == 0;
                if (isDone)
                {
                    continue;
                }

                objectInfo = keyValuePair.Value.Pop();
                rb2D = keyValuePair.Key.GetComponent<Rigidbody2D>();
                keyValuePair.Key.transform.position = objectInfo.objectPosition;
                keyValuePair.Key.transform.rotation = objectInfo.objectRotation;
                keyValuePair.Key.transform.localScale = objectInfo.localScale;
                rb2D.velocity = objectInfo.velocity;
                rb2D.angularVelocity = objectInfo.angularVelocity;
                rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }
    }

    public static void ChangeTimeFlow(Rigidbody2D _rb2D, float endTime, float timeStep = -0.1f)
    {
        _rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        for (float i = Time.timeScale; i < endTime; i += timeStep)
        {
            Time.timeScale = (float)System.Math.Round((decimal)i, 1);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        _rb2D.interpolation = RigidbodyInterpolation2D.None;
    }

    public static void RewindTime()
    {
        //AudioManager.PlayAudio("RewindTime", 2.4f);
        Time.timeScale = 3;
        isRewinding = true;
    }

    public static void StopRewinding()
    {
        foreach (KeyValuePair<GameObject, Stack<ObjectInfo>> objectInfos in rewindObjects)
        {
            objectInfos.Key.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
            objectInfos.Value.Clear();
        }
        isRewinding = false;
        Time.timeScale = 1;
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