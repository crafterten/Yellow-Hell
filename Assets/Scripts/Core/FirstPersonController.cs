using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7.5f;
    public float jumpForce = 6f;
    public float mouseSensitivity = 0.15f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundMask = ~0;
    public Transform cameraPivot;

    public float maxStamina = 100f;
    public float staminaDrain = 25f;
    public float staminaRegen = 15f;
    public float staminaRegenDelay = 1.2f;

    Rigidbody rb;
    CapsuleCollider col;
    float pitch;
    float stamina;
    float lastSprintTime;
    bool sprintLocked;

    public float Stamina => stamina;
    public bool IsSprinting { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        stamina = maxStamina;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        if (mouse != null && cameraPivot != null)
        {
            Vector2 d = mouse.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(0f, d.x, 0f, Space.World);
            pitch = Mathf.Clamp(pitch - d.y, -89f, 89f);
            cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        if (kb == null) return;

        float h = 0f, v = 0f;
        if (kb.aKey.isPressed) h -= 1f;
        if (kb.dKey.isPressed) h += 1f;
        if (kb.wKey.isPressed) v += 1f;
        if (kb.sKey.isPressed) v -= 1f;

        bool wantSprint = kb.leftShiftKey.isPressed && v > 0f && !sprintLocked;
        IsSprinting = wantSprint && stamina > 0f;
        float speed = IsSprinting ? sprintSpeed : walkSpeed;

        if (IsSprinting)
        {
            stamina -= staminaDrain * Time.deltaTime;
            lastSprintTime = Time.time;
            if (stamina <= 0f) { stamina = 0f; sprintLocked = true; }
        }
        else if (Time.time - lastSprintTime > staminaRegenDelay)
        {
            stamina = Mathf.Min(maxStamina, stamina + staminaRegen * Time.deltaTime);
            if (sprintLocked && stamina > maxStamina * 0.3f) sprintLocked = false;
        }

        Vector3 dir = (transform.right * h + transform.forward * v).normalized;
        Vector3 vel = rb.linearVelocity;
        vel.x = dir.x * speed;
        vel.z = dir.z * speed;
        rb.linearVelocity = vel;

        if (kb.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rb.linearVelocity = new Vector3(vel.x, jumpForce, vel.z);
        }

        if (kb.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    bool IsGrounded()
    {
        if (col == null) return true;
        float halfHeight = col.height * 0.5f * transform.lossyScale.y;
        float castDist = halfHeight - col.radius * transform.lossyScale.y + groundCheckDistance + 0.05f;
        return Physics.Raycast(transform.position, Vector3.down, castDist, groundMask, QueryTriggerInteraction.Ignore);
    }
}
