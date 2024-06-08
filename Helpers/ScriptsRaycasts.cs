using UnityEngine;

namespace Scripts
{
    class Raycasts
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
            {
                if (hit.collider != null)
                {
                    return true;
                }
            }
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

        /// <summary> Performs raycasts in parallel. Returns true if something is hit. </summary> // TODO: possible parabola base
        // public static bool RaycastParallel(Vector2 leftBase, Vector2 rightBase, int rays, float distance, LayerMask layerMask, out string hitTag, out float hitDistance, float padding = 0.05f, bool isDebug = false)
        // {
        //     float baseAngle = F_Angles.PositionsToAngle(leftBase, rightBase);
        //     float rayAngle = baseAngle + Mathf.PI / 2;
        //     Vector2 rayDirection = F_Angles.AngleToVector2(rayAngle);
        //     Vector2 paddedLeftBase = (1 - padding) * leftBase + padding * rightBase;
        //     Vector2 paddedRightBase = (1 - padding) * rightBase + padding * leftBase;
        //     Vector2[] startPos = new Vector2[rays];
        //     RaycastHit2D[] hits = new RaycastHit2D[rays];

        //     for (int i = 0; i < rays; i++)
        //     {
        //         startPos[i] = Vector2.Lerp(paddedLeftBase, paddedRightBase, (float)i / (rays - 1));
        //         hits[i] = Physics2D.Raycast(Vector2.Lerp(paddedLeftBase, paddedRightBase, (float)i / (rays - 1)), rayDirection, distance, layerMask);
        //     }

        //     if (isDebug)
        //     {
        //         Debug.DrawLine(paddedLeftBase, paddedRightBase, Color.yellow, 0, false); // Base

        //         for (int i = 0; i < rays; i++)
        //         {
        //             if (hits[i].transform != null) Debug.DrawLine(startPos[i], startPos[i] + rayDirection * hits[i].distance, Color.red, 0, false);
        //             else Debug.DrawLine(startPos[i], startPos[i] + rayDirection * distance, Color.green, 0, false);
        //         }
        //     }

        //     hitTag = null;
        //     hitDistance = 9999;
        //     foreach (RaycastHit2D hit in hits) // Check for nearest hit from all hits
        //         if (hit.transform != null & hit.distance < hitDistance)
        //         {
        //             hitTag = hit.transform.tag;
        //             hitDistance = hit.distance;
        //         }
        //     if (hitTag == null) return false;
        //     else return true;
        // }


        // public static bool RaycastRadial(Vector2 center, int rayCount, float distance, LayerMask layerMask, out RaycastHit2D outHit, bool isDebug = false)
        // {
        //     // Returns normalized Vector3[] spread from origin (start from right) 
        //     Vector2[] directions = new Vector2[rayCount];
        //     RaycastHit2D[] hits = new RaycastHit2D[rayCount];
        //     // RaycastHit[] hits = new RaycastHit[rayCount];
        //     for (int i = 0; i < rayCount; i++)
        //     {
        //         float angle = 2 * Mathf.PI * i / rayCount;
        //         directions[i] = F_Angles.AngleToVector2(angle);
        //         // Physics.Raycast(center, directions[i], out hits[i], distance, layerMask);
        //         hits[i] = Physics2D.Raycast(center, directions[i], distance, layerMask);
        //     }

        //     if (isDebug)
        //     {
        //         for (int i = 0; i < rayCount; i++)
        //         {
        //             if (hits[i].transform != null) Debug.DrawLine(center, center + directions[i] * hits[i].distance, Color.red, 0, false);
        //             else Debug.DrawLine(center, center + directions[i] * distance, Color.green, 0, false);
        //         }
        //     }

        //     // Check for nearest hit
        //     float hitDistance = 9999;
        //     outHit = hits[0];
        //     foreach (RaycastHit2D hit in hits) 
        //         if (hit.transform != null & hit.distance < hitDistance)
        //             outHit = hit;
        //     if (outHit.transform == null) return false;
        //     else return true;
        // }

    }
}

