using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public Light beam;
    public GameObject model;
    public bool startOn = true;

    void Awake()
    {
        if (beam == null) beam = GetComponentInChildren<Light>(true);
        SetState(startOn);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.fKey.wasPressedThisFrame) SetState(beam != null && !beam.enabled);
    }

void SetState(bool on)
    {
        if (beam != null) beam.enabled = on;
    }
}
