using System.Collections;
using UnityEngine;

/// <summary>Slides a leaf left/right on its local X or Z axis.</summary>
public class SlidingGate : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The child transform that actually moves")]
    public Transform leaf;                           // <- drag GateLeaf here

    [Header("Movement")]
    [Tooltip("Distance the leaf travels when fully open (use negative if it slides left)")]
    public float openDistance = -12f;                 // metres
    public float speed = 2f;                  // metres / second

    [Header("Auto-cycling")]
    [Tooltip("Should the gate auto-open/close on a timer?")]
    public bool cycle = true;
    [Tooltip("Seconds the gate stays fully open or closed")]
    public float dwellTime = 7f;

    // ---------- runtime state ----------
    Vector3 _closedPos, _openPos;
    bool _opening;                                // current target (true = open)

    // ---------- life-cycle --------------
    void Awake()
    {
        // record reference positions while *leaf* is in CLOSED pose
        _closedPos = leaf.localPosition;
        _openPos = _closedPos + Vector3.right * openDistance;  // change to .forward if needed
    }

    void Start()
    {
        // ensure the gate starts closed in play mode
        leaf.localPosition = _closedPos;
        _opening = false;

        // launch auto-cycle if requested
        if (cycle) StartCoroutine(OpenCloseLoop());
    }

    void Update()
    {
        // move toward whichever pose we're currently targeting
        var target = _opening ? _openPos : _closedPos;

        leaf.localPosition = Vector3.MoveTowards(
            leaf.localPosition,
            target,
            speed * Time.deltaTime);
    }

    // ---------- public helpers ----------
    /// <summary>Call from other scripts to toggle immediately.</summary>
    public void Toggle() => _opening = !_opening;

    // ---------- coroutine ---------------
    IEnumerator OpenCloseLoop()
    {
        while (true)
        {
            /* 1️⃣ wait until the leaf is practically at its target
                  - we compare squared distance to avoid a sqrt()   */
            while ((leaf.localPosition -
                    (_opening ? _openPos : _closedPos)).sqrMagnitude > 1e-4f)
            {
                yield return null;          // wait one frame
            }

            /* 2️⃣ stay fully open OR fully closed for dwellTime seconds */
            yield return new WaitForSeconds(dwellTime);

            /* 3️⃣ flip the target (open ↔ closed) and loop again */
            _opening = !_opening;
        }
    }
}
