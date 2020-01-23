using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Public Properties
    public static Rigidbody2D targetRB2D;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 5f, maxUnlockedSpeed = 10f;
    #endregion

    #region Private Properties
    private bool lockedCam = true;
    private Vector3 currentVelocity, mousePos;
    #endregion

    #region Main code
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            lockedCam = !lockedCam;
        }
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = targetRB2D && lockedCam ? new Vector3(-0.2f * Mathf.Sign(targetRB2D.transform.position.x - mousePos.x), -0.2f * Mathf.Sign(targetRB2D.transform.position.y - mousePos.y), offset.z) : new Vector3(0,  0, offset.z);
    }

    private void FixedUpdate()
    {
        if (targetRB2D)
        {
            transform.position = Vector3.SmoothDamp(transform.position, (lockedCam ? targetRB2D.transform.position : mousePos) + offset, ref currentVelocity, smoothSpeed * Time.fixedDeltaTime, lockedCam ? Mathf.Infinity : maxUnlockedSpeed, Time.fixedDeltaTime);
        }
    }
    #endregion
}