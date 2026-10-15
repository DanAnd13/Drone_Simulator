
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Esri.ArcGISMapsSDK.Components;

public class ArcGISStreamingController : MonoBehaviour
{
    [Header("ArcGIS Camera")]
    [SerializeField] private ArcGISCameraComponent arcGISCamera;
    [SerializeField] private Camera unityCamera;

    [Header("Async Experiment")]
    [SerializeField] private bool runAsyncExperiment = true;

    [Tooltip("Кількість одночасних робочих завдань.")]
    [SerializeField, Min(1)] private int workerCount = 4;

    [Tooltip("Кількість елементів в одному пакеті.")]
    [SerializeField, Min(1)] private int tilesPerWorker = 8;

    [Tooltip("Інтервал між запуском пакетів, секунд.")]
    [SerializeField, Min(0f)] private float batchInterval = 0.1f;

    [Header("Evaluation")]
    [SerializeField, Min(0.01f)]
    private float evaluationInterval = 0.25f;

    [Header("Adaptive Quality Scaling Factor")]
    [SerializeField] private bool adaptiveQuality = true;
    [SerializeField, Min(0.1f)] private float minimumQuality = 1f;
    [SerializeField, Min(0.1f)] private float maximumQuality = 2f;
    [SerializeField, Min(0.05f)] private float qualityStep = 0.25f;

    [Header("Movement thresholds")]
    [SerializeField, Min(0f)] private float fastMovementSpeed = 15f;
    [SerializeField, Min(0f)] private float slowMovementSpeed = 2f;
    [SerializeField, Min(0f)] private float fastRotationSpeed = 60f;
    [SerializeField, Min(0f)] private float slowRotationSpeed = 15f;

    [Header("Frame time thresholds")]
    [SerializeField, Min(1f)] private float overloadFrameTimeMs = 28f;
    [SerializeField, Min(1f)] private float recoveryFrameTimeMs = 20f;
    [SerializeField, Min(0.1f)] private float frameTimeSmoothingSeconds = 0.5f;
    [SerializeField, Min(0.1f)] private float stableTimeBeforeQualityIncrease = 2f;

    [Header("Optional render distance limit")]
    [SerializeField] private bool limitRenderDistance = false;
    [SerializeField, Min(100f)] private float maximumRenderDistance = 10000f;

    [Header("Diagnostics")]
    [SerializeField] private bool logDiagnostics = true;

    private float currentQuality;
    private float originalQuality;
    private bool hasOriginalQuality;
    private bool canControlQuality;

    private float originalFarClipPlane;
    private bool hasOriginalFarClipPlane;
    private bool originalUpdateClippingPlanes;
    private bool hasOriginalUpdateClippingPlanes;
    private bool renderDistanceLimitActive;

    private float frameTimeAverageMs;
    private bool hasFrameTimeSample;
    private float evaluationTimer;
    private float diagnosticTimer;
    private float stableTimer;
    private float batchTimer;

    private float measuredMovementSpeed;
    private float measuredRotationSpeed;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    private bool initialized;
    private bool shuttingDown;

    private CancellationTokenSource cancellation;
    private readonly List<Task> workers = new List<Task>();
    private readonly object workerLock = new object();

    private long processedItems;
    private long completedBatches;
    private long failedBatches;

    private void Start()
    {
        InitializeController();

        if (initialized && runAsyncExperiment)
            StartAsyncExperiment();
    }

    private void InitializeController()
    {
        if (arcGISCamera == null)
            arcGISCamera = FindObjectOfType<ArcGISCameraComponent>();

        if (arcGISCamera == null)
        {
            Debug.LogError("[ArcGISStreamingController] ArcGIS camera not found.");
            enabled = false;
            return;
        }

        if (unityCamera == null)
            unityCamera = arcGISCamera.GetComponent<Camera>();

        if (unityCamera == null)
            unityCamera = Camera.main;

        if (unityCamera == null)
        {
            Debug.LogError("[ArcGISStreamingController] Unity camera not found.");
            enabled = false;
            return;
        }

        if (TryGetFloat(arcGISCamera, "QualityScalingFactor", out originalQuality))
        {
            hasOriginalQuality = true;
            canControlQuality =
                FindMember(arcGISCamera, "QualityScalingFactor", true) != null;
            currentQuality = originalQuality;

            if (canControlQuality)
            {
                SetQuality(
                    Mathf.Clamp(currentQuality, minimumQuality, maximumQuality),
                    "Initialization");
            }
        }
        else
        {
            Debug.LogWarning(
                "[ArcGISStreamingController] QualityScalingFactor not found.");
        }

        InitializeRenderDistanceLimit();

        previousPosition = arcGISCamera.transform.position;
        previousRotation = arcGISCamera.transform.rotation;
        initialized = true;

        Debug.Log(
            $"[ArcGISStreamingController] Initialized. " +
            $"Async={runAsyncExperiment}, Workers={workerCount}, " +
            $"BatchSize={tilesPerWorker}, Quality={currentQuality:F2}");
    }

