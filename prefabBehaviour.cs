using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using QOutline = cakeslice.Outline;

public class ARPrefabBehaviour : MonoBehaviour
{
    #region Classes
    [System.Serializable]
    public class Section
    {
        public string sectionName;
        public List<GameObject> parentObjects;
    }

    [System.Serializable]
    public class EquipmentInfo
    {
        public string dispName;

        [Header("Service dates (dd/MM/yyyy)")]
        public string lastService;
        public string nextService;

        // key‑value spec list shown in Inspector
        public List<SpecKV> specifications = new List<SpecKV>();
    }

    [System.Serializable]
    public struct SpecKV
    {
        public string key;
        public string value;
    }
    #endregion

    #region Fields/Variables
    private List<float> scaleFactors = new List<float> { 0.05f, 0.1f, 0.15f, 0.2f };
    private int currentScaleIndex = 0;
    private Dictionary<string, bool> transparencyState = new Dictionary<string, bool>();
    private Renderer[] renderers;
    private ConveyorBeltAnimator[] beltAnimators;   // all belts in the scene
    private bool beltsRunning = false;              // current on/off state
    [SerializeField] private List<Section> sections = new List<Section>();
    [SerializeField] private Text displayText;
    [SerializeField] private List<EquipmentInfo> equipmentInfos;
    private const string conveyorSpec =
        "Manufacturer: <b>Adept</b>\n" +
        "Specifications: length = 3 m, height = 2 m, " +
        "speed = 30 m/min, capacity = 2.3 t";
    private readonly Dictionary<Renderer, QOutline> outlineCache =
        new Dictionary<Renderer, QOutline>();
    public AudioClip conveyorWorking;
    public AudioClip conveyorNotWorking;
    private AudioSource audioSource;
    private bool playWorkingSoundNext = true;
    #endregion

