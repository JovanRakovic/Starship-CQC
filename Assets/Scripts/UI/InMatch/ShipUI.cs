using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ShipUI : NetworkBehaviour
{
    [SerializeField] private Transform canvas; 
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
    [SerializeField] private Camera overlayCam;
    [SerializeField] private RawImage overlayImage;

    private int lastScreenWidth, lastScreenHeight;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            Destroy(canvas.gameObject);
            Destroy(this);
            return;
        }

        ResizeOverlayTexture();
        canvas.SetParent(null);

        manuverPointer = forwardPointer.GetChild(0).GetComponent<RectTransform>();
        manuverPointerImage = manuverPointer.GetComponent<Image>();
        throttleMat = throttleIndicator.material;
    }

    private void ResizeOverlayTexture()
    {
        if(overlayCam.targetTexture != null)
            overlayCam.targetTexture.Release();

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        
        RenderTexture t = new RenderTexture(lastScreenWidth, lastScreenHeight, 16, RenderTextureFormat.ARGB32);
        t.Create();
        overlayCam.targetTexture = t;
        overlayImage.texture = t;
    }

    void Update()
    {
        if(!IsSpawned)
            return;

        Vector3 temp = mainCamera.WorldToScreenPoint(forwardPointerTransform.position + forwardPointerTransform.forward * 5000);
        temp.z = 0;
        forwardPointer.position = temp;

        if(lastScreenHeight != Screen.height || lastScreenWidth != Screen.width)
            ResizeOverlayTexture();
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
        if(throttleMat != null)
            throttleMat.SetFloat("_cutoff", throttle*.5f);
    }
}