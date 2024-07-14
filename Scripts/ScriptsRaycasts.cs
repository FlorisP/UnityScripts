using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    class Raycasts2D
    {
        public static RaycastHit2D[] ParallelRaycasts2D(Vector2 left, Vector2 right, int numberOfRays, float distance, LayerMask layerMask = default)
        {
            RaycastHit2D[] hits = new RaycastHit2D[numberOfRays];

            Vector2 rayDirection = right - left;
            float raySpacing = rayDirection.magnitude / (numberOfRays - 1);

            // Calculate the perpendicular direction for raycasts
            Vector2 perpendicularDirection = new Vector2(rayDirection.y, -rayDirection.x).normalized;

            for (int i = 0; i < numberOfRays; i++)
            {
                Vector2 rayOrigin = left + (rayDirection.normalized * raySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, perpendicularDirection, distance, layerMask);
                hits[i] = hit;

                if (hit.collider != null)
                {
                    // Draw a red line for a hit
                    Debug.DrawLine(rayOrigin, hit.point, Color.red);
                }
                else
                {
                    // Draw a green line for no collision
                    Debug.DrawLine(rayOrigin, rayOrigin + perpendicularDirection * distance, Color.green);
                }
            }
            return hits;
        }

        public static RaycastHit2D[] RadialRaycast2D(Vector2 center, int numberOfRays, float distance, LayerMask layerMask = default)
        {
            RaycastHit2D[] hits = new RaycastHit2D[numberOfRays];

            for (int i = 0; i < numberOfRays; i++)
            {
                float angle = 2 * Mathf.PI * i / numberOfRays;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                RaycastHit2D hit = Physics2D.Raycast(center, direction, distance, layerMask);
                hits[i] = hit;

                // Draw a red line for a hit
                if (hit.collider != null)
                {
                    Debug.DrawLine(center, hit.point, Color.red);
                }
                else
                {
                    // Draw a green line for no collision
                    Debug.DrawLine(center, center + direction * distance, Color.green);
                }
            }
            
            return hits;
        }
        
        public static bool AnyHits(RaycastHit2D[] hits)
        {
            foreach (RaycastHit2D hit in hits)
                if (hit.collider != null)
                    return true;
            return false;
        }

        public static RaycastHit2D ClosestHit(RaycastHit2D[] hits)
        {
            RaycastHit2D closestHit = default;
            float closestDistance = Mathf.Infinity;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.distance < closestDistance)
                {
                    closestHit = hit;
                    closestDistance = hit.distance;
                }
            }
            return closestHit;
        }
    }

    class Raycasts
    {
        public static RaycastHit[] SphericalRaycasts(Vector3 center, float distance, LayerMask layerMask = default, int[] pattern = null)
        {
            if (layerMask == default)
                layerMask = -1; // all
            if (pattern == null)
                pattern = new int[] { 1, 8, 8, 8, 1 }; // Default ray pattern

            List<RaycastHit> hits = new List<RaycastHit>();

            int layerCount = pattern.Length;
            float layerHeight = 2f / (layerCount - 1); // Height between layers, ranging from -1 to 1

            for (int layer = 0; layer < layerCount; layer++)
            {
                float y = 1f - (layer * layerHeight);
                int raysInLayer = pattern[layer];

                if (raysInLayer == 1)
                {
                    // Single ray for top or bottom
                    Vector3 direction = new Vector3(0, y, 0).normalized;
                    hits.Add(Physics.Raycast(center, direction, out RaycastHit hit, distance, layerMask) ? hit : new RaycastHit());
                    Debug.DrawRay(center, direction * distance, hit.collider != null ? Color.red : Color.cyan);
                }
                else
                {
                    // Multiple rays in a ring
                    float layerRadius = Mathf.Sqrt(1 - y * y);
                    for (int i = 0; i < raysInLayer; i++)
                    {
                        float angle = i * (2 * Mathf.PI / raysInLayer);
                        Vector3 direction = new Vector3(
                            Mathf.Cos(angle) * layerRadius,
                            y,
                            Mathf.Sin(angle) * layerRadius
                        ).normalized;

                        hits.Add(Physics.Raycast(center, direction, out RaycastHit hit, distance, layerMask) ? hit : new RaycastHit());
                        Debug.DrawRay(center, direction * distance, hit.collider != null ? Color.red : Color.green);
                    }
                }
            }
            return hits.ToArray();
        }
        
        public static RaycastHit[] SphericalRaycastsEvenly(Vector3 center, int numRays, float distance, LayerMask layerMask = default)
        {
            RaycastHit[] hits = new RaycastHit[numRays];
            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;

            for (int i = 0; i < numRays; i++)
            {
                float t = (float)i / numRays;
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = angleIncrement * i;

                Vector3 direction = new Vector3(
                    Mathf.Sin(inclination) * Mathf.Cos(azimuth),
                    Mathf.Sin(inclination) * Mathf.Sin(azimuth),
                    Mathf.Cos(inclination)
                );

                RaycastHit hit;
                if (Physics.Raycast(center, direction, out hit, distance, layerMask))
                {
                    hits[i] = hit;
                    Debug.DrawLine(center, hit.point, Color.red);
                }
                else
                {
                    Debug.DrawRay(center, direction * distance, Color.green);
                }
            }

            return hits;
        }


        public static bool AnyHits(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
                if (hit.collider != null)
                    return true;
            return false;
        }

        public static RaycastHit ClosestHit(RaycastHit[] hits)
        {
            RaycastHit closestHit = default;
            float closestDistance = Mathf.Infinity;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null && hit.distance < closestDistance)
                {
                    closestHit = hit;
                    closestDistance = hit.distance;
                }
            }
            return closestHit;
        }
    }
}

