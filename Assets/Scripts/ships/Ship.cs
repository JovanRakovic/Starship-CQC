using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour
{
    [SerializeField] private bool autopilot = true;
    private bool cameraFixed = true;
    [SerializeField] private Renderer outsideCockipGlass;
    private Rigidbody rigid;
    private ShipUI shipUI;

    //Main controls the forward/backward axis and the rotation around the same axis
    private Vector2 mainControls = Vector2.zero;
    //Boogaloo controls the secound set of axis (left/right, up/down)
    private Vector2 boogalooControls = Vector2.zero;
    private Vector2 cursorManuver = Vector2.zero;
    private float throttle = 0f;

    [SerializeField] private float mainThrusterStrength = 100f;
    [SerializeField] private float boogalooThrustersStrength = 100f;
    [SerializeField] private float turnStrength = 100f;
    [SerializeField] private float rotationTrustersStrength = 100f;
    [SerializeField] private float cursorSensitivity = 1f;
    [SerializeField] private float cursorDeadzone = .05f;
    [SerializeField] private float velocityLimit = 10f;
    [SerializeField] private float angularVelocityLimit = 10f;

    void Start()
    {
        shipUI = GetComponent<ShipUI>();
        shipUI.SetForwardPointerReference(transform);
        rigid = GetComponent<Rigidbody>();
        outsideCockipGlass.enabled = false;
    }

    private void Update()
    {
        if(autopilot)
            AddToThrottle(mainControls.y * Time.deltaTime);
        else
            throttle = mainControls.y;
        shipUI.UpdateThrottle(throttle);
    }

    void FixedUpdate()
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        rigid.AddForce(transform.forward * (throttle * mainThrusterStrength * rigid.mass * Time.fixedDeltaTime), ForceMode.Force);
        rigid.AddForce(transform.TransformDirection(boogalooControls) * (boogalooThrustersStrength * rigid.mass * Time.fixedDeltaTime), ForceMode.Force);
        rigid.AddTorque(transform.TransformDirection(new Vector3(0,0,-mainControls.x)) * (turnStrength * Time.fixedDeltaTime * rigid.mass), ForceMode.Force);
        if(cursorManuver.magnitude > cursorDeadzone && cameraFixed)
            rigid.AddTorque(transform.TransformDirection(new Vector3(-cursorManuver.y, cursorManuver.x, 0) * (rotationTrustersStrength * rigid.mass * Time.fixedDeltaTime)), ForceMode.Force);
        
        //Speed Limiting
        if(rigid.velocity.magnitude > velocityLimit)
            rigid.AddForce(-rigid.velocity.normalized * (mainThrusterStrength * rigid.mass * Time.fixedDeltaTime), ForceMode.Force);
        if(rigid.angularVelocity.magnitude > angularVelocityLimit)
            rigid.AddTorque(-rigid.angularVelocity.normalized * (Time.fixedDeltaTime * turnStrength * Mathf.Rad2Deg * 100), ForceMode.Force);
    }

    private void AddToThrottle(float add)
    {
        throttle += add;
        throttle = Mathf.Clamp(throttle, -1, 1);
    }

#region INPUT
    public void OnMove(InputValue value) {mainControls = value.Get<Vector2>();}
    public void OnMove2ElectricBoogaloo(InputValue value) {boogalooControls = value.Get<Vector2>();}
    public void OnDelta(InputValue value) 
    {
        cursorManuver += value.Get<Vector2>() * cursorSensitivity;
        if(cursorManuver.magnitude > 1)
            cursorManuver.Normalize();
        shipUI.SetManuverPointerPosition(cursorManuver);
    }
#endregion
}