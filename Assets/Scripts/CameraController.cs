using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float smoothSpeed = 5f;
    public float maxUnlockedSpeed = 10f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private bool lockedCam = true;
    private Vector3 currentVelocity;

    private void Start()
    {
        lockedCam = true;
        currentVelocity = target.GetComponent<Rigidbody2D>().velocity;
        transform.position = target.transform.position;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            lockedCam = !lockedCam;
        }
        transform.position = Vector3.SmoothDamp(transform.position, (lockedCam ? target.transform.position : Camera.main.ScreenToWorldPoint(Input.mousePosition)) + offset, ref currentVelocity, smoothSpeed * Time.fixedDeltaTime, lockedCam ? Mathf.Infinity : maxUnlockedSpeed, Time.fixedDeltaTime);
    }
}
