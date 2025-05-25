using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ConveyorBeltAnimator : MonoBehaviour
{
    [Header("Scroll settings")]
    [Tooltip("Units-per-second that the texture moves.")]
    [SerializeField] private float conveyorSpeed = 0.3f;

    [Tooltip("Scroll axis in UV space (0,1) = up, (1,0) = right, etc.).")]
    [SerializeField] private Vector2 scrollDirection = new Vector2(0f, 1f);

    /* ---------------- Internals ---------------- */
    private MeshRenderer rend;
    private Material beltMaterial;   // Instanced copy so each belt can have its own speed
    private Vector2 uvOffset;        // Running UV offset

    private void Awake()
    {
        enabled = false;
        rend = GetComponent<MeshRenderer>();
    }
    //void Start()
    //{
    //    /* Clone the renderer’s material so belts with different speeds don’t fight
    //       over the same shared material.  If you *want* every belt segment to stay
    //       perfectly in sync, swap `.material` for `.sharedMaterial`. */
    //    beltMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    //}
    public void SetRunning(bool run)
    {
        enabled = run;
        if (run)
            beltMaterial = rend.sharedMaterial;
    }

    void Update()
    {
        if (rend.sharedMaterial != beltMaterial)
            beltMaterial = rend.sharedMaterial;

        uvOffset += scrollDirection.normalized * conveyorSpeed * Time.deltaTime;
        // Built-in RP keyword:
        beltMaterial.SetTextureOffset("_MainTex", uvOffset);

        // For URP / HDRP use this instead:
        // beltMaterial.SetTextureOffset("_BaseMap", uvOffset);
    }
}