    private void Update()
    {
        if (!initialized)
            return;

        float dt = Time.unscaledDeltaTime;
        if (dt <= 0f)
            return;

        float frameTimeMs = dt * 1000f;

        if (!hasFrameTimeSample)
        {
            frameTimeAverageMs = frameTimeMs;
            hasFrameTimeSample = true;
        }
        else
        {
            float alpha = 1f - Mathf.Exp(
                -dt / Mathf.Max(0.01f, frameTimeSmoothingSeconds));

            frameTimeAverageMs =
                Mathf.Lerp(frameTimeAverageMs, frameTimeMs, alpha);
        }

        evaluationTimer += dt;
        diagnosticTimer += dt;
        batchTimer += dt;

        if (runAsyncExperiment && batchTimer >= batchInterval)
        {
            batchTimer = 0f;
            ScheduleBatches();
        }

        if (evaluationTimer >= evaluationInterval)
        {
            float elapsed = evaluationTimer;
            evaluationTimer = 0f;

            MeasureCameraMovement(elapsed);

            if (adaptiveQuality && canControlQuality)
                UpdateAdaptiveQuality(elapsed);
        }

        if (logDiagnostics && diagnosticTimer >= 1f)
        {
            diagnosticTimer = 0f;

            Debug.Log(
                $"[ArcGISStreamingController] " +
                $"Move={measuredMovementSpeed:F1}; " +
                $"Rotate={measuredRotationSpeed:F1}; " +
                $"FrameTime={frameTimeAverageMs:F1}ms; " +
                $"Quality={currentQuality:F2}; " +
                $"Workers={ActiveWorkerCount}; " +
                $"Processed={Interlocked.Read(ref processedItems)}; " +
                $"Batches={Interlocked.Read(ref completedBatches)}; " +
                $"Failed={Interlocked.Read(ref failedBatches)}");
        }
    }

    private int ActiveWorkerCount
    {
        get
        {
            lock (workerLock)
            {
                workers.RemoveAll(t => t.IsCompleted);
                return workers.Count;
            }
        }
    }

    private void StartAsyncExperiment()
    {
        cancellation = new CancellationTokenSource();

        Debug.Log(
            "[ArcGISStreamingController] Async experiment started. " +
            "Background workers process test batches only; " +
            "they do not download ArcGIS I3S nodes.");
    }

    private void ScheduleBatches()
    {
        if (cancellation == null || cancellation.IsCancellationRequested)
            return;

        lock (workerLock)
        {
            workers.RemoveAll(t => t.IsCompleted);

            while (workers.Count < Mathf.Max(1, workerCount))
            {
                Task task = Task.Run(
                    () => ProcessTestBatch(
                        Mathf.Max(1, tilesPerWorker),
                        cancellation.Token),
                    cancellation.Token);

                workers.Add(task);
            }
        }
    }

