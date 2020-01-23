using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class TimeManager : MonoBehaviour
{
    #region Public Properties
    public static Dictionary<GameObject, Stack<ObjectInfo>> rewindObjects = new Dictionary<GameObject, Stack<ObjectInfo>>();
    public static bool Rewinding = false;
    public static bool SlowMo = false;
    #endregion

    #region Private Properties
    private static bool isDone = false;
    #endregion

    #region Main code
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
        if (Rewinding && !isDone)
        {
            foreach (KeyValuePair<GameObject, Stack<ObjectInfo>> keyValuePair in rewindObjects)
            {
                isDone = keyValuePair.Value.Count == 0;
                if (!isDone)
                {
                    GameObject targetObject = keyValuePair.Key;
                    Rigidbody2D rb2D = targetObject.GetComponent<Rigidbody2D>();
                    ObjectInfo objectInfo = keyValuePair.Value.Pop();
                    targetObject.SetActive(true);

                    rb2D.isKinematic = true;
                    rb2D.velocity = objectInfo.velocity;
                    rb2D.angularVelocity = objectInfo.angularVelocity;
                    rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;

                    targetObject.transform.position = objectInfo.objectPosition;
                    targetObject.transform.rotation = objectInfo.objectRotation;
                    targetObject.transform.localScale = objectInfo.localScale;

                    Player player = targetObject.GetComponent<Player>();
                    Enemy enemy = targetObject.GetComponent<Enemy>();
                    if (player)
                    {
                        player.health = player.maxHealth;
                    }
                    else if(enemy)
                    {
                        enemy.currentHealth = enemy.maxHealth;
                    }
                }
            }
        }
        else if (Rewinding && isDone)
        {
            StopRewinding();
        }
    }

    public static void RewindTime()
    {
        Time.timeScale = 9;
        Rigidbody2D playerRB2D = FindObjectOfType<Player>().GetComponent<Rigidbody2D>();
        playerRB2D.velocity = Vector2.zero;
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
            objectInfos.Value.Clear();
        }
        Rewinding = false;
        isDone = false;
        Time.timeScale = 1;
    }
    #endregion
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
