using UnityEngine;
using UnityEngine.UI;

public class GoopClean : MonoBehaviour, IMinigame
{
    public RawImage image;

    public Texture2D dirtyTexture;
    public Texture2D cleanTexture;

    public int brushSize = 30;

    [Range(0f, 1f)]
    public float cleanRequired = 0.9f;

    [Range(0f, 1f)]
    public float alphaThreshold = 0.01f;

    public float cleanSpeed = 0.5f;

    private Texture2D workingTexture;
    private StartGame source;

    private Color[] dirtyPixels;
    private Color[] cleanPixels;
    private Color[] workingPixels;

    // 0 = completely dirty, 1 = completely clean.
    private float[] cleanedPixels;

    private int totalVisiblePixels;
    private int fullyCleanedPixels;

    private Vector2 lastPosition;
    private bool hasLastPosition;
    private bool textureChanged;

    public void StartMinigame(StartGame source)
    {
        if (image == null || dirtyTexture == null || cleanTexture == null)
        {
            Debug.LogError("GoopClean: Assign Image, Dirty Texture, and Clean Texture.");
            return;
        }

        if (dirtyTexture.width != cleanTexture.width ||
            dirtyTexture.height != cleanTexture.height)
        {
            Debug.LogError("GoopClean: Textures must be the same size.");
            return;
        }

        this.source = source;

        if (workingTexture != null)
            Destroy(workingTexture);

        int width = dirtyTexture.width;
        int height = dirtyTexture.height;
        int pixelCount = width * height;

        dirtyPixels = dirtyTexture.GetPixels();
        cleanPixels = cleanTexture.GetPixels();
        workingPixels = new Color[pixelCount];

        cleanedPixels = new float[pixelCount];

        totalVisiblePixels = 0;
        fullyCleanedPixels = 0;

        for (int i = 0; i < pixelCount; i++)
        {
            Color dirty = dirtyPixels[i];
            Color clean = cleanPixels[i];

            if (dirty.a > alphaThreshold)
            {
                totalVisiblePixels++;

                workingPixels[i] = dirty;
            }
            else
            {
                workingPixels[i] = clean;
            }
        }

        workingTexture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        workingTexture.SetPixels(workingPixels);
        workingTexture.Apply();

        image.texture = workingTexture;

        hasLastPosition = false;
        textureChanged = false;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (workingTexture == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 position;

            if (GetPixelPosition(Input.mousePosition, out position))
            {
                lastPosition = position;
                hasLastPosition = true;

                CleanLine(position, position);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 position;

            if (!GetPixelPosition(Input.mousePosition, out position))
            {
                hasLastPosition = false;
                return;
            }

            if (hasLastPosition)
                CleanLine(lastPosition, position);
            else
                CleanLine(position, position);

            lastPosition = position;
            hasLastPosition = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            hasLastPosition = false;
        }

        if (textureChanged)
        {
            workingTexture.SetPixels(workingPixels);
            workingTexture.Apply();

            textureChanged = false;
        }
    }

    private bool GetPixelPosition(
        Vector2 mousePosition,
        out Vector2 position)
    {
        position = Vector2.zero;

        Camera cam = null;

        if (image.canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = image.canvas.worldCamera;

        Vector2 localPosition;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            image.rectTransform,
            mousePosition,
            cam,
            out localPosition))
        {
            return false;
        }

        float x = Mathf.InverseLerp(
            image.rectTransform.rect.xMin,
            image.rectTransform.rect.xMax,
            localPosition.x
        );

        float y = Mathf.InverseLerp(
            image.rectTransform.rect.yMin,
            image.rectTransform.rect.yMax,
            localPosition.y
        );

        if (x < 0f || x > 1f || y < 0f || y > 1f)
            return false;

        position = new Vector2(
            x * (dirtyTexture.width - 1),
            y * (dirtyTexture.height - 1)
        );

        return true;
    }

    private void CleanLine(Vector2 start, Vector2 end)
    {
        int size = Mathf.Max(1, brushSize);

        float distance = Vector2.Distance(start, end);

        // keeps it from skipping pixels when the mouse moves fast
        int steps = Mathf.Max(
            1,
            Mathf.CeilToInt(distance / size)
        );

        //moving the mouse faster doesnt make you clean faster
        float amount = (cleanSpeed * Time.deltaTime) / steps;

        for (int i = 0; i <= steps; i++)
        {
            Vector2 position = Vector2.Lerp(
                start,
                end,
                i / (float)steps
            );

            CleanAt(position, amount);
        }

        if (totalVisiblePixels > 0)
        {
            float progress =
                fullyCleanedPixels / (float)totalVisiblePixels;

            if (progress >= cleanRequired)
            {
                CompleteMinigame();
            }
        }
    }

    private void CleanAt(Vector2 position, float amount)
    {
        int centerX = Mathf.RoundToInt(position.x);
        int centerY = Mathf.RoundToInt(position.y);

        int size = Mathf.Max(1, brushSize);

        for (int y = -size; y <= size; y++)
        {
            for (int x = -size; x <= size; x++)
            {
                if (x * x + y * y > size * size)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= dirtyTexture.width ||
                    py < 0 || py >= dirtyTexture.height)
                    continue;

                int index = py * dirtyTexture.width + px;

                // Transparent parts of the PNG aren't considered goop,
                if (dirtyPixels[index].a <= alphaThreshold)
                    continue;

                // Prevent the same pixel from being counted twice
                if (cleanedPixels[index] >= 1f)
                    continue;

                cleanedPixels[index] += amount;

                if (cleanedPixels[index] >= 1f)
                {
                    cleanedPixels[index] = 1f;
                    fullyCleanedPixels++;
                }

                // Blend between the dirty and clean versions based on how much this pixel has been washed
                workingPixels[index] = Color.Lerp(
                    dirtyPixels[index],
                    cleanPixels[index],
                    cleanedPixels[index]
                );

                textureChanged = true;
            }
        }
    }

    private void CompleteMinigame()
    {
        hasLastPosition = false;

        if (source != null)
            source.CompleteRepair();

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (workingTexture != null)
            Destroy(workingTexture);
    }
}