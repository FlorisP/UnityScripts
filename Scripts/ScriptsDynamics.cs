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
            float omega = 2f * Mathf.PI * f;

            k1 = z / (Mathf.PI * f);
            k2 = 1f / (omega * omega);
            k3 = r * z / omega;
        }

        public float Update(float dt, float target, float? xd = null)
        {
            if (dt <= 0)
                return y;

            // Estimate target change
            if (xd == null)
            {
                xd = (target - previous_target) / dt;
                previous_target = target;
            }

            // Calculate position and new speed
            float k2_stable = Mathf.Max(Mathf.Max(k2, dt * dt / 2f + dt * k1 / 2f), dt * k1);
            y += dt * yd;
            yd += dt * (target - y + k3 * (float)xd - k1 * yd) / k2_stable; // integrate velocity by acceleration

            return y;
        }

        public void SetPosition(float newPosition, bool resetSpeed)
        {
            y = newPosition;
            previous_target = newPosition;
            if (resetSpeed) yd = 0;
        }

        public void AddImpulse(float impulse) => yd += impulse;

        public void UpdateFZR(Vector3 fzr) => UpdateConstants(fzr.x, fzr.y, fzr.z);

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
            float omega = 2f * Mathf.PI * f;

            k1 = z / (Mathf.PI * f);
            k2 = 1f / (omega * omega);
            k3 = r * z / omega;
        }

        public Vector3 Update(float dt, Vector3 x, Vector3? xd = null)
        {

            if (dt <= 0)
                return y;

            if (xd == null)
            {
                xd = (x - xp) / dt;
                xp = x;
            }
            float k2_stable = Mathf.Max(Mathf.Max(k2, dt * dt / 2f + dt * k1 / 2f), dt * k1);
            y += dt * yd;
            yd += dt * (x + k3 * xd.Value - y - k1 * yd) / k2_stable; // integrate velocity by acceleration
            return y;
        }

        public void SetPosition(Vector3 newPosition, bool resetSpeed = false)
        {
            y = newPosition;
            xp = newPosition;
            if (resetSpeed) yd = Vector3.zero;
        }

        public void AddImpulse(Vector3 impulse) => yd += impulse;

        public void UpdateFZR(Vector3 fzr) => UpdateConstants(fzr.x, fzr.y, fzr.z);        

    }

    // Angle-safe second order dynamics (degrees, 0–360 wrap)
    public class SecondOrderDynamics360
    {
        readonly SecondOrderDynamics dynamics;

        float angleUnwrapped; // can go beyond 0..360

        public SecondOrderDynamics360(float f, float z, float r, float angle0)
        {
            float a0 = Mathf.Repeat(angle0, 360f);
            angleUnwrapped = a0;
            dynamics = new SecondOrderDynamics(f, z, r, angleUnwrapped);
        }

        public float Update(float dt, float targetAngle, float? targetAngularVelocity = null)
        {
            float currentWrapped = Mathf.Repeat(angleUnwrapped, 360f);
            float targetWrapped = Mathf.Repeat(targetAngle, 360f);

            float delta = Mathf.DeltaAngle(currentWrapped, targetWrapped); // shortest path [-180..180]
            float targetUnwrapped = angleUnwrapped + delta;

            angleUnwrapped = dynamics.Update(dt, targetUnwrapped, targetAngularVelocity);
            return Mathf.Repeat(angleUnwrapped, 360f);
        }

        public void SetAngle(float angle, bool resetSpeed)
        {
            angleUnwrapped = Mathf.Repeat(angle, 360f);
            dynamics.SetPosition(angleUnwrapped, resetSpeed);
        }

        public void AddImpulse(float angularImpulse) => dynamics.AddImpulse(angularImpulse);
        public void UpdateFZR(Vector3 fzr) => dynamics.UpdateFZR(fzr);
    }

}

