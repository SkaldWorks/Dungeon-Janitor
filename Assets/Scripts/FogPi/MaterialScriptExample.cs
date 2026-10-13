using UnityEngine;

public class MaterialScriptExample : MonoBehaviour
{
    public float speed = 0.0f;
    public float arrowOpacity = 0.0f;
    private Renderer renderer;
    private Material material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_Speed", speed);
        material.SetFloat("_ArrowOpacity", arrowOpacity);
    }
}
