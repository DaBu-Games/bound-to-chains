using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private Camera currentCamera;
    
    public void ChangeCamera( Camera newCamera )
    {

        if (currentCamera != null)
        {
            currentCamera.enabled = false; // Disable the current camera
        }

        currentCamera = newCamera;

        if (currentCamera != null)
        {
            currentCamera.enabled = true; // Enable the new camera
        }

    }
    
}
