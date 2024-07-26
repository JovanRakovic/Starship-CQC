using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour
{
    [SerializeField] private RectTransform forwardPointer;
    private Transform forwardPointerTransform;
    private RectTransform manuverPointer;
    private Image manuverPointerImage;
    //For some reason I have to manually assign the camera instead of being able to assign it with Camera.main
    //I suspect that might be due to there being two cameras in the scene
    [SerializeField] Camera mainCamera;
    [SerializeField] private float manuverPointerExtent = 5;
    [SerializeField] private Renderer throttleIndicator;
    private Material throttleMat;

    void Start()
    {
        manuverPointer = forwardPointer.GetChild(0).GetComponent<RectTransform>();
        manuverPointerImage = manuverPointer.GetComponent<Image>();
        throttleMat = throttleIndicator.material;
    }

    void Update()
    {
        Vector3 temp = mainCamera.WorldToScreenPoint(forwardPointerTransform.position + forwardPointerTransform.forward * 5000);
        temp.z = 0;
        forwardPointer.position = temp;
    }

    public void SetForwardPointerReference(Transform reference) { forwardPointerTransform = reference; }
    public void SetManuverPointerPosition(Vector2 position)
    {
        manuverPointer.localPosition = manuverPointerExtent * position;
        manuverPointer.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg);
        Color col = manuverPointerImage.color;
        col.a = Mathf.Clamp(Mathf.Sqrt((position.magnitude - .05f) * 1.06f), 0, 1);
        manuverPointerImage.color = col;
    }

    public void UpdateThrottle(float throttle)
    {
        throttleMat.SetFloat("_cutoff", throttle*.5f);
    }
}