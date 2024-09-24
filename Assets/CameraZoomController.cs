using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera Cam;
    public float CamSize;
    public float zoomDelta;
    public float minZoom = 10f;
    public float maxZoom = 25f;

    void Awake()
    {
        // find camera and starting size
        Cam = Camera.main;
        CamSize = Cam.orthographicSize;
    }
    void Update()
    {
        // get the current size + the scrollwheel change
        zoomDelta = CamSize - Input.mouseScrollDelta.y;
        // constrain zoom
        CamSize = Mathf.Clamp(zoomDelta, minZoom, maxZoom);
        // apply zoom to field of view
        Cam.orthographicSize = CamSize;
    }
}
