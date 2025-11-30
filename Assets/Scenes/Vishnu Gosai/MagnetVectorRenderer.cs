using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MagnetVectorRenderer : MonoBehaviour
{
    private SpriteShapeController controller;
    private SpriteShapeRenderer renderer2D;
    private Spline spline;

    private MagnetSpawnerScript spawner;
    private Transform magnetTransform;

    [Header("Anchors")]
    [Tooltip("Where tether starts. If null, uses this transform.")]
    public Transform baseTransform;

    public string magnetCloneName = "MagnetProjectile(Clone)";
    public string magnetLayerName = "Magnet";

    [Header("Point Spacing")]
    [Tooltip("Interval used to decide how many points we WANT.")]
    public float dropInterval = 10f;

    [Tooltip("If magnet gets closer than this to base, reset line.")]
    public float recallResetDistance = 1.0f;

    [Header("Stop Detection (flag only)")]
    [Tooltip("Movement below this per frame counts as 'not moving'.")]
    public float moveThreshold = 0.15f;

    [Header("Stop Detection Timing")]
    [Tooltip("How long the magnet must stay below moveThreshold before we consider it stopped.")]
    public float stoppedMinTime = 0.08f;
    private float stoppedTimer;

    [Header("Electric Arc Parameters")]
    [Tooltip("Overall max perpendicular displacement for the arc.")]
    public float arcAmplitude = 1.5f;

    [Tooltip("Base frequency for the noise along the line.")]
    public float arcFrequency = 1.0f;

    [Tooltip("Number of noise layers (higher = more jagged detail).")]
    [Range(1, 8)]
    public int arcOctaves = 4;

    [Tooltip("Amplitude falloff per octave (0.3–0.7 is typical).")]
    [Range(0.05f, 0.95f)]
    public float arcRoughness = 0.5f;

    [Tooltip("How fast the arc animates over time in SMOOTH mode (time multiplier).")]
    public float arcSpeed = 2.0f;

    [Tooltip("Extra randomization so multiple arcs don't look identical.")]
    public int noiseSeed = 0;

    [Header("Stepped Arc Motion (Optional)")]
    [Tooltip("If ON, arc points jump to new noise positions, then hold for a random duration.")]
    public bool steppedArc = false;

    [Tooltip("Random hold duration range (seconds) per step. Each step picks a new value in this range.")]
    public Vector2 steppedHoldRange = new Vector2(0.05f, 0.15f);

    [Header("Corner Sharpening")]
    [Tooltip("Chance each interior point becomes a sharp 'kink'.")]
    [Range(0f, 1f)]
    public float cornerChance = 0.35f;

    [Tooltip("How often corner pattern refreshes (seconds).")]
    public float cornerReseedInterval = 0.12f;

    [Tooltip("Tangents scale for non-corner points (1 = smooth, lower = tighter).")]
    [Range(0.05f, 1f)]
    public float smoothTangentScale = 0.6f;

    [Tooltip("Tangents scale for corner points (0 = very sharp corner).")]
    [Range(0f, 0.5f)]
    public float cornerTangentScale = 0.0f;

    [Header("Sparks (Optional Endpoints)")]
    [Tooltip("Particles at the player end.")]
    public Transform baseSparks;

    [Tooltip("Particles at the magnet end.")]
    public Transform magnetSparks;

    [Header("Master Array Mode")]
    [Tooltip("If ON, this script does all spline math but stores it in masterArray instead of writing to a SpriteShape.")]
    public bool isMasterArray = false;

    [Header("Tethered Child Arcs (optional)")]
    [Tooltip("When ON (on the master), children that reference this as master will use tethered child arcs instead of copying a continuous segment. Also makes the master visible.")]
    public bool canGenerateTetheredArcs = false;

    [Tooltip("Max extra interior points a tethered child arc may have beyond the minimum determined by the sampled master range.")]
    public int childMaxExtraPoints = 6;

    [Tooltip("Range for initial perpendicular amplitude of tethered child arcs (wave center).")]
    public Vector2 childArcAmplitudeRange = new Vector2(0.2f, 0.6f);

    [Header("Tethered Child Movement")]
    [Tooltip("If ON, tethered child arcs will move one of their anchors stepwise along the master spline.")]
    public bool enableMovement = false;

    [Tooltip("Range of time each moving anchor stays on a master point before hopping to the next.")]
    public Vector2 childAnchorStepTimeRange = new Vector2(0.03f, 0.08f);

    [Header("Extra Chaos Noise (optional)")]
    [Tooltip("If ON, apply additional chaotic jitter (perp + parallel) to this arc (main or child).")]
    public bool enableExtraChaos = false;

    [Tooltip("Extra perpendicular jitter amplitude for chaos noise.")]
    public float extraChaosPerpJitter = 0.4f;

    [Tooltip("Extra parallel jitter amplitude along the chord for chaos noise.")]
    public float extraChaosParallelJitter = 0.3f;

    [Tooltip("Chaos noise frequency multiplier along the arc.")]
    public float extraChaosFrequency = 3.0f;

    [Tooltip("Chaos noise speed multiplier over time.")]
    public float extraChaosSpeed = 2.0f;

    [System.Serializable]
    public struct MasterPoint
    {
        public Vector2 position;
        public Vector2 leftTangent;
        public Vector2 rightTangent;
        public ShapeTangentMode tangentMode;
    }

    [Header("Master Array (runtime)")]
    public MasterPoint[] masterArray;

    [Header("Child of Master Array Mode")]
    [Tooltip("If ON, this instance ignores its own tether math and copies data from a master MagnetVectorRenderer that has isMasterArray = true.")]
    public bool isChildToMasterArray = false;

    [Tooltip("Reference to the master MagnetVectorRenderer (typically on the parent GameObject).")]
    public MagnetVectorRenderer masterArraySource;

    [Header("Child Copy Settings (Master -> Child)")]
    [Tooltip("Fraction [0,1] of the master arc to use for child arcs. In legacy mode it controls segment length in points; in tethered mode it controls how much of the arc the two anchor points span.")]
    public Vector2 childCopyFractionRange = new Vector2(0.4f, 1.0f);

    [Tooltip("Only used in legacy mode (master.canGenerateTetheredArcs is OFF). If ON, child samples a snapshot of a continuous segment and holds it for a lifetime before resampling.")]
    public bool sharpChild = false;

    [Tooltip("Lifetime range (seconds) for each sampled child snapshot.")]
    public Vector2 childLifetimeRange = new Vector2(0.05f, 0.15f);

    [Tooltip("Extra random delay after each lifetime before the next resample (seconds). Gap between arcs.")]
    public Vector2 childDelayRange = new Vector2(0f, 0.05f);

    [Header("Random Sparks Along Spline")]
    [Tooltip("Template particle system used for random sparks along the spline (cloned into a pool on Awake).")]
    public ParticleSystem sparks;

    [Tooltip("Range of time between each 'batch' of random sparks along the spline (seconds).")]
    public Vector2 sparksIntervalRange = new Vector2(0.05f, 0.2f);

    [Tooltip("How long each spark instance plays (seconds).")]
    public Vector2 sparksDurationRange = new Vector2(0.05f, 0.15f);

    [Tooltip("Percent of interior spline points used for sparks each interval (0–1).")]
    [Range(0f, 1f)]
    public float sparksDensity = 0.3f;

    [Tooltip("Maximum number of sparks that may be active at once (size of the pool).")]
    public int maxConcurrentSparks = 5;

    [Header("Material / Alpha")]
    [Tooltip("Per-arc opacity that drives the _alpha property on the electric material.")]
    [Range(0f, 1f)]
    public float arcAlpha = 1f;

    [Header("Visibility")]
    [Tooltip("If ON, this arc (and child arcs) only render while the magnet is stopped (has hit the wall).")]
    public bool onlyShowWhenStopped = false;

    [Header("State Flags")]
    public bool magnetMoving;
    public bool magnetIsStopped;

    [Header("Debug")]
    public float splineLength;
    public int desiredInteriorPoints;

    private bool lastMagnetActive;
    private Vector3 lastMagnetWorldPos;
    private bool hasLastPos;

    // cached particle systems (optional)
    private ParticleSystem basePS;
    private ParticleSystem magnetPS;

    // ----- stepped arc internals -----
    private bool lastSteppedArc;
    private bool steppedInitialized;
    private float steppedSampleTime; // time fed into noise; only changes on step
    private float nextStepTime;      // when to jump next
    private float currentHold;       // current step hold duration

    // ----- legacy child sharp sampling internals -----
    private MasterPoint[] childCachedSegment;
    private bool childHasCachedSegment;
    private float childLifeEndTime;   // when current child segment stops being visible
    private float childDelayEndTime;  // when we are allowed to sample the next segment

    // ----- tethered child arc internals -----
    private bool tetherHasArc;
    private int tetherAnchorMasterIndexA;
    private int tetherAnchorMasterIndexB;
    private int tetherChildPointCount;
    private float tetherLifeEndTime;
    private float tetherNextSpawnTime;
    private float tetherAmplitude;
    private Vector3 tetherPerpLocalDir;

    // movement internals for tethered child arcs
    private bool tetherHasMovingAnchor;
    private bool tetherMovingAnchorIsA;
    private int tetherMovementDirection; // +1 toward magnet (end), -1 toward player (start)
    private float tetherNextMoveTime;

    // ----- random sparks pool -----
    private ParticleSystem[] sparksPool;
    private float[] sparkEndTimes;
    private int sparksPoolSize;
    private float nextSparksTime;

    // ----- MPB internals -----
    private MaterialPropertyBlock arcMPB;
    private bool hasArcMPB;
    private float lastArcAlpha = -1f;

    // ----- burst visibility (controlled by manager) -----
    private bool burstVisible = true;

    private bool SparksEnabled
        => sparks != null && maxConcurrentSparks > 0;

    void Awake()
    {
        controller = GetComponent<SpriteShapeController>();
        renderer2D = GetComponent<SpriteShapeRenderer>();
        if (controller != null)
            spline = controller.spline;

        // Only master / normal need the spawner; child copies geometry only.
        if (!isChildToMasterArray)
        {
            spawner = FindSpawnerOnSameLevel();
            if (spawner == null)
                Debug.LogWarning("MagnetVectorRenderer: No MagnetSpawnerScript found.");
        }

        if (baseSparks != null)
            basePS = baseSparks.GetComponentInChildren<ParticleSystem>(true);

        if (magnetSparks != null)
            magnetPS = magnetSparks.GetComponentInChildren<ParticleSystem>(true);

        InitRandomSparksPool();

        // INIT depending on mode
        if (!isMasterArray && !isChildToMasterArray)
        {
            if (spline != null && controller != null)
            {
                InitTwoPointLine();
            }

            if (renderer2D != null)
                renderer2D.enabled = false;
        }
        else if (isChildToMasterArray)
        {
            if (controller == null || renderer2D == null)
                Debug.LogWarning($"{name}: isChildToMasterArray is ON but no SpriteShapeController/Renderer found.");
        }
        else if (isMasterArray)
        {
            // Start hidden; we'll enable later if canGenerateTetheredArcs is true and magnet state allows.
            if (renderer2D != null)
                renderer2D.enabled = false;
        }

        lastSteppedArc = steppedArc;
        steppedInitialized = false;
        childHasCachedSegment = false;
        stoppedTimer = 0f;

        tetherHasArc = false;
        tetherHasMovingAnchor = false;
        tetherNextSpawnTime = Time.time;
        tetherNextMoveTime = Time.time;
    }

    void Update()
    {
        // ----- CHILD MODE (copies from master array) -----
        if (isChildToMasterArray)
        {
            UpdateFromMasterArray();
            UpdateAlphaOnMaterial();
            return;
        }

        // ----- NORMAL / MASTER-ARRAY MODE -----

        if (spawner == null) return;

        bool magnetActive = spawner.magnetActive;

        if (!magnetActive)
        {
            if (lastMagnetActive)
            {
                if (!isMasterArray && spline != null)
                    InitTwoPointLine();

                magnetTransform = null;
                hasLastPos = false;
                stoppedTimer = 0f;
            }

            if (!isMasterArray && renderer2D != null)
                renderer2D.enabled = false;

            lastMagnetActive = false;
            magnetIsStopped = false;
            magnetMoving = false;

            // reset stepped state so next activation snaps fresh
            steppedInitialized = false;
            lastSteppedArc = steppedArc;

            if (isMasterArray)
                masterArray = null;

            UpdateSparks(null, null);
            StopAllRandomSparks();
            UpdateAlphaOnMaterial();
            return;
        }

        if (magnetTransform == null)
        {
            TryCacheMagnet();
            lastMagnetActive = true;
            if (magnetTransform == null)
            {
                UpdateSparks(null, null);
                StopAllRandomSparks();
                UpdateAlphaOnMaterial();
                return;
            }
        }

        DetectMovingFlag();

        Transform baseT = baseTransform != null ? baseTransform : transform;
        Vector3 baseLocal = transform.InverseTransformPoint(baseT.position);
        Vector3 endLocal  = transform.InverseTransformPoint(magnetTransform.position);

        Vector3 delta = endLocal - baseLocal;
        splineLength = delta.magnitude;

        if (splineLength <= recallResetDistance)
        {
            if (!isMasterArray && spline != null)
                InitTwoPointLine();

            hasLastPos = false;
            stoppedTimer = 0f;
            lastMagnetActive = true;
            magnetIsStopped = false;

            steppedInitialized = false;
            lastSteppedArc = steppedArc;

            if (isMasterArray)
                masterArray = null;

            UpdateSparks(null, null);
            StopAllRandomSparks();
            UpdateAlphaOnMaterial();
            return;
        }

        if (splineLength < 0.0001f)
        {
            if (!isMasterArray && spline != null)
                InitTwoPointLine();

            lastMagnetActive = true;
            magnetIsStopped = true;
            magnetMoving = false;
            stoppedTimer = stoppedMinTime; // treat as fully stopped

            steppedInitialized = false;
            lastSteppedArc = steppedArc;

            bool baseShow = !onlyShowWhenStopped || magnetIsStopped;
            bool finalShow = baseShow && burstVisible;

            if (renderer2D != null)
            {
                if (isMasterArray)
                    renderer2D.enabled = canGenerateTetheredArcs && finalShow;
                else
                    renderer2D.enabled = finalShow;
            }

            if (finalShow && (!isMasterArray || canGenerateTetheredArcs))
            {
                UpdateSparks(baseLocal, endLocal);
                UpdateRandomSparks();
            }
            else
            {
                UpdateSparks(null, null);
                StopAllRandomSparks();
            }

            UpdateAlphaOnMaterial();
            return;
        }

        Vector3 dir = delta / splineLength;
        Vector3 perp = new Vector3(-dir.y, dir.x, 0f);

        desiredInteriorPoints = ComputeBufferedInteriorCount(splineLength, dropInterval);
        int targetCount = desiredInteriorPoints + 2;

        float arcTime = GetArcTime(); // smooth or stepped

        if (isMasterArray)
        {
            // Always compute & store into masterArray
            UpdateMasterArray(targetCount, baseLocal, endLocal, dir, perp, arcTime, splineLength);

            // If canGenerateTetheredArcs is ON, also render this master as a visible tether.
            if (canGenerateTetheredArcs && spline != null && controller != null)
            {
                CopyMasterArrayToOwnSpline();
            }
        }
        else
        {
            // Normal non-master arc: build directly into the spline
            if (spline == null || controller == null)
                return; // safety

            AdjustPointCount(targetCount);

            spline.SetPosition(0, baseLocal);

            for (int i = 1; i <= desiredInteriorPoints; i++)
            {
                float tNorm = (float)i / (desiredInteriorPoints + 1);

                Vector3 p = baseLocal + dir * (tNorm * splineLength);

                float off = ComputeArcOffset(tNorm, arcTime);
                p += perp * off;

                // extra chaos (optional) for main arcs as well
                p = ApplyExtraChaosNoise(p, dir, perp, tNorm, arcTime);

                spline.SetPosition(i, p);
            }

            spline.SetPosition(targetCount - 1, endLocal);

            ApplyMixedTangents(dir, splineLength, arcTime);

            controller.RefreshSpriteShape();
        }

        lastMagnetActive = true;

        bool baseShow2 = !onlyShowWhenStopped || magnetIsStopped;
        bool finalShow2 = baseShow2 && burstVisible;

        if (renderer2D != null)
        {
            if (isMasterArray)
                renderer2D.enabled = canGenerateTetheredArcs && finalShow2;
            else
                renderer2D.enabled = finalShow2;
        }

        if (finalShow2 && (!isMasterArray || canGenerateTetheredArcs))
        {
            UpdateSparks(baseLocal, endLocal);
            UpdateRandomSparks();
        }
        else
        {
            UpdateSparks(null, null);
            StopAllRandomSparks();
        }

        lastSteppedArc = steppedArc;

        // Sync alpha -> MPB
        UpdateAlphaOnMaterial();
    }

    // ---------- CHILD: COPY FROM MASTER ARRAY (legacy + tethered) ----------

    private void UpdateFromMasterArray()
    {
        if (masterArraySource == null)
        {
            if (renderer2D != null)
                renderer2D.enabled = false;

            UpdateSparks(null, null);
            StopAllRandomSparks();
            tetherHasArc = false;
            tetherHasMovingAnchor = false;
            return;
        }

        var arr = masterArraySource.masterArray;
        if (arr == null || arr.Length < 2)
        {
            if (renderer2D != null)
                renderer2D.enabled = false;

            UpdateSparks(null, null);
            StopAllRandomSparks();
            tetherHasArc = false;
            tetherHasMovingAnchor = false;
            return;
        }

        // Mirror state flags from master (for visibility & sparks behavior).
        magnetMoving = masterArraySource.magnetMoving;
        magnetIsStopped = masterArraySource.magnetIsStopped;

        // Make sure we have a spline to draw on.
        if (controller == null || spline == null)
        {
            controller = GetComponent<SpriteShapeController>();
            renderer2D = GetComponent<SpriteShapeRenderer>();
            if (controller != null)
                spline = controller.spline;
        }

        if (controller == null || spline == null)
        {
            Debug.LogWarning($"{name}: isChildToMasterArray is ON but no SpriteShapeController/Spline is available.");
            UpdateSparks(null, null);
            StopAllRandomSparks();
            tetherHasArc = false;
            tetherHasMovingAnchor = false;
            return;
        }

        // If master wants tethered child arcs, use the new system.
        if (masterArraySource.canGenerateTetheredArcs)
        {
            UpdateTetheredChildFromMaster(arr);
            return;
        }

        // Otherwise, legacy behavior (segment copy / sharpChild).
        float now = Time.time;
        bool baseAllow = !onlyShowWhenStopped || magnetIsStopped;
        bool allowShow = baseAllow && burstVisible;

        if (!sharpChild)
        {
            // Continuous copy: pick a fresh random segment each frame.
            if (!allowShow)
            {
                if (renderer2D != null)
                    renderer2D.enabled = false;

                UpdateSparks(null, null);
                StopAllRandomSparks();
                return;
            }

            if (renderer2D != null)
                renderer2D.enabled = true;

            int start, segCount;
            SelectRandomSegmentFromMaster(arr, out start, out segCount);
            CopySegmentToSpline(arr, start, segCount);

            // Random sparks based on the child spline geometry
            UpdateRandomSparks();
        }
        else
        {
            // Sharp: sample once, hold for lifetime, then hide until delay end, then resample.
            if (!childHasCachedSegment || now >= childDelayEndTime)
            {
                int start, segCount;
                SelectRandomSegmentFromMaster(arr, out start, out segCount);
                CacheSegment(arr, start, segCount);

                float life = RandomRangeSafe(childLifetimeRange);
                float delayTotal = RandomRangeSafe(childDelayRange);

                // Ensure total delay is at least the visible lifetime
                if (delayTotal < life)
                    delayTotal = life;

                childLifeEndTime = now + life;        // visible until here
                childDelayEndTime = now + delayTotal; // next resample at this time
            }

            bool visible = childHasCachedSegment &&
                           now < childLifeEndTime &&
                           allowShow;

            if (visible)
            {
                if (renderer2D != null)
                    renderer2D.enabled = true;

                CopyCachedSegmentToSpline();

                // Random sparks only while visible
                UpdateRandomSparks();
            }
            else
            {
                // Hidden during the remainder of the delay window or while magnet moving.
                if (renderer2D != null)
                    renderer2D.enabled = false;

                UpdateSparks(null, null);
                StopAllRandomSparks();
            }
        }
    }

    // ---------- NEW: TETHERED CHILD ARCS + MOVEMENT ----------

    private void UpdateTetheredChildFromMaster(MasterPoint[] arr)
    {
        float now = Time.time;
        bool baseAllow = !onlyShowWhenStopped || magnetIsStopped;
        bool allowShow = baseAllow && burstVisible;

        if (!allowShow)
        {
            tetherHasArc = false;
            tetherHasMovingAnchor = false;

            if (renderer2D != null)
                renderer2D.enabled = false;

            UpdateSparks(null, null);
            StopAllRandomSparks();
            return;
        }

        if (renderer2D == null)
            renderer2D = GetComponent<SpriteShapeRenderer>();

        if (spline == null && controller != null)
            spline = controller.spline;

        if (spline == null)
        {
            UpdateSparks(null, null);
            StopAllRandomSparks();
            tetherHasArc = false;
            tetherHasMovingAnchor = false;
            return;
        }

        // If we have an active tether arc, update or end it.
        if (tetherHasArc)
        {
            // Lifetime ended -> kill arc and schedule next spawn
            if (now >= tetherLifeEndTime)
            {
                tetherHasArc = false;
                tetherHasMovingAnchor = false;
                tetherNextSpawnTime = now + RandomRangeSafe(childDelayRange);

                if (renderer2D != null)
                    renderer2D.enabled = false;

                UpdateSparks(null, null);
                StopAllRandomSparks();
                return;
            }

            // Ensure anchors are still valid (master may have shrunk)
            int masterCount = arr.Length;
            if (tetherAnchorMasterIndexA >= masterCount ||
                tetherAnchorMasterIndexB >= masterCount)
            {
                tetherHasArc = false;
                tetherHasMovingAnchor = false;
                tetherNextSpawnTime = now; // retry soon
                if (renderer2D != null)
                    renderer2D.enabled = false;
                UpdateSparks(null, null);
                StopAllRandomSparks();
                return;
            }

            if (renderer2D != null)
                renderer2D.enabled = true;

            // Move the chosen anchor along the master spline if enabled
            UpdateMovingAnchor(arr, now);

            // Update geometry each frame while alive
            UpdateTetheredChildGeometry(arr, now);
            UpdateRandomSparks();
            return;
        }

        // No active tether: respect delay before spawning a new one
        if (now < tetherNextSpawnTime)
        {
            if (renderer2D != null)
                renderer2D.enabled = false;

            UpdateSparks(null, null);
            StopAllRandomSparks();
            return;
        }

        // Try to spawn a new tethered child arc
        if (!TrySpawnTetheredChild(arr, now))
        {
            tetherNextSpawnTime = now + RandomRangeSafe(childDelayRange);
            if (renderer2D != null)
                renderer2D.enabled = false;
            UpdateSparks(null, null);
            StopAllRandomSparks();
            return;
        }

        // First frame after spawn: build geometry
        if (renderer2D != null)
            renderer2D.enabled = true;

        UpdateTetheredChildGeometry(arr, now);
        UpdateRandomSparks();
    }

    private bool TrySpawnTetheredChild(MasterPoint[] arr, float now)
    {
        int total = arr.Length;
        if (total < 4)
            return false; // need enough master points for spacing

        int maxSpanIndices = total - 1; // 0 .. last

        float minFrac = Mathf.Clamp01(childCopyFractionRange.x);
        float maxFrac = Mathf.Clamp01(childCopyFractionRange.y);
        if (maxFrac < minFrac)
        {
            float tmp = minFrac;
            minFrac = maxFrac;
            maxFrac = tmp;
        }

        int minSpanIndices = 3;
        float frac = Random.Range(minFrac, maxFrac);
        int spanIndices = Mathf.RoundToInt(frac * maxSpanIndices);
        spanIndices = Mathf.Clamp(spanIndices, minSpanIndices, maxSpanIndices);

        int maxStart = maxSpanIndices - spanIndices;
        if (maxStart < 0)
            return false;

        int ia = Random.Range(0, maxStart + 1);
        int ib = ia + spanIndices;

        if (ia > ib)
        {
            int tmp = ia;
            ia = ib;
            ib = tmp;
        }
        if (Mathf.Abs(ia - ib) < minSpanIndices)
            return false;

        int masterInteriorCount = ib - ia - 1;
        int minInterior = Mathf.Max(2, masterInteriorCount);
        int maxInterior = Mathf.Max(minInterior, childMaxExtraPoints);

        int interiorCount = Random.Range(minInterior, maxInterior + 1);
        int totalChildPoints = interiorCount + 2;

        tetherAnchorMasterIndexA = ia;
        tetherAnchorMasterIndexB = ib;
        tetherChildPointCount = totalChildPoints;

        AdjustPointCount(totalChildPoints);

        Vector2 pa = arr[ia].position;
        Vector2 pb = arr[ib].position;
        Vector2 d = (pb - pa).normalized;
        Vector2 perp = new Vector2(-d.y, d.x);
        if (Random.value < 0.5f)
            perp = -perp;

        tetherPerpLocalDir = new Vector3(perp.x, perp.y, 0f);

        float ampMin = Mathf.Max(0f, childArcAmplitudeRange.x);
        float ampMax = Mathf.Max(ampMin, childArcAmplitudeRange.y);
        tetherAmplitude = Random.Range(ampMin, ampMax);

        float life = RandomRangeSafe(childLifetimeRange);
        tetherLifeEndTime = now + life;

        tetherHasArc = true;
        tetherHasMovingAnchor = false;

        if (enableMovement)
        {
            int lastIndex = total - 1;

            bool moveA = Random.value < 0.5f;
            tetherMovingAnchorIsA = moveA;

            int idx = moveA ? tetherAnchorMasterIndexA : tetherAnchorMasterIndexB;

            idx = Mathf.Clamp(idx, 0, lastIndex);
            if (moveA)
                tetherAnchorMasterIndexA = idx;
            else
                tetherAnchorMasterIndexB = idx;

            int distToStart = idx;
            int distToEnd   = lastIndex - idx;

            tetherMovementDirection = (distToEnd < distToStart) ? +1 : -1;

            int nextIndex = idx + tetherMovementDirection;

            if (nextIndex <= 0 || nextIndex >= lastIndex)
            {
                tetherHasMovingAnchor = false;
            }
            else
            {
                tetherHasMovingAnchor = true;
                float step = RandomRangeSafe(childAnchorStepTimeRange);
                tetherNextMoveTime = now + step;
            }
        }

        return true;
    }

    private void UpdateMovingAnchor(MasterPoint[] arr, float now)
    {
        if (!enableMovement || !tetherHasMovingAnchor)
            return;

        int masterCount = arr.Length;
        if (masterCount < 2)
        {
            tetherHasMovingAnchor = false;
            return;
        }

        int lastIndex = masterCount - 1;

        if (now < tetherNextMoveTime)
            return;

        int idx = tetherMovingAnchorIsA ? tetherAnchorMasterIndexA : tetherAnchorMasterIndexB;

        if (idx < 0) idx = 0;
        if (idx > lastIndex) idx = lastIndex;

        int nextIndex = idx + tetherMovementDirection;

        if (nextIndex <= 0 || nextIndex >= lastIndex)
        {
            int clampedNext = Mathf.Clamp(nextIndex, 0, lastIndex);
            if (tetherMovingAnchorIsA)
                tetherAnchorMasterIndexA = clampedNext;
            else
                tetherAnchorMasterIndexB = clampedNext;

            tetherHasMovingAnchor = false;
            return;
        }

        idx = nextIndex;

        if (tetherMovingAnchorIsA)
            tetherAnchorMasterIndexA = idx;
        else
            tetherAnchorMasterIndexB = idx;

        float step = RandomRangeSafe(childAnchorStepTimeRange);
        tetherNextMoveTime = now + step;
    }

    private void UpdateTetheredChildGeometry(MasterPoint[] arr, float now)
    {
        if (spline == null) return;

        int total = tetherChildPointCount;
        AdjustPointCount(total);
        if (total < 2) return;

        int masterCount = arr.Length;
        if (tetherAnchorMasterIndexA >= masterCount ||
            tetherAnchorMasterIndexB >= masterCount)
            return;

        Vector3 aLocal = MasterIndexToLocal(arr, tetherAnchorMasterIndexA);
        Vector3 bLocal = MasterIndexToLocal(arr, tetherAnchorMasterIndexB);

        spline.SetPosition(0, aLocal);
        spline.SetPosition(total - 1, bLocal);

        Vector3 chord = bLocal - aLocal;
        float length = chord.magnitude;
        if (length < 0.0001f)
            length = 0.0001f;
        Vector3 dir = chord / length;

        Vector3 jitterPerp = new Vector3(-dir.y, dir.x, 0f);

        float arcTime = GetArcTime();

        int interiorCount = total - 2;
        for (int i = 1; i <= interiorCount; i++)
        {
            float tNorm = (float)i / (interiorCount + 1);
            Vector3 p = Vector3.Lerp(aLocal, bLocal, tNorm);

            float wave = Mathf.Sin(Mathf.PI * tNorm) * tetherAmplitude;
            p += tetherPerpLocalDir * wave;

            float off = ComputeArcOffset(tNorm, arcTime);
            p += jitterPerp * off;

            // extra chaos noise (optional)
            p = ApplyExtraChaosNoise(p, dir, jitterPerp, tNorm, arcTime);

            spline.SetPosition(i, p);
        }

        ApplyMixedTangents(dir, length, arcTime);

        if (total >= 2)
        {
            float segLen = length / (total - 1);
            float baseHandle = Mathf.Min(dropInterval, segLen) * 0.5f;

            Vector3 p0 = spline.GetPosition(0);
            Vector3 p1 = spline.GetPosition(Mathf.Min(1, total - 1));
            Vector3 plast = spline.GetPosition(total - 1);
            Vector3 pBeforeLast = spline.GetPosition(Mathf.Max(0, total - 2));

            Vector3 tan0 = (p1 - p0).normalized * baseHandle;
            Vector3 tanLast = (pBeforeLast - plast).normalized * baseHandle;

            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(0, Vector3.zero);
            spline.SetRightTangent(0, tan0);

            spline.SetTangentMode(total - 1, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(total - 1, tanLast);
            spline.SetRightTangent(total - 1, Vector3.zero);
        }

        controller.RefreshSpriteShape();

        UpdateSparks(aLocal, bLocal);
    }

    private Vector3 MasterIndexToLocal(MasterPoint[] arr, int i)
    {
        i = Mathf.Clamp(i, 0, arr.Length - 1);
        Vector2 p = arr[i].position;
        return new Vector3(p.x, p.y, 0f);
    }

    private float RandomRangeSafe(Vector2 range)
    {
        float min = Mathf.Max(0f, range.x);
        float max = Mathf.Max(min, range.y);
        return Random.Range(min, max);
    }

    // ---------- legacy child segment helpers ----------

    private void SelectRandomSegmentFromMaster(MasterPoint[] arr, out int startIndex, out int segmentCount)
    {
        int total = arr.Length;

        float minFrac = Mathf.Clamp01(childCopyFractionRange.x);
        float maxFrac = Mathf.Clamp01(childCopyFractionRange.y);
        if (maxFrac < minFrac)
        {
            float tmp = minFrac;
            minFrac = maxFrac;
            maxFrac = tmp;
        }

        float frac = Random.Range(minFrac, maxFrac);
        segmentCount = Mathf.RoundToInt(frac * total);
        segmentCount = Mathf.Clamp(segmentCount, 2, total);

        int maxStart = total - segmentCount;
        startIndex = Random.Range(0, maxStart + 1);
    }

    private void CopySegmentToSpline(MasterPoint[] arr, int startIndex, int segmentCount)
    {
        if (segmentCount < 2) return;

        AdjustPointCount(segmentCount);

        for (int i = 0; i < segmentCount; i++)
        {
            var mp = arr[startIndex + i];
            Vector3 p = new Vector3(mp.position.x, mp.position.y, 0f);

            spline.SetPosition(i, p);
            spline.SetTangentMode(i, mp.tangentMode);
            spline.SetLeftTangent(i, new Vector3(mp.leftTangent.x, mp.leftTangent.y, 0f));
            spline.SetRightTangent(i, new Vector3(mp.rightTangent.x, mp.rightTangent.y, 0f));
        }

        controller.RefreshSpriteShape();

        Vector3 baseLocal = new Vector3(arr[startIndex].position.x, arr[startIndex].position.y, 0f);
        Vector3 endLocal  = new Vector3(arr[startIndex + segmentCount - 1].position.x,
                                        arr[startIndex + segmentCount - 1].position.y, 0f);
        UpdateSparks(baseLocal, endLocal);
    }

    private void CacheSegment(MasterPoint[] arr, int startIndex, int segmentCount)
    {
        if (segmentCount < 2)
        {
            childCachedSegment = null;
            childHasCachedSegment = false;
            return;
        }

        if (childCachedSegment == null || childCachedSegment.Length != segmentCount)
            childCachedSegment = new MasterPoint[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            childCachedSegment[i] = arr[startIndex + i];
        }

        childHasCachedSegment = true;
    }

    private void CopyCachedSegmentToSpline()
    {
        if (!childHasCachedSegment || childCachedSegment == null || childCachedSegment.Length < 2)
        {
            UpdateSparks(null, null);
            return;
        }

        int count = childCachedSegment.Length;
        AdjustPointCount(count);

        for (int i = 0; i < count; i++)
        {
            var mp = childCachedSegment[i];
            Vector3 p = new Vector3(mp.position.x, mp.position.y, 0f);

            spline.SetPosition(i, p);
            spline.SetTangentMode(i, mp.tangentMode);
            spline.SetLeftTangent(i, new Vector3(mp.leftTangent.x, mp.leftTangent.y, 0f));
            spline.SetRightTangent(i, new Vector3(mp.rightTangent.x, mp.rightTangent.y, 0f));
        }

        controller.RefreshSpriteShape();

        Vector3 baseLocal = new Vector3(childCachedSegment[0].position.x, childCachedSegment[0].position.y, 0f);
        Vector3 endLocal  = new Vector3(childCachedSegment[count - 1].position.x,
                                        childCachedSegment[count - 1].position.y, 0f);
        UpdateSparks(baseLocal, endLocal);
    }

    // ---------- stepped timing ----------

    private float GetArcTime()
    {
        if (!steppedArc)
        {
            steppedInitialized = false;
            return Time.time;
        }

        if (!steppedInitialized || !lastSteppedArc)
        {
            steppedInitialized = true;
            SteppedPickNewHoldAndAdvance();
        }

        if (Time.time >= nextStepTime)
        {
            SteppedPickNewHoldAndAdvance();
        }

        return steppedSampleTime;
    }

    private void SteppedPickNewHoldAndAdvance()
    {
        float minH = Mathf.Max(0.0001f, steppedHoldRange.x);
        float maxH = Mathf.Max(minH, steppedHoldRange.y);

        currentHold = Random.Range(minH, maxH);
        nextStepTime = Time.time + currentHold;

        steppedSampleTime = Time.time;
    }

    // ---------- MASTER ARRAY VERSION OF THE SPLINE MATH ----------

    private void EnsureMasterArraySize(int count)
    {
        if (masterArray == null || masterArray.Length != count)
            masterArray = new MasterPoint[count];
    }

    private void UpdateMasterArray(int targetCount,
                                   Vector3 baseLocal,
                                   Vector3 endLocal,
                                   Vector3 dir,
                                   Vector3 perp,
                                   float time,
                                   float length)
    {
        EnsureMasterArraySize(targetCount);

        masterArray[0].position = new Vector2(baseLocal.x, baseLocal.y);

        for (int i = 1; i <= desiredInteriorPoints; i++)
        {
            float tNorm = (float)i / (desiredInteriorPoints + 1);

            Vector3 p = baseLocal + dir * (tNorm * length);

            float off = ComputeArcOffset(tNorm, time);
            p += perp * off;

            masterArray[i].position = new Vector2(p.x, p.y);
        }

        masterArray[targetCount - 1].position = new Vector2(endLocal.x, endLocal.y);

        int count = targetCount;
        if (count < 2) return;

        float segLen = length / (count - 1);
        float baseHandle = Mathf.Min(dropInterval, segLen) * 0.5f;
        int reseedStep = Mathf.FloorToInt(time / Mathf.Max(0.0001f, cornerReseedInterval));

        for (int i = 0; i < count; i++)
        {
            if (i == 0 || i == count - 1)
            {
                Vector3 tan3 = dir * (baseHandle * smoothTangentScale);

                masterArray[i].tangentMode = ShapeTangentMode.Continuous;

                if (i == 0)
                {
                    masterArray[i].leftTangent  = Vector2.zero;
                    masterArray[i].rightTangent = new Vector2(tan3.x, tan3.y);
                }
                else
                {
                    masterArray[i].leftTangent  = new Vector2(-tan3.x, -tan3.y);
                    masterArray[i].rightTangent = Vector2.zero;
                }

                continue;
            }

            float r = Hash01(i, reseedStep, noiseSeed);
            bool isCorner = r < cornerChance;

            if (isCorner)
            {
                masterArray[i].tangentMode = ShapeTangentMode.Linear;
                Vector3 tan3 = dir * (baseHandle * cornerTangentScale);
                masterArray[i].leftTangent  = new Vector2(-tan3.x, -tan3.y);
                masterArray[i].rightTangent = new Vector2(tan3.x, tan3.y);
            }
            else
            {
                masterArray[i].tangentMode = ShapeTangentMode.Continuous;
                Vector3 tan3 = dir * (baseHandle * smoothTangentScale);
                masterArray[i].leftTangent  = new Vector2(-tan3.x, -tan3.y);
                masterArray[i].rightTangent = new Vector2(tan3.x, tan3.y);
            }
        }
    }

    /// <summary>
    /// When isMasterArray && canGenerateTetheredArcs, use this to render the
    /// master's own arc using its masterArray data.
    /// </summary>
    private void CopyMasterArrayToOwnSpline()
    {
        if (masterArray == null || masterArray.Length < 2 || spline == null || controller == null)
            return;

        int count = masterArray.Length;
        AdjustPointCount(count);

        for (int i = 0; i < count; i++)
        {
            var mp = masterArray[i];
            Vector3 p = new Vector3(mp.position.x, mp.position.y, 0f);

            spline.SetPosition(i, p);
            spline.SetTangentMode(i, mp.tangentMode);
            spline.SetLeftTangent(i, new Vector3(mp.leftTangent.x, mp.leftTangent.y, 0f));
            spline.SetRightTangent(i, new Vector3(mp.rightTangent.x, mp.rightTangent.y, 0f));
        }

        controller.RefreshSpriteShape();
    }

    // ---------- Sparks at endpoints ----------

    private void UpdateSparks(Vector3? baseLocalOpt, Vector3? endLocalOpt)
    {
        if (!baseLocalOpt.HasValue || !endLocalOpt.HasValue)
        {
            StopPS(basePS);
            StopPS(magnetPS);
            return;
        }

        Vector3 baseLocal = baseLocalOpt.Value;
        Vector3 endLocal  = endLocalOpt.Value;

        Vector3 baseWorld = transform.TransformPoint(baseLocal);
        Vector3 endWorld  = transform.TransformPoint(endLocal);

        if (baseSparks != null)
            baseSparks.position = baseWorld;

        if (magnetSparks != null)
            magnetSparks.position = endWorld;

        Vector3 dirBaseToMag = (endWorld - baseWorld);
        if (dirBaseToMag.sqrMagnitude > 0.000001f)
        {
            Vector3 n = dirBaseToMag.normalized;
            Quaternion flipX = Quaternion.Euler(180f, 0f, 0f);

            if (baseSparks != null)
                baseSparks.rotation = Quaternion.LookRotation(-n, Vector3.forward) * flipX;

            if (magnetSparks != null)
                magnetSparks.rotation = Quaternion.LookRotation(n, Vector3.forward) * flipX;
        }

        if (magnetIsStopped)
        {
            PlayPS(basePS);
            PlayPS(magnetPS);
        }
        else
        {
            StopPS(basePS);
            StopPS(magnetPS);
        }
    }

    private void PlayPS(ParticleSystem ps)
    {
        if (ps == null) return;
        if (!ps.isPlaying) ps.Play(true);
    }

    private void StopPS(ParticleSystem ps)
    {
        if (ps == null) return;
        if (ps.isPlaying) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    // ---------- Random Sparks Along Spline ----------

    private void InitRandomSparksPool()
    {
        if (!SparksEnabled)
            return;

        sparksPoolSize = Mathf.Max(1, maxConcurrentSparks);
        sparksPool = new ParticleSystem[sparksPoolSize];
        sparkEndTimes = new float[sparksPoolSize];

        sparksPool[0] = sparks;
        sparkEndTimes[0] = 0f;

        if (sparks.transform.parent != transform)
        {
            sparks.transform.SetParent(transform, true);
        }

        sparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        for (int i = 1; i < sparksPoolSize; i++)
        {
            ParticleSystem clone = Instantiate(sparks, transform);
            clone.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sparksPool[i] = clone;
            sparkEndTimes[i] = 0f;
        }

        nextSparksTime = Time.time + 0.1f;
    }

    private void StopAllRandomSparks()
    {
        if (sparksPool == null) return;

        for (int i = 0; i < sparksPool.Length; i++)
        {
            StopPS(sparksPool[i]);
            sparkEndTimes[i] = 0f;
        }
    }

    private void UpdateRandomSparks()
    {
        if (!SparksEnabled || spline == null || sparksPool == null)
            return;

        int pointCount = spline.GetPointCount();
        if (pointCount <= 2)
            return;

        float now = Time.time;

        for (int i = 0; i < sparksPoolSize; i++)
        {
            var ps = sparksPool[i];
            if (ps == null) continue;

            if (ps.isPlaying && now > sparkEndTimes[i])
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        if (now < nextSparksTime)
            return;

        float minInt = Mathf.Max(0.0001f, sparksIntervalRange.x);
        float maxInt = Mathf.Max(minInt, sparksIntervalRange.y);
        float interval = Random.Range(minInt, maxInt);
        nextSparksTime = now + interval;

        int interiorCount = pointCount - 2;
        if (interiorCount <= 0)
            return;

        int desiredCount = Mathf.RoundToInt(sparksDensity * interiorCount);
        if (desiredCount <= 0)
            return;

        desiredCount = Mathf.Min(desiredCount, sparksPoolSize);

        List<int> chosenIndices = new List<int>(desiredCount);
        int safety = 0;
        while (chosenIndices.Count < desiredCount && safety < 1000)
        {
            int idx = Random.Range(1, pointCount - 1);
            if (!chosenIndices.Contains(idx))
                chosenIndices.Add(idx);
            safety++;
        }

        float minDur = Mathf.Max(0.0001f, sparksDurationRange.x);
        float maxDur = Mathf.Max(minDur, sparksDurationRange.y);

        foreach (int idx in chosenIndices)
        {
            int poolIndex = GetFreeSparkFromPool();
            if (poolIndex < 0)
                break;

            var ps = sparksPool[poolIndex];
            if (ps == null) continue;

            Vector3 localP = spline.GetPosition(idx);
            Vector3 worldP = transform.TransformPoint(localP);

            ps.transform.position = worldP;

            float dur = Random.Range(minDur, maxDur);
            sparkEndTimes[poolIndex] = now + dur;

            if (!ps.isPlaying)
                ps.Play(true);
        }
    }

    private int GetFreeSparkFromPool()
    {
        if (sparksPool == null) return -1;

        float now = Time.time;

        for (int i = 0; i < sparksPoolSize; i++)
        {
            var ps = sparksPool[i];
            if (ps == null) continue;

            if (!ps.isPlaying || now > sparkEndTimes[i])
                return i;
        }

        return -1;
    }

    // ---------- Arc noise ----------

    private float ComputeArcOffset(float t, float time)
    {
        float amp = arcAmplitude;
        float freq = arcFrequency;
        float sum = 0f;

        float seedX = noiseSeed * 17.13f;
        float seedY = noiseSeed * 3.71f;

        float speedMul = steppedArc ? 1f : arcSpeed;

        for (int o = 0; o < arcOctaves; o++)
        {
            float n = Mathf.PerlinNoise(
                seedX + t * freq + time * speedMul,
                seedY + o * 10.0f
            );

            n = (n - 0.5f) * 2f;
            sum += n * amp;

            amp *= arcRoughness;
            freq *= 2f;
        }

        return sum;
    }

    private Vector3 ApplyExtraChaosNoise(Vector3 position, Vector3 dir, Vector3 perp, float tNorm, float time)
    {
        if (!enableExtraChaos)
            return position;

        float chaosTime = time * extraChaosSpeed;
        float seedBase = noiseSeed * 0.123f;
        float u = tNorm * extraChaosFrequency + seedBase;

        float nPerp = Mathf.PerlinNoise(u, chaosTime);
        float nPar  = Mathf.PerlinNoise(u + 37.21f, chaosTime + 11.73f);

        float jPerp = (nPerp - 0.5f) * 2f * extraChaosPerpJitter;
        float jPar  = (nPar  - 0.5f) * 2f * extraChaosParallelJitter;

        position += perp * jPerp + dir * jPar;
        return position;
    }

    // ---------- Tangents on real spline ----------

    private void ApplyMixedTangents(Vector3 dir, float length, float time)
    {
        if (spline == null) return;

        int count = spline.GetPointCount();
        if (count < 2) return;

        float segLen = length / (count - 1);
        float baseHandle = Mathf.Min(dropInterval, segLen) * 0.5f;

        int reseedStep = Mathf.FloorToInt(time / Mathf.Max(0.0001f, cornerReseedInterval));

        for (int i = 0; i < count; i++)
        {
            if (i == 0 || i == count - 1)
            {
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                Vector3 tan = dir * (baseHandle * smoothTangentScale);

                if (i == 0)
                {
                    spline.SetLeftTangent(i, Vector3.zero);
                    spline.SetRightTangent(i, tan);
                }
                else
                {
                    spline.SetLeftTangent(i, -tan);
                    spline.SetRightTangent(i, Vector3.zero);
                }
                continue;
            }

            float r = Hash01(i, reseedStep, noiseSeed);
            bool isCorner = r < cornerChance;

            if (isCorner)
            {
                spline.SetTangentMode(i, ShapeTangentMode.Linear);
                Vector3 tan = dir * (baseHandle * cornerTangentScale);
                spline.SetLeftTangent(i, -tan);
                spline.SetRightTangent(i, tan);
            }
            else
            {
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                Vector3 tan = dir * (baseHandle * smoothTangentScale);
                spline.SetLeftTangent(i, -tan);
                spline.SetRightTangent(i, tan);
            }
        }
    }

    private float Hash01(int i, int step, int seed)
    {
        uint h = (uint)(i * 374761393 + step * 668265263 + seed * 2246822519u);
        h = (h ^ (h >> 13)) * 1274126177u;
        h ^= (h >> 16);
        return (h & 0xFFFFFF) / (float)0x1000000;
    }

    // ----- core spline count logic -----

    private int ComputeBufferedInteriorCount(float length, float x)
    {
        if (x <= 0.0001f) return 0;

        int baseCount = Mathf.FloorToInt(length / x);
        float remainder = length % x;

        if (remainder > x * 0.5f)
            baseCount += 1;

        return Mathf.Max(0, baseCount);
    }

    // ----- debounced movement / stop detection -----

    private void DetectMovingFlag()
    {
        if (magnetTransform == null)
        {
            magnetMoving = false;
            magnetIsStopped = false;
            hasLastPos = false;
            stoppedTimer = 0f;
            return;
        }

        Vector3 current = magnetTransform.position;

        if (!hasLastPos)
        {
            lastMagnetWorldPos = current;
            hasLastPos = true;

            magnetMoving = true;
            magnetIsStopped = false;
            stoppedTimer = 0f;
            return;
        }

        float movedDist = Vector3.Distance(current, lastMagnetWorldPos);
        lastMagnetWorldPos = current;

        bool aboveThreshold = movedDist > moveThreshold;

        if (aboveThreshold)
        {
            stoppedTimer = 0f;
            magnetMoving = true;
            magnetIsStopped = false;
        }
        else
        {
            stoppedTimer += Time.deltaTime;

            if (stoppedTimer >= stoppedMinTime)
            {
                magnetMoving = false;
                magnetIsStopped = true;
            }
            else
            {
                magnetMoving = true;
                magnetIsStopped = false;
            }
        }
    }

    private void AdjustPointCount(int targetCount)
    {
        if (spline == null) return;

        int current = spline.GetPointCount();

        while (current < targetCount)
        {
            spline.InsertPointAt(current, spline.GetPosition(Mathf.Max(0, current - 1)));
            current++;
        }

        while (current > targetCount)
        {
            spline.RemovePointAt(current - 1);
            current--;
        }
    }

    private void InitTwoPointLine()
    {
        if (spline == null || controller == null) return;

        for (int i = spline.GetPointCount() - 1; i >= 0; i--)
            spline.RemovePointAt(i);

        spline.InsertPointAt(0, Vector3.zero);
        spline.InsertPointAt(1, Vector3.right);

        spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        spline.SetTangentMode(1, ShapeTangentMode.Continuous);
        spline.SetLeftTangent(0, Vector3.zero);
        spline.SetRightTangent(0, Vector3.right * dropInterval * 0.25f);
        spline.SetLeftTangent(1, Vector3.left * dropInterval * 0.25f);
        spline.SetRightTangent(1, Vector3.zero);

        controller.RefreshSpriteShape();

        desiredInteriorPoints = 0;
        magnetIsStopped = false;
        magnetMoving = false;
        stoppedTimer = 0f;
        hasLastPos = false;
    }

    // ----- MPB / Material init & update -----

    public void InitializeMaterialForArc(Material baseMaterial)
    {
        if (renderer2D == null)
            renderer2D = GetComponent<SpriteShapeRenderer>();

        if (renderer2D == null)
        {
            Debug.LogWarning($"{name}: InitializeMaterialForArc called but no SpriteShapeRenderer found.");
            return;
        }

        if (baseMaterial != null)
            renderer2D.sharedMaterial = baseMaterial;

        if (arcMPB == null)
            arcMPB = new MaterialPropertyBlock();

        renderer2D.GetPropertyBlock(arcMPB);
        arcMPB.SetFloat("_alpha", arcAlpha);
        renderer2D.SetPropertyBlock(arcMPB);

        hasArcMPB = true;
        lastArcAlpha = arcAlpha;
    }

    private void UpdateAlphaOnMaterial()
    {
        if (!hasArcMPB || renderer2D == null)
            return;

        if (Mathf.Approximately(lastArcAlpha, arcAlpha))
            return;

        renderer2D.GetPropertyBlock(arcMPB);
        arcMPB.SetFloat("_alpha", arcAlpha);
        renderer2D.SetPropertyBlock(arcMPB);

        lastArcAlpha = arcAlpha;
    }

    // ----- burst visibility control (called by manager) -----

    public void SetBurstVisible(bool visible)
    {
        burstVisible = visible;
    }

    // ----- magnet / spawner lookup -----

    private void TryCacheMagnet()
    {
        GameObject magnetObj = GameObject.Find(magnetCloneName);
        if (magnetObj == null) return;

        int magnetLayer = LayerMask.NameToLayer(magnetLayerName);
        if (magnetLayer != -1 && magnetObj.layer != magnetLayer) return;

        magnetTransform = magnetObj.transform;
    }

    private MagnetSpawnerScript FindSpawnerOnSameLevel()
    {
        var s = GetComponent<MagnetSpawnerScript>();
        if (s != null) return s;

        Transform parent = transform.parent;
        if (parent != null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                s = child.GetComponent<MagnetSpawnerScript>();
                if (s != null) return s;
            }
        }

        return Object.FindFirstObjectByType<MagnetSpawnerScript>();
    }
}
