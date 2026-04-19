using UnityEngine;

public class FlashlightBeamAim : MonoBehaviour
{
    public Transform aimTarget;
    public float followSpeed = 8f;

    void LateUpdate()
    {
        if (aimTarget == null) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            aimTarget.rotation,
            1f - Mathf.Exp(-followSpeed * Time.deltaTime)
        );
    }
}