    void Awake()
    {
        // Drop an Outline on every renderer once, keep it disabled for now
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            var ol = r.gameObject.AddComponent<QOutline>();
            ol.color = 0;   // red (element 0 in OutlineEffect)
            ol.enabled = false;
            outlineCache[r] = ol;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null )  
            Debug.Log("No Audio"); 
    }
    void Start()
    {
        MainController.Instance.SetCurrentARPrefab(this);
        renderers = GetComponentsInChildren<Renderer>();
        beltAnimators = FindObjectsOfType<ConveyorBeltAnimator>();
        SetDisplayVisible(false);
    }

    public void SetSectionTransparency(string targetSectionName)
    {
        // -------- safety checks -------------------------------------------------
        var targetSection = sections.Find(s => s.sectionName == targetSectionName);
        if (targetSection == null || targetSection.parentObjects == null || targetSection.parentObjects.Count == 0)
        {
            Debug.LogWarning($"Section '{targetSectionName}' not found or empty!");
            return;
        }

        // -------- decide whether we’re entering or leaving “isolate” mode -------
        bool anyTranslucent = transparencyState.Values.Any(v => v);   // true ⇢ already isolating something
        bool enterIsolate = !anyTranslucent;                        // true ⇢ make others translucent
                                                                    // false ⇢ restore everything

        if (enterIsolate)
            ShowEquipmentInfo(targetSectionName);   // first click → show
        else
            SetDisplayVisible(false);                             // second click → hide

        // -------- apply alpha to every section ----------------------------------
        foreach (var section in sections)
        {
            if (section.parentObjects == null || section.parentObjects.Count == 0)
                continue;

            float alpha = 1f;  // default (normal state)

            if (enterIsolate)
                alpha = section.sectionName == targetSectionName ? 1f : 0.1f;

            foreach (var obj in section.parentObjects)
            {
                foreach (var rend in obj.GetComponentsInChildren<Renderer>())
                {
                    foreach (var mat in rend.materials)
                    {
                        SetMaterialAlpha(mat, alpha);
                    }

                    if (outlineCache.TryGetValue(rend, out var ol))
                        ol.enabled = enterIsolate &&               // only during isolate
                                     section.sectionName == targetSectionName;
                }
            }

            // update bookkeeping: true  ⇒ this section is translucent
            //                      false ⇒ this section is opaque
            transparencyState[section.sectionName] =
                enterIsolate && section.sectionName != targetSectionName;
        }
    }
    public void SetScaleByIndex(int index)
        {
            if (index >= 0 && index < scaleFactors.Count)
            {
                currentScaleIndex = index;
                float scale = scaleFactors[currentScaleIndex];
                transform.localScale = new Vector3(scale, scale, scale);
                Debug.Log("Scale set to: " + scale + "x");
                UpdateDisplay();
            }
            else
            {
                Debug.LogWarning("Invalid scale index passed to SetScaleByIndex");
            }
        }
    void SetMaterialAlpha(Material mat, float alpha)
    {
        Color c = mat.color;
        c.a = alpha;
        mat.color = c;

        if (alpha < 1f)            // --- TRANSPARENT ------------------------
        {
            mat.SetFloat("_Mode", 2f);                                   // Fade
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else                       // --- OPAQUE -----------------------------
        {
            mat.SetFloat("_Mode", 0f);                                   // Opaque
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        }
    }
    private void UpdateDisplay()
    {
        if (displayText != null)
        {
            displayText.text = $"Scale: {scaleFactors[currentScaleIndex]}x";
        }
    }
    private void SetDisplayVisible(bool visible)
    {
        if (displayText != null)
            displayText.gameObject.SetActive(visible);
    }
    public List<float> GetScaleFactors()
    {
        return scaleFactors;
    }
    string FormatEquipmentInfo(EquipmentInfo e)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"<b>{e.dispName}</b>");
        sb.AppendLine("Servicing:");
        sb.Append("  last = ").Append(e.lastService)
          .Append(", next = ").Append(e.nextService).AppendLine();

        sb.Append("Specifications:");
        for (int i = 0; i < e.specifications.Count; i++)
        {
            var s = e.specifications[i];
            sb.Append(i == 0 ? "  " : ", ");
            sb.Append(s.key).Append(" = ").Append(s.value);
        }
        return sb.ToString();
    }
    public void ShowEquipmentInfo(string sectionName)
    {
        var info = equipmentInfos.FirstOrDefault(i => i.dispName == sectionName);
        if (info == null)
        {
            Debug.LogWarning($"No EquipmentInfo found for '{sectionName}'");
            return;
        }

        displayText.text = FormatEquipmentInfo(info);
        displayText.gameObject.SetActive(true);
    }
    public void ToggleBeltsAndInfo()
    {
        beltsRunning = !beltsRunning;

        foreach (var belt in beltAnimators)
            if (belt != null)
                belt.SetRunning(beltsRunning);

                
        if (beltsRunning)
        {
            displayText.text = conveyorSpec;
            displayText.gameObject.SetActive(true);
            //AudioClip selectedClip = Random.value > 0.5f ? conveyorWorking : conveyorNotWorking;
            AudioClip selectedClip = playWorkingSoundNext ? conveyorWorking : conveyorNotWorking;
            if (selectedClip != null)
            {
                audioSource.clip = selectedClip;
                audioSource.loop = true;
                audioSource.Play();

                playWorkingSoundNext = !playWorkingSoundNext;
            }
            else
            {
                Debug.Log("No Clips");
            }
            
        }
        else
        {
            displayText.gameObject.SetActive(false);
            audioSource.Stop();
        }
    }

    #region Vuforia Tracking Handling
    // These methods will be called by Vuforia
    // Disables the top panel display text if there is no prefab 
    public void OnTrackingFound()
    {
        SetDisplayVisible(true);
    }
    public void OnTrackingLost()
    {
        SetDisplayVisible(false);
    }
    #endregion

}
