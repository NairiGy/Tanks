using System.Collections.Generic;
using Tanks.Core;
using Tanks.Match;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Tanks.AI
{
    // Static facts about the current map (bases, bushes, corners), computed once and shared by every bot.
    public sealed class AIMapInfo
    {
        private static AIMapInfo instance;

        private readonly int sceneHandle;
        private readonly Dictionary<Team, Transform> bases = new Dictionary<Team, Transform>();
        private readonly Collider[] bushAreas;
        private readonly Dictionary<(float inset, float sampleDistance), IReadOnlyList<Vector3>> cornerCache =
            new Dictionary<(float inset, float sampleDistance), IReadOnlyList<Vector3>>();

        public IReadOnlyList<Collider> BushAreas => bushAreas;

        public static AIMapInfo Get()
        {
            int handle = SceneManager.GetActiveScene().handle;

            if (instance == null || instance.sceneHandle != handle)
            {
                instance = new AIMapInfo(handle);
            }

            return instance;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            instance = null;
        }

        private AIMapInfo(int sceneHandle)
        {
            this.sceneHandle = sceneHandle;

            foreach (TeamBase teamBase in Object.FindObjectsByType<TeamBase>(FindObjectsSortMode.None))
            {
                bases[teamBase.Team] = teamBase.transform;
            }

            Bush[] bushes = Object.FindObjectsByType<Bush>(FindObjectsSortMode.None);
            bushAreas = new Collider[bushes.Length];

            for (int i = 0; i < bushes.Length; i++)
            {
                bushAreas[i] = bushes[i].GetComponent<Collider>();
            }
        }

        public Transform GetBase(Team team)
        {
            return bases.TryGetValue(team, out Transform teamBase) ? teamBase : null;
        }

        public IReadOnlyList<Vector3> GetCornerPoints(float inset, float sampleDistance)
        {
            var key = (inset, sampleDistance);

            if (!cornerCache.TryGetValue(key, out IReadOnlyList<Vector3> corners))
            {
                corners = BuildCornerPoints(inset, sampleDistance);
                cornerCache[key] = corners;
            }

            return corners;
        }

        private static List<Vector3> BuildCornerPoints(float inset, float sampleDistance)
        {
            var points = new List<Vector3>();

            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            if (triangulation.vertices.Length == 0) return points;

            var bounds = new Bounds(triangulation.vertices[0], Vector3.zero);

            foreach (Vector3 vertex in triangulation.vertices)
            {
                bounds.Encapsulate(vertex);
            }

            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            float y = bounds.center.y;

            Vector3[] rawCorners =
            {
                new Vector3(min.x + inset, y, min.z + inset),
                new Vector3(min.x + inset, y, max.z - inset),
                new Vector3(max.x - inset, y, min.z + inset),
                new Vector3(max.x - inset, y, max.z - inset),
            };

            foreach (Vector3 raw in rawCorners)
            {
                if (NavMesh.SamplePosition(raw, out NavMeshHit hit, sampleDistance, NavMesh.AllAreas))
                {
                    points.Add(hit.position);
                }
            }

            return points;
        }
    }
}
