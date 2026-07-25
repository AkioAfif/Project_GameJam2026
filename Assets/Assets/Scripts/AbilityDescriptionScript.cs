using UnityEngine;
 
public class AbilityDescriptionScript : MonoBehaviour
{
    private Transform _targetCamera;
 
    private void Start()
    {
      
        GameObject camObj = GameObject.Find("PlayerCam");
        if (camObj != null)
        {
            _targetCamera = camObj.transform;
        }
    }
 
    private void LateUpdate()
    {
        if (_targetCamera == null) return;

        // Cari arah dari kamera ke objek ini (agar teks tidak terbalik/mirrored)
        Vector3 directionFromCamera = transform.position - _targetCamera.position;
        
        // Kunci sumbu Y agar teks tetap berdiri tegak, tidak mendongak/menunduk
        directionFromCamera.y = 0;
 
        // Terapkan rotasi
        if (directionFromCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionFromCamera, Vector3.up);
        }
    }
}