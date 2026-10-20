using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public float riseHeight = 3f;
    public float riseSpeed = 2f;

    [Header("Requirements")]
    public bool canOpen;

    [SerializeField, HideInInspector]
    private string saveId;

    public string SaveId => saveId;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen;
    private bool saveStateLoaded;

    // Handles unlock updates that occur before Start restores
    // the saved state. This avoids overwriting saved data
    // because different scripts start in different orders.
    private bool hasPendingCanOpen;
    private bool pendingCanOpen;

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * riseHeight;
    }

    private void Start()
    {
        if (SaveManager.Instance != null &&
            SaveManager.Instance.TryGetDoorState(
                saveId,
                out bool savedCanOpen,
                out bool savedIsOpen))
        {
            canOpen = savedCanOpen;
            isOpen = savedIsOpen;
        }

        saveStateLoaded = true;

        // DungeonProgressManager may determine the current
        // unlock status before this component finishes loading.
        if (hasPendingCanOpen)
            canOpen = pendingCanOpen;

        // Restore an already-open door at its final position.
        if (isOpen)
            transform.position = openPosition;

        if (hasPendingCanOpen)
            SaveCurrentState();
    }

    private void Update()
    {
        if (!isOpen)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            openPosition,
            riseSpeed * Time.deltaTime
        );
    }

    public void Interact()
    {
        if (!canOpen || isOpen)
            return;

        isOpen = true;
        SaveCurrentState();
    }

    public void SetCanOpen(bool value)
    {
        canOpen = value;

        if (!saveStateLoaded)
        {
            hasPendingCanOpen = true;
            pendingCanOpen = value;
            return;
        }

        SaveCurrentState();
    }

    // Keeps compatibility with existing Unity events
    // or scripts that call SetCanOpen() without an argument.
    public void SetCanOpen()
    {
        SetCanOpen(true);
    }

    public void OnTouchingPlayer()
    {
    }

    public void OnNotTouchingPlayer()
    {
    }

    private void SaveCurrentState()
    {
        if (SaveManager.Instance == null)
            return;

        SaveManager.Instance.SaveDoorState(
            saveId,
            canOpen,
            isOpen
        );
    }

#if UNITY_EDITOR
    private void Reset()
    {
        GenerateSaveIdIfNeeded();
    }

    private void OnValidate()
    {
        GenerateSaveIdIfNeeded();
    }

    private void GenerateSaveIdIfNeeded()
    {
        if (!string.IsNullOrWhiteSpace(saveId))
            return;

        saveId = Guid.NewGuid().ToString("N");
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Regenerate Save ID")]
    private void RegenerateSaveId()
    {
        saveId = Guid.NewGuid().ToString("N");
        EditorUtility.SetDirty(this);
    }
#endif
}