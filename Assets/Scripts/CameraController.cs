using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float smoothSpeed = 5f;
    public float maxUnlockedSpeed = 10f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private bool lockedCam = true;
    private Vector3 currentVelocity;
    private Vector3 mousePos;

    private void Start()
    {
        currentVelocity = target.GetComponent<Rigidbody2D>().velocity;
        transform.position = target.transform.position;
        lockedCam = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            lockedCam = !lockedCam;
        }
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = lockedCam ? new Vector3(-0.2f * Mathf.Sign(target.transform.position.x - mousePos.x), -0.2f * Mathf.Sign(target.transform.position.y - mousePos.y), offset.z) : new Vector3(0,  0, offset.z);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, (lockedCam ? target.transform.position : mousePos) + offset, ref currentVelocity, smoothSpeed * Time.fixedDeltaTime, lockedCam ? Mathf.Infinity : maxUnlockedSpeed, Time.fixedDeltaTime);
    }
}