    // Експериментальний фоновий пакет.
    // Це навмисно не називається завантаженням I3S:
    // SDK не передає цьому класу список реальних вузлів/тайлів.
    private void ProcessTestBatch(int itemCount, CancellationToken token)
    {
        try
        {
            for (int i = 0; i < itemCount; i++)
            {
                token.ThrowIfCancellationRequested();

                // Мінімальна робота, що дозволяє перевірити
                // механізм планування Task.Run без доступу до Unity API.
                Thread.SpinWait(100);
                Interlocked.Increment(ref processedItems);
            }

            Interlocked.Increment(ref completedBatches);
        }
        catch (OperationCanceledException)
        {
            // Нормальне завершення під час вимкнення компонента.
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref failedBatches);
            Debug.LogWarning(
                "[ArcGISStreamingController] Worker error: " + ex.Message);
        }
    }

    private void MeasureCameraMovement(float elapsed)
    {
        Vector3 position = arcGISCamera.transform.position;
        Quaternion rotation = arcGISCamera.transform.rotation;

        measuredMovementSpeed =
            Vector3.Distance(position, previousPosition) /
            Mathf.Max(0.001f, elapsed);

        measuredRotationSpeed =
            Quaternion.Angle(rotation, previousRotation) /
            Mathf.Max(0.001f, elapsed);

        previousPosition = position;
        previousRotation = rotation;
    }

    private void UpdateAdaptiveQuality(float elapsed)
    {
        bool overloaded =
            frameTimeAverageMs >= overloadFrameTimeMs ||
            measuredMovementSpeed >= fastMovementSpeed ||
            measuredRotationSpeed >= fastRotationSpeed;

        if (overloaded)
        {
            stableTimer = 0f;

            SetQuality(
                Mathf.Max(minimumQuality, currentQuality - qualityStep),
                "Overload");

            return;
        }

        bool stable =
            measuredMovementSpeed <= slowMovementSpeed &&
            measuredRotationSpeed <= slowRotationSpeed &&
            frameTimeAverageMs <= recoveryFrameTimeMs;

        if (!stable)
        {
            stableTimer = 0f;
            return;
        }

        stableTimer += elapsed;

        if (stableTimer >= stableTimeBeforeQualityIncrease)
        {
            stableTimer = 0f;

            SetQuality(
                Mathf.Min(maximumQuality, currentQuality + qualityStep),
                "Stable performance");
        }
    }

    private void SetQuality(float value, string reason)
    {
        value = Mathf.Clamp(value, minimumQuality, maximumQuality);

        if (Mathf.Approximately(value, currentQuality))
            return;

        if (!TrySetMember(arcGISCamera, "QualityScalingFactor", value))
        {
            canControlQuality = false;
            Debug.LogError(
                "[ArcGISStreamingController] Cannot set QualityScalingFactor.");
            return;
        }

        currentQuality = value;

        if (logDiagnostics)
            Debug.Log($"[ArcGISStreamingController] Quality={value:F2}; {reason}");
    }

    private void InitializeRenderDistanceLimit()
    {
        if (!limitRenderDistance)
            return;

        if (!TryGetBool(
            arcGISCamera, "UpdateClippingPlanes",
            out originalUpdateClippingPlanes))
        {
            Debug.LogWarning(
                "[ArcGISStreamingController] UpdateClippingPlanes not found.");
            return;
        }

        hasOriginalUpdateClippingPlanes = true;

        if (!TrySetMember(arcGISCamera, "UpdateClippingPlanes", false))
            return;

        originalFarClipPlane = unityCamera.farClipPlane;
        hasOriginalFarClipPlane = true;

        unityCamera.farClipPlane = Mathf.Max(
            unityCamera.nearClipPlane + 1f, maximumRenderDistance);

        renderDistanceLimitActive = true;
    }

    private void LateUpdate()
    {
        if (!initialized || !renderDistanceLimitActive)
            return;

        unityCamera.farClipPlane = Mathf.Max(
            unityCamera.nearClipPlane + 1f, maximumRenderDistance);
    }

    private void OnDisable()
    {
        StopAsyncExperiment();

        if (arcGISCamera != null && hasOriginalQuality && canControlQuality)
            TrySetMember(arcGISCamera, "QualityScalingFactor", originalQuality);

        if (arcGISCamera != null && hasOriginalUpdateClippingPlanes)
            TrySetMember(
                arcGISCamera, "UpdateClippingPlanes",
                originalUpdateClippingPlanes);

        if (unityCamera != null && hasOriginalFarClipPlane)
            unityCamera.farClipPlane = originalFarClipPlane;
    }

    private void OnDestroy()
    {
        StopAsyncExperiment();
    }

    private void StopAsyncExperiment()
    {
        if (shuttingDown)
            return;

        shuttingDown = true;

        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }
    }

    private static bool TryGetFloat(
        Component target, string name, out float value)
    {
        value = 0f;

        if (!TryGetMember(target, name, out object raw) || raw == null)
            return false;

        try
        {
            value = Convert.ToSingle(raw);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryGetBool(
        Component target, string name, out bool value)
    {
        value = false;

        if (!TryGetMember(target, name, out object raw) || !(raw is bool))
            return false;

        value = (bool)raw;
        return true;
    }

    private static bool TryGetMember(
        Component target, string name, out object value)
    {
        value = null;
        MemberInfo member = FindMember(target, name, false);

        try
        {
            if (member is PropertyInfo property)
                value = property.GetValue(target);
            else if (member is FieldInfo field)
                value = field.GetValue(target);
            else
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TrySetMember(
        Component target, string name, object value)
    {
        MemberInfo member = FindMember(target, name, true);

        try
        {
            if (member is PropertyInfo property)
            {
                property.SetValue(
                    target, Convert.ChangeType(value, property.PropertyType));
                return true;
            }

            if (member is FieldInfo field)
            {
                field.SetValue(
                    target, Convert.ChangeType(value, field.FieldType));
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(
                $"[ArcGISStreamingController] Cannot set {name}: {ex.Message}");
        }

        return false;
    }

    private static MemberInfo FindMember(
        Component target, string name, bool forWrite)
    {
        if (target == null)
            return null;

        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;

        for (Type type = target.GetType();
             type != null;
             type = type.BaseType)
        {
            foreach (PropertyInfo property in type.GetProperties(flags))
            {
                if (!string.Equals(
                    property.Name, name,
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                if (property.GetIndexParameters().Length != 0)
                    continue;

                MethodInfo accessor = forWrite
                    ? property.GetSetMethod(true)
                    : property.GetGetMethod(true);

                if (accessor != null)
                    return property;
            }

            foreach (FieldInfo field in type.GetFields(flags))
            {
                if (!string.Equals(
                    field.Name, name,
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                if (forWrite && (field.IsInitOnly || field.IsLiteral))
                    continue;

                return field;
            }
        }

        return null;
    }
}
