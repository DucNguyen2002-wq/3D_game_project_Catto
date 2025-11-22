using UnityEngine;

public class CollectionAreaMarker : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color areaColor = new Color(0, 1, 0, 0.3f); // Xanh lá trong su?t
    public float pulseSpeed = 1f; // T?c ?? nh?p nháy
    
    private Material areaMaterial;
    private Renderer areaRenderer;
    private float pulseTimer = 0f;

    void Start()
    {
        // L?y renderer
        areaRenderer = GetComponent<Renderer>();
        
        if (areaRenderer != null)
        {
            // T?o material m?i
            areaMaterial = new Material(Shader.Find("Standard"));
            areaMaterial.color = areaColor;
            areaMaterial.SetFloat("_Mode", 3); // Transparent mode
            areaMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            areaMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            areaMaterial.SetInt("_ZWrite", 0);
            areaMaterial.DisableKeyword("_ALPHATEST_ON");
            areaMaterial.EnableKeyword("_ALPHABLEND_ON");
            areaMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            areaMaterial.renderQueue = 3000;
            
            areaRenderer.material = areaMaterial;
        }
    }

    void Update()
    {
        // Hi?u ?ng nh?p nháy
        if (areaMaterial != null)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float alpha = Mathf.Lerp(0.2f, 0.5f, (Mathf.Sin(pulseTimer) + 1f) / 2f);
            
            Color newColor = areaColor;
            newColor.a = alpha;
            areaMaterial.color = newColor;
        }
    }
}
