using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Ship : NetworkBehaviour
{
    public delegate void CameraSetup(Transform camParent);
    public static CameraSetup cameraSetup;

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
    [SerializeField] private float forwardThrusterStrength = 1000f;
    [SerializeField] private float boogalooThrusterStrength = 1000f;
    [SerializeField] private float rollStrength = 1000f;
    [SerializeField] private float cursorTrustersStrength = 1000f;
    [SerializeField] private float cursorSensitivity = 1f;
    [SerializeField] private float cursorDeadzone = .1f;
    [SerializeField] private float velocityLimit = 10f;
    [SerializeField] private float rollVelocityLimit = 2f;
    [SerializeField] private float pitchYawVelocityLimit = 1.5f;
    [SerializeField] private Transform camParent;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            Destroy(GetComponent<PlayerInput>());
            outsideCockipGlass.enabled = true;
            return;
        }

        cameraSetup?.Invoke(camParent);

        shipUI = GetComponent<ShipUI>();
        shipUI.SetForwardPointerTarget(transform);
        rigid = GetComponent<Rigidbody>();
        outsideCockipGlass.enabled = false;
    }

    private void Update()
    {
        if(!IsSpawned || !IsOwner)
            return;

        if(autopilot)
            AddToThrottle(mainControls.y * Time.deltaTime);
        else
            throttle = mainControls.y;
        shipUI.UpdateThrottle(throttle);
    }

    void FixedUpdate()
    {
        if(!IsSpawned || !IsOwner)
            return;
        
        HandleMovement();
    }

    private void HandleMovement()
    {
        //A lot of things are multiplied by these two, so thought I should save a bit of computing power by storing them in a variable
        float standardMultiplication = rigid.mass * Time.fixedDeltaTime;

        float deadzonedThrottle = 0;
        if(Mathf.Abs(throttle) > throttleDeadzone) 
            deadzonedThrottle = (throttle + ((throttle > 0)? -throttleDeadzone : throttleDeadzone)) / (1f - throttleDeadzone);
        Vector3 forwardThrust = transform.forward * deadzonedThrottle + (transform.right * boogalooControls.x + transform.up * boogalooControls.y) * (boogalooThrusterStrength / forwardThrusterStrength);
        forwardThrust = autopilot? Vector3.ClampMagnitude(forwardThrust * velocityLimit - rigid.velocity, 1) : forwardThrust.normalized;

        Vector3 targetRoll = transform.forward * -mainControls.x;
        Vector3 forwardProjection = Vector3.Project(rigid.angularVelocity, transform.forward);

        targetRoll = autopilot? Vector3.ClampMagnitude(targetRoll * rollVelocityLimit - forwardProjection, 1) : targetRoll;

        Vector3 targetPitchYaw = Vector3.zero;
        if(cursorManuver.magnitude > cursorDeadzone)
        {
            targetPitchYaw = transform.right * -cursorManuver.y + transform.up * cursorManuver.x;
            targetPitchYaw = (targetPitchYaw - targetPitchYaw.normalized * cursorDeadzone) / (1f - cursorDeadzone);
        }

        if(autopilot)
            targetPitchYaw = Vector3.ClampMagnitude(targetPitchYaw * pitchYawVelocityLimit - rigid.angularVelocity + forwardProjection, 1);
            
        //Applying the forces
        targetPitchYaw *= cursorTrustersStrength / rollStrength;
        rigid.AddForce(forwardThrust * (forwardThrusterStrength * standardMultiplication), ForceMode.Force);
        rigid.AddTorque((targetRoll + targetPitchYaw) * (rollStrength * standardMultiplication), ForceMode.Force);

        //Speed Limiting
        Vector3 angularVelocityForwardProjection = Vector3.Project(rigid.angularVelocity, transform.forward);
        Vector3 angularVelocityWithoutRoll = rigid.angularVelocity -  angularVelocityForwardProjection;
        //I put a two in the equation as the player could "stall" the speed limit if they somehow got passed it in the first place
        standardMultiplication *= 2;
        if(rigid.velocity.magnitude > velocityLimit)
            rigid.AddForce(-rigid.velocity.normalized * (forwardThrusterStrength * standardMultiplication), ForceMode.Force);
        if(angularVelocityForwardProjection.magnitude > rollVelocityLimit)
            rigid.AddTorque(-angularVelocityForwardProjection.normalized * (standardMultiplication * rollStrength), ForceMode.Force);
        if(angularVelocityWithoutRoll.magnitude > pitchYawVelocityLimit)
            rigid.AddTorque(-angularVelocityWithoutRoll.normalized * (standardMultiplication * cursorTrustersStrength), ForceMode.Force);
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