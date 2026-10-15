
using UnityEngine;
using UnityEngine.UI;

public class TripwireGraphic : MonoBehaviour
{
    [SerializeField] private RectTransform[] wireSegments;

    private bool isVisible;

    private void Awake()
    {
        if (wireSegments != null)
        {
            foreach (RectTransform segment in wireSegments)
            {
                if (segment == null)
                    continue;

                Image image = segment.GetComponent<Image>();

                if (image != null)
                    image.raycastTarget = false;
            }
        }

        Hide();
    }

    public bool CanDrawPath(int guideCount)
    {
        int requiredSegments = guideCount + 1;

        if (wireSegments == null ||
            wireSegments.Length < requiredSegments)
        {
            return false;
        }

        for (int i = 0; i < requiredSegments; i++)
        {
            if (wireSegments[i] == null ||
                wireSegments[i].GetComponent<Image>() == null)
            {
                return false;
            }
        }

        return true;
    }

    public void Show()
    {
        isVisible = true;
    }

    public void Hide()
    {
        isVisible = false;

        if (wireSegments == null)
            return;

        foreach (RectTransform segment in wireSegments)
        {
            if (segment != null)
                segment.gameObject.SetActive(false);
        }
    }

    public void SetPath(
        Vector3 start,
        RectTransform[] guides,
        int completedGuideCount,
        Vector3 end)
    {
        if (!isVisible || guides == null || wireSegments == null)
            return;

        int completedCount = Mathf.Clamp(
            completedGuideCount, 0, guides.Length);

        // One segment for each completed guide, plus the loose end.
        int activeSegmentCount = completedCount + 1;

        Vector3 segmentStart = start;

        for (int i = 0; i < wireSegments.Length; i++)
        {
            RectTransform segment = wireSegments[i];

            if (segment == null)
                continue;

            if (i >= activeSegmentCount)
            {
                segment.gameObject.SetActive(false);
                continue;
            }

            Vector3 segmentEnd = i < completedCount
                ? guides[i].position
                : end;

            SetSegment(segment, segmentStart, segmentEnd);

            segment.gameObject.SetActive(true);
            segmentStart = segmentEnd;
        }
    }

    private void SetSegment(
        RectTransform segment,
        Vector3 start,
        Vector3 end)
    {
        Vector3 direction = end - start;

        segment.position = (start + end) * 0.5f;

        Vector2 size = segment.sizeDelta;
        size.x = direction.magnitude;
        segment.sizeDelta = size;

        float angle = Mathf.Atan2(
            direction.y,
            direction.x) * Mathf.Rad2Deg;

        segment.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
