using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Task4Recorder : MonoBehaviour
{
    [Header("Rig References")]
    public Transform xrOrigin;          // XR Origin (XR Rig)
    public Transform cameraOffset;      // XR Origin/Camera Offset
    public Transform head;              // Main Camera 
    public Transform leftController;    // Left Controller 
    public Transform rightController;   // Right Controller 

    [Header("Grab sources (interactors)")]
    public XRBaseInteractor leftInteractor;
    public XRBaseInteractor rightInteractor;

    [Header("Teleport detection (optional)")]
    public float teleportDetectDistance = 1.5f;

    [Header("Sampling")]
    public float sampleInterval = 0.02f; // 50Hz

    [Header("Keys (NOT recorded)")]
    public Key recordToggleKey = Key.Digit8; //key 8  

    public bool IsRecording { get; private set; }

    private float _accum;
    private Vector3 _lastOriginPos;

    private readonly List<Frame> _frames = new();
    private readonly List<Event> _events = new();

    [Serializable] public class Session
    {
        public float sampleInterval;
        public List<Frame> frames;
        public List<Event> events;
    }

    [Serializable] public class Frame
    {
        public float t;                 // Time.time
        public PoseData originWorld;    // xrOrigin world pose
        public PoseData headLocal;      // local under Camera Offset
        public PoseData leftLocal;      // local under Camera Offset
        public PoseData rightLocal;     // local under Camera Offset
        public bool teleportedThisFrame;
    }

    [Serializable] public class PoseData
    {
        public Vector3 pos;
        public Quaternion rot;
    }

    [Serializable] public class Event
    {
        public float t;
        public string type;         // Grab / Release
        public string hand;         // Left / Right
        public string targetName;   // interactable GameObject name
    }

    private void Awake()
    {
        if (!xrOrigin || !cameraOffset || !head || !leftController || !rightController)
            Debug.LogWarning("[Task4Recorder] Missing rig references in Inspector.");

        if (leftInteractor != null)
        {
            leftInteractor.selectEntered.AddListener(args => LogGrab("Left", args.interactableObject));
            leftInteractor.selectExited.AddListener(args => LogRelease("Left", args.interactableObject));
        }

        if (rightInteractor != null)
        {
            rightInteractor.selectEntered.AddListener(args => LogGrab("Right", args.interactableObject));
            rightInteractor.selectExited.AddListener(args => LogRelease("Right", args.interactableObject));
        }

        if (xrOrigin) _lastOriginPos = xrOrigin.position;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current[recordToggleKey].wasPressedThisFrame)
        {
            if (!IsRecording) StartRecording();
            else StopAndSaveRecording();
        }

        if (!IsRecording) return;

        _accum += Time.deltaTime;
        if (_accum < sampleInterval) return;
        _accum = 0f;

        // Teleport detection: large jump in origin
        bool teleported = false;
        if (xrOrigin)
        {
            float jump = Vector3.Distance(xrOrigin.position, _lastOriginPos);
            teleported = jump >= teleportDetectDistance;
            _lastOriginPos = xrOrigin.position;
        }

        _frames.Add(new Frame
        {
            t = Time.time,
            originWorld = ToWorldPose(xrOrigin),
            headLocal = ToLocalPose(head),
            leftLocal = ToLocalPose(leftController),
            rightLocal = ToLocalPose(rightController),
            teleportedThisFrame = teleported
        });
    }

    public void StartRecording()
    {
        _frames.Clear();
        _events.Clear();
        IsRecording = true;
        _accum = 0f;
        if (xrOrigin) _lastOriginPos = xrOrigin.position;

        Debug.Log("[Task4Recorder] Recording started.");
    }

    public void StopAndSaveRecording()
    {
        IsRecording = false;

        var session = new Session
        {
            sampleInterval = sampleInterval,
            frames = _frames,
            events = _events
        };

        string json = JsonUtility.ToJson(session, true);

        string folder = Path.Combine(Application.persistentDataPath, "Task4Recordings");
        Directory.CreateDirectory(folder);

        string file = Path.Combine(folder, $"session_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        File.WriteAllText(file, json);

        Debug.Log($"[Task4Recorder] Recording saved: {file}");
        Debug.Log($"[Task4Recorder] Folder: {folder}");
    }

    private void LogGrab(string hand, IXRSelectInteractable interactable)
    {
        if (!IsRecording || interactable == null) return;
        _events.Add(new Event { t = Time.time, type = "Grab", hand = hand, targetName = interactable.transform.name });
    }

    private void LogRelease(string hand, IXRSelectInteractable interactable)
    {
        if (!IsRecording || interactable == null) return;
        _events.Add(new Event { t = Time.time, type = "Release", hand = hand, targetName = interactable.transform.name });
    }

    private static PoseData ToWorldPose(Transform t)
    {
        if (!t) return new PoseData();
        return new PoseData { pos = t.position, rot = t.rotation };
    }

    private static PoseData ToLocalPose(Transform t)
    {
        if (!t) return new PoseData();
        return new PoseData { pos = t.localPosition, rot = t.localRotation };
    }
}
