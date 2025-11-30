using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for all MagnetVectorRenderer arcs under this object.
/// - Initializes materials / alpha per arc.
/// - Groups children into "arc groups" (each direct child of this GameObject).
/// - Optional short-burst mode: only a subset of groups are visible for a
///   random lifetime, then a different subset is chosen.
/// - While the magnet is moving, burst mode is suspended and all arcs are
///   allowed to render; once the magnet stops, burst mode kicks in.
/// </summary>
public class MagnetEffectMaterialManager : MonoBehaviour
{
    [Header("Material Setup")]
    [Tooltip("Base electric material (Lighting ShaderGraph) used for all arcs.")]
    public Material electricBaseMaterial;

    [Tooltip("Alpha for main (non-child) arcs. This is the *base* value before distance fading, etc.")]
    [Range(0f, 1f)]
    public float mainArcAlpha = 1f;

    [Tooltip("Alpha for child arcs (isChildToMasterArray = true).")]
    [Range(0f, 1f)]
    public float childArcAlpha = 0.5f;

    [Header("Short Burst Mode")]
    [Tooltip("If ON, only a random subset of arc groups are visible for short bursts.")]
    public bool shortBurstMode = false;

    [Tooltip("Maximum number of arc groups that may be visible at once (0 = none).")]
    [Min(0)]
    public int maxVisibleArcs = 2;

    [Tooltip("Range of lifetime (seconds) for each burst configuration.")]
    public Vector2 arcLifetimeRange = new Vector2(0.08f, 0.2f);

    // ----- internal arc-group representation -----

    private class ArcGroup
    {
        public string name;
        public List<MagnetVectorRenderer> arcs = new List<MagnetVectorRenderer>();
    }

    private List<ArcGroup> arcGroups = new List<ArcGroup>();

    // cached flat list so we can easily query global magnet state
    private MagnetVectorRenderer[] allArcs;

    // burst timing
    private float currentBurstEndTime = 0f;

    // state
    private bool lastShortBurstMode = false;
    private bool magnetWasStopped = false;

    void Awake()
    {
        // 1) Initialize materials / alpha for all arcs
        allArcs = GetComponentsInChildren<MagnetVectorRenderer>(true);
        foreach (var arc in allArcs)
        {
            if (arc == null) continue;

            // Initialize the per-renderer MPB and set starting alpha
            if (electricBaseMaterial != null)
                arc.InitializeMaterialForArc(electricBaseMaterial);

            arc.arcAlpha = arc.isChildToMasterArray ? childArcAlpha : mainArcAlpha;
        }

        // 2) Build arc groups from direct children of this manager object
        BuildArcGroups();

        // Start with everything visible in case shortBurstMode starts off
        SetAllGroupsVisible(true);
        lastShortBurstMode = shortBurstMode;
        magnetWasStopped = false;
        currentBurstEndTime = 0f;
    }

    void Update()
    {
        // If short burst mode just got disabled, restore normal behavior
        if (!shortBurstMode)
        {
            if (lastShortBurstMode)
            {
                SetAllGroupsVisible(true);
                currentBurstEndTime = 0f;
            }

            lastShortBurstMode = false;
            magnetWasStopped = GetGlobalMagnetStopped();
            return;
        }

        lastShortBurstMode = true;

        if (arcGroups == null || arcGroups.Count == 0 || allArcs == null || allArcs.Length == 0)
            return;

        bool magnetStopped = GetGlobalMagnetStopped();

        // While the magnet is moving -> suspend burst mode and show everything
        if (!magnetStopped)
        {
            // show all arcs; internal onlyShowWhenStopped flags still apply
            SetAllGroupsVisible(true);

            // reset burst timing so we start fresh when it stops again
            currentBurstEndTime = 0f;
            magnetWasStopped = false;
            return;
        }

        // Magnet is stopped here.

        // First frame after stopping -> immediately start a burst
        float now = Time.time;
        if (!magnetWasStopped || currentBurstEndTime <= 0f)
        {
            RunNewBurst(now);
            magnetWasStopped = true;
            return;
        }

        // Ongoing burst mode while magnet remains stopped
        if (now >= currentBurstEndTime)
        {
            RunNewBurst(now);
        }
    }

    // ---------- grouping / burst helpers ----------

    private void BuildArcGroups()
    {
        arcGroups.Clear();

        // Each *direct child* of this GameObject becomes one "arc group".
        foreach (Transform child in transform)
        {
            if (child == null) continue;

            var arcs = child.GetComponentsInChildren<MagnetVectorRenderer>(true);
            if (arcs == null || arcs.Length == 0)
                continue;

            var group = new ArcGroup
            {
                name = child.name,
                arcs = new List<MagnetVectorRenderer>(arcs)
            };

            arcGroups.Add(group);
        }
    }

    private void SetAllGroupsVisible(bool visible)
    {
        if (arcGroups == null) return;

        foreach (var group in arcGroups)
        {
            if (group == null) continue;

            foreach (var arc in group.arcs)
            {
                if (arc == null) continue;
                arc.SetBurstVisible(visible);
            }
        }
    }

    private void RunNewBurst(float now)
    {
        // Hide everything first
        SetAllGroupsVisible(false);

        int groupCount = arcGroups.Count;
        int desiredVisible = Mathf.Clamp(maxVisibleArcs, 0, groupCount);

        // Choose a new lifetime for this burst
        float life = RandomRangeSafe(arcLifetimeRange);
        currentBurstEndTime = now + life;

        if (desiredVisible <= 0)
            return; // all arcs stay hidden this burst

        // Build an index list [0..groupCount-1]
        List<int> indices = new List<int>(groupCount);
        for (int i = 0; i < groupCount; i++)
            indices.Add(i);

        // Partial Fisher–Yates shuffle to pick "desiredVisible" unique groups
        for (int i = 0; i < desiredVisible; i++)
        {
            int swapIndex = Random.Range(i, groupCount);
            int tmp = indices[i];
            indices[i] = indices[swapIndex];
            indices[swapIndex] = tmp;
        }

        // Activate the chosen groups
        for (int k = 0; k < desiredVisible; k++)
        {
            int idx = indices[k];
            if (idx < 0 || idx >= groupCount) continue;

            ArcGroup g = arcGroups[idx];
            if (g == null) continue;

            foreach (var arc in g.arcs)
            {
                if (arc == null) continue;
                arc.SetBurstVisible(true);
            }
        }
    }

    private float RandomRangeSafe(Vector2 range)
    {
        float min = Mathf.Max(0f, range.x);
        float max = Mathf.Max(min, range.y);
        return Random.Range(min, max);
    }

    /// <summary>
    /// Reads a "global" magnet stopped state from any arc.
    /// All arcs share the same magnet / spawner, so this is safe.
    /// </summary>
    private bool GetGlobalMagnetStopped()
    {
        if (allArcs == null || allArcs.Length == 0)
            return false;

        foreach (var arc in allArcs)
        {
            if (arc == null) continue;
            return arc.magnetIsStopped;
        }

        return false;
    }
}
