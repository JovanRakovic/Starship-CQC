using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour
{
    private bool autopilot = true;
    private bool cameraFixed = true;
    [SerializeField] private Renderer outsideCockipGlass;
    private Rigidbody rigid;

    private Vector2 mainControls = Vector2.zero;
    private Vector2 boogalooControls = Vector2.zero;
    private Vector2 cameraDelta = Vector2.zero;

    [SerializeField] private float mainThrusterStrength = 100f;
    [SerializeField] private float boogalooThrustersStrength = 100f;
    [SerializeField] private float turnStrength = 100f;
    [SerializeField] private float rotationTrustersStrength = 100f;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        outsideCockipGlass.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    void FixedUpdate()
    {
        rigid.AddForce(transform.forward * (mainControls.y * mainThrusterStrength * rigid.mass * Time.fixedDeltaTime), ForceMode.Force);
        rigid.AddForce(transform.TransformDirection(boogalooControls) * (boogalooThrustersStrength * rigid.mass * Time.fixedDeltaTime), ForceMode.Force);
        rigid.AddTorque(transform.TransformDirection(new Vector3(0,0,-mainControls.x)) * (turnStrength * Time.fixedDeltaTime * rigid.mass), ForceMode.Force);
        if(cameraFixed)
            rigid.AddTorque(transform.TransformDirection(new Vector3(-cameraDelta.y, cameraDelta.x, 0).normalized) * (rotationTrustersStrength * rigid.mass * Time.fixedDeltaTime), ForceMode.Force);
    }

#region INPUT
    public void OnMove(InputValue value) {mainControls = value.Get<Vector2>();}
    public void OnMove2ElectricBoogaloo(InputValue value) {boogalooControls = value.Get<Vector2>();}
    public void OnDelta(InputValue value) {cameraDelta = value.Get<Vector2>();}
#endregion
}
