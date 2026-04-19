using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PillController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 6f;
    public float mouseSensitivity = 0.15f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundMask = ~0;
    public Transform cameraPivot;

    Rigidbody rb;
    float pitch;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
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

        Vector3 dir = (transform.right * h + transform.forward * v).normalized;
        Vector3 vel = rb.linearVelocity;
        vel.x = dir.x * moveSpeed;
        vel.z = dir.z * moveSpeed;
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
        var col = GetComponent<CapsuleCollider>();
        float halfHeight = col != null ? col.height * 0.5f * transform.lossyScale.y : 1f;
        float castDist = halfHeight - (col != null ? col.radius * transform.lossyScale.y : 0.5f) + groundCheckDistance + 0.05f;
        return Physics.Raycast(transform.position, Vector3.down, castDist, groundMask, QueryTriggerInteraction.Ignore);
    }
}
