using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Transform orientation;

    float xRotation;
    float yRotation;

    // Jika true, kamera tidak akan merespons input mouse
    private bool isLocked = false;

    private void Start()
    {
        Vector3 rot = transform.eulerAngles;
        xRotation = rot.x;
        yRotation = rot.y;
    }

    /// <summary>
    /// Dipanggil oleh MainMenuScript untuk memblokir atau membebaskan kamera.
    /// </summary>
    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }

    private void Update()
    {
        // Jika kamera dikunci, tidak ada input mouse yang diproses
        if (isLocked) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
