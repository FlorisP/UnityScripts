using System;
using UnityEngine;

namespace Scripts
{
    //https://www.youtube.com/watch?v=KPoeNZZ6H4s t3ssel8r
    // Y = location, X = target
    public class SecondOrderDynamics
    {
        float y, yd;
        float k1, k2, k3; // dynamics constants
        float previous_target;

        // f (natural frequency) = speed to which system reacts (not shape)
        // z = damping (undamped = 0, underdamped = 0-1, critical damping = 1, overdampled > 1)
        // r (initial response) = (0 takes time, 1 immedial reaction, > 1 overshoot, < 0 anticipate) 2 typical
        public SecondOrderDynamics(float f, float z, float r, float x0)
        {
            UpdateConstants(f, z, r);

            previous_target = x0;
            y = x0;
            yd = 0;
        }

        void UpdateConstants(float f, float z, float r)
        {
            f = Mathf.Max(f, 0.01f);

            k1 = z / (Mathf.PI * f);
            k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
            k3 = r * z / (2 * Mathf.PI * f);
        }

        public float Update(float dt, float target, float? xd = null)
        {
            if(dt <= 0)
                return y;

            // Estimate target change
            if (xd == null){ 
                xd = (target - previous_target) / dt;
                previous_target = target;
            }

            // Calculate position and new speed
            float k2_stable = Math.Max(Math.Max(k2, dt*dt/2 + dt*k1/2), dt*k1); // clamp k2 to guarantee stability without jitter
            y += dt * yd;
            yd += dt * (target - y + k3*(float)xd - k1*yd) / k2_stable; // integrate velocity by acceleration
            
            return y;
        }

        public void UpdateFZR(Vector3 fzr)
            { UpdateConstants(fzr.x, fzr.y, fzr.z); }

    }

    public class SecondOrderDynamics3D
    {
        Vector3 y, yd;
        float k1, k2, k3; // dynamics constants
        Vector3 xp;
        
        public SecondOrderDynamics3D(float f, float z, float r, Vector3 x0)
        {
            UpdateConstants(f, z, r);

            xp = x0;
            y = x0;
            yd = Vector3.zero;
        }

        void UpdateConstants(float f, float z, float r)
        {
            f = Mathf.Max(f, 0.01f);

            k1 = z / (Mathf.PI * f);
            k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
            k3 = r * z / (2 * Mathf.PI * f);
        }

        public Vector3 Update(float dt, Vector3 x, Vector3? xd = null)
        {

            if(dt <= 0)
                return y;

            if (xd == null)
            {
                xd = (x - xp) / dt;
                xp = x;
            }
            float k2_stable = Mathf.Max(Mathf.Max(k2, dt * dt / 2 + dt * k1 / 2), dt * k1); // clamp k2 to guarantee stability without jitter
            y += dt * yd;
            yd += dt * (x + k3 * xd.Value - y - k1 * yd) / k2_stable; // integrate velocity by acceleration
            return y;
        }

        public void SetCurrentValue(Vector3 newY, Vector3 newYd = default)
        {
            y = newY;
            yd = newYd == default ? Vector3.zero : newYd;
            xp = newY; // Optional: also set xp to newY if you want to reset the target position
        }

        public void UpdateFZR(Vector3 fzr)
            { UpdateConstants(fzr.x, fzr.y, fzr.z); }

    }

    

}

