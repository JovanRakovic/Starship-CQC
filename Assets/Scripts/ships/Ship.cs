using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

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
    [SerializeField] private float throttle = 0f;
    [SerializeField] private float throttleDeadzone = 0.08f;
    [SerializeField] private float thrusterStrength = 1000f;
    [SerializeField] private float rollStrength = 1000f;
    [SerializeField] private float rotationTrustersStrength = 1000f;
    [SerializeField] private float cursorSensitivity = 1f;
    [SerializeField] private float cursorDeadzone = .1f;
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
        //A lot of things are multiplied by these two, so thought I should save a bit of computing power by storing them in a variable
        float standardMultiplication = rigid.mass * Time.fixedDeltaTime;

        float deadzonedThrottle = 0;
        if(Mathf.Abs(throttle) > throttleDeadzone) 
            deadzonedThrottle = (throttle + ((throttle > 0)? -throttleDeadzone : throttleDeadzone)) / (1f - throttleDeadzone);
        Vector3 targetDirection = transform.forward * deadzonedThrottle + transform.right * boogalooControls.x + transform.up * boogalooControls.y;
        targetDirection = autopilot? Vector3.ClampMagnitude(targetDirection * velocityLimit - rigid.velocity, 1) : targetDirection.normalized;

        Vector3 targetRoll = transform.forward * -mainControls.x;
        Vector3 forwardProjection = Vector3.Project(rigid.angularVelocity, transform.forward);
        targetRoll = autopilot? Vector3.ClampMagnitude(targetRoll * angularVelocityLimit - forwardProjection, 1) : targetRoll.normalized;

        rigid.AddForce(targetDirection * (thrusterStrength * standardMultiplication), ForceMode.Force);
        rigid.AddTorque(targetRoll * (rollStrength * standardMultiplication), ForceMode.Force);

        Vector3 tourqeDirection = Vector3.zero;
        if(cursorManuver.magnitude > cursorDeadzone)
        {
            tourqeDirection = transform.right * -cursorManuver.y + transform.up * cursorManuver.x;
            tourqeDirection = (tourqeDirection - tourqeDirection.normalized * cursorDeadzone) / (1f - cursorDeadzone);
        }

        if(autopilot)
            tourqeDirection = Vector3.ClampMagnitude(tourqeDirection * angularVelocityLimit - rigid.angularVelocity - forwardProjection, 1);
            
        rigid.AddTorque(tourqeDirection * (rotationTrustersStrength * standardMultiplication), ForceMode.Force);

        //Speed Limiting
        if(rigid.velocity.magnitude > velocityLimit)
            //I put a two in the equation as the player could "stall" the speed limit if they somehow got passed it in the first place
            rigid.AddForce(-rigid.velocity.normalized * (thrusterStrength * 2 * standardMultiplication), ForceMode.Force);
        if(rigid.angularVelocity.magnitude > angularVelocityLimit)
            rigid.AddTorque(-rigid.angularVelocity.normalized * (standardMultiplication * rollStrength), ForceMode.Force);
    }

    private void AddToThrottle(float add)
    {
        throttle += add;
        throttle = Mathf.Clamp(throttle, -1, 1);
    }

#region INPUT
    public void OnMove(InputValue value) { mainControls = value.Get<Vector2>(); }
    public void OnMove2ElectricBoogaloo(InputValue value) { boogalooControls = value.Get<Vector2>(); }
    public void OnDelta(InputValue value) 
    {
        cursorManuver += value.Get<Vector2>() * cursorSensitivity;
        if(cursorManuver.magnitude > 1)
            cursorManuver.Normalize();
        shipUI.SetManuverPointerPosition(cursorManuver);
    }
    public void OnAutopilot() { autopilot = !autopilot; }
#endregion
}