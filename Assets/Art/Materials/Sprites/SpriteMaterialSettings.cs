
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class SpriteMaterialSettings : MonoBehaviour
{
    [Header("Per-Object Settings")]
    public Texture2D spriteTexture;

    [Range(0f, 1f)]
    public float lightingAmount = 1f;

    private Renderer targetRenderer;
    private MaterialPropertyBlock propertyBlock;

    private static readonly int imageID =
        Shader.PropertyToID("_Image");

    private static readonly int LightingAmountID =
        Shader.PropertyToID("_LightingAmount");

    private void OnEnable()
    {
        ApplySettings();
    }

    private void OnValidate()
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer == null)
            return;

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        // Preserve other property overrides on this renderer.
        targetRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetTexture(
            imageID, spriteTexture);

        propertyBlock.SetFloat(
            LightingAmountID, lightingAmount);

        targetRenderer.SetPropertyBlock(propertyBlock);
    }
}
