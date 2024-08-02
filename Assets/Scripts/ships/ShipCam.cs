using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCam : MonoBehaviour
{
    public delegate void CameraDelegate(Camera camMain, Camera camOverlay);
    public static CameraDelegate sendCams;
    private void Awake() { Ship.cameraSetup += SetupCamera; }

    private void SetupCamera(Transform parent)
    {
        transform.parent = parent;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        sendCams!.Invoke(GetComponent<Camera>(), transform.GetChild(0).GetComponent<Camera>());
    }

    private void OnDestroy() { Ship.cameraSetup -= SetupCamera; }
}
