using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Task4Replayer : MonoBehaviour
{
    [Header("Recorder (to locate files / settings)")]
    public Task4Recorder recorder;

    [Header("Replay targets")]
    public Transform xrOrigin;          // XR Origin (XR Rig)
    public Transform cameraOffset;      // XR Origin/Camera Offset
    public Transform head;              // Main Camera
    public Transform leftController;    // Left Controller
    public Transform rightController;   // Right Controller

    [Header("Interactors + Interaction Manager (for forcing grabs)")]
    public XRInteractionManager interactionManager;
    public XRBaseInteractor leftInteractor;
    public XRBaseInteractor rightInteractor;

    [Header("Disable while replaying (IMPORTANT)")]
    public Behaviour[] disableWhileReplaying;

    [Header("Keys (NOT recorded)")]
    public Key replayKey = Key.Digit9;

    private bool _isReplaying;
    private bool _hooked;

    private List<Task4Recorder.Frame> _frames;
    private List<Task4Recorder.Event> _events;
    private float _dt;

    private int _frameIndex;
    private int _eventIndex;

    // Pose buffers applied in onBeforeRender 
    private Task4Recorder.PoseData _originWorld;
    private Task4Recorder.PoseData _headLocal;
    private Task4Recorder.PoseData _leftLocal;
    private Task4Recorder.PoseData _rightLocal;
    private float _currentT;

    private readonly Dictionary<string, IXRSelectInteractable> _interactables = new();

    private void OnEnable()
    {
        if (!_hooked)
        {
            Application.onBeforeRender += ApplyReplayPoses;
            _hooked = true;
        }
    }

    private void OnDisable()
    {
        if (_hooked)
        {
            Application.onBeforeRender -= ApplyReplayPoses;
            _hooked = false;
        }
    }

    private void Update()
    {
        if (_isReplaying) return;

        if (Keyboard.current != null && Keyboard.current[replayKey].wasPressedThisFrame)
            StartCoroutine(ReplayLatest());
    }

    private IEnumerator ReplayLatest()
    {
        _isReplaying = true;
        SetReplayMode(true);

        string folder = Path.Combine(Application.persistentDataPath, "Task4Recordings");
        if (!Directory.Exists(folder))
        {
            Debug.LogWarning("[Task4Replayer] No recordings folder found yet.");
            SetReplayMode(false);
            _isReplaying = false;
            yield break;
        }

        string latest = GetLatestJson(folder);
        if (string.IsNullOrEmpty(latest))
        {
            Debug.LogWarning("[Task4Replayer] No .json recordings found.");
            SetReplayMode(false);
            _isReplaying = false;
            yield break;
        }

        string json = File.ReadAllText(latest);
        var session = JsonUtility.FromJson<Task4Recorder.Session>(json);

        _frames = session.frames ?? new List<Task4Recorder.Frame>();
        _events = session.events ?? new List<Task4Recorder.Event>();
        _dt = (session.sampleInterval > 0f) ? session.sampleInterval : 0.02f;

        if (_frames.Count == 0)
        {
            Debug.LogWarning("[Task4Replayer] Recording has 0 frames.");
            SetReplayMode(false);
            _isReplaying = false;
            yield break;
        }

        Debug.Log($"[Task4Replayer] Replaying: {latest} | frames={_frames.Count} events={_events.Count}");

        // Build interactable lookup
        _interactables.Clear();
        foreach (var x in FindObjectsOfType<XRBaseInteractable>(true))
        {
            if (!_interactables.ContainsKey(x.transform.name))
                _interactables.Add(x.transform.name, x);
        }

        _frameIndex = 0;
        _eventIndex = 0;

        while (_frameIndex < _frames.Count)
        {
            var f = _frames[_frameIndex];

            // buffer poses 
            _originWorld = f.originWorld;
            _headLocal = f.headLocal;
            _leftLocal = f.leftLocal;
            _rightLocal = f.rightLocal;
            _currentT = f.t;

            // fire events up to this frame time
            while (_eventIndex < _events.Count && _events[_eventIndex].t <= _currentT)
            {
                FireEvent(_events[_eventIndex]);
                _eventIndex++;
            }

            _frameIndex++;

            // more stable than WaitForSeconds for replay
            yield return new WaitForSecondsRealtime(_dt);
        }

        Debug.Log("[Task4Replayer] Replay complete.");

        SetReplayMode(false);
        _isReplaying = false;
    }

    private void ApplyReplayPoses()
    {
        if (!_isReplaying) return;

        // Origin world pose (teleport/move)
        ApplyWorldPose(xrOrigin, _originWorld);

        // Camera Offset stays clean
        if (cameraOffset != null)
        {
            cameraOffset.localPosition = Vector3.zero;
            cameraOffset.localRotation = Quaternion.identity;
        }

        // Head and controllers local under Camera Offset
        ApplyLocalPose(head, _headLocal);
        ApplyLocalPose(leftController, _leftLocal);
        ApplyLocalPose(rightController, _rightLocal);
    }

    private void SetReplayMode(bool replaying)
    {
        if (disableWhileReplaying != null)
        {
            foreach (var b in disableWhileReplaying)
                if (b != null) b.enabled = !replaying;
        }

        
    }

    private void FireEvent(Task4Recorder.Event e)
    {
        if (interactionManager == null) return;

        XRBaseInteractor interactor =
            (e.hand == "Left") ? leftInteractor :
            (e.hand == "Right") ? rightInteractor : null;

        if (interactor == null) return;

        if (!_interactables.TryGetValue(e.targetName, out var target) || target == null)
            return;

        if (e.type == "Grab")
            interactionManager.SelectEnter(interactor, target);
        else if (e.type == "Release")
            interactionManager.SelectExit(interactor, target);
    }

    private static void ApplyWorldPose(Transform t, Task4Recorder.PoseData p)
    {
        if (t == null || p == null) return;
        t.SetPositionAndRotation(p.pos, p.rot);
    }

    private static void ApplyLocalPose(Transform t, Task4Recorder.PoseData p)
    {
        if (t == null || p == null) return;
        t.localPosition = p.pos;
        t.localRotation = p.rot;
    }

    private static string GetLatestJson(string folder)
    {
        var files = Directory.GetFiles(folder, "*.json");
        if (files.Length == 0) return null;

        string latest = files[0];
        var latestTime = File.GetLastWriteTime(latest);

        for (int i = 1; i < files.Length; i++)
        {
            var t = File.GetLastWriteTime(files[i]);
            if (t > latestTime)
            {
                latestTime = t;
                latest = files[i];
            }
        }
        return latest;
    }
}
