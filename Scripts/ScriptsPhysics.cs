using UnityEngine;

namespace Scripts
{
    public static class MaterialPhysics
    {
        public static float DynamicFriction(Collider a, Collider b) =>
            DynamicFriction(Material(a), Material(b));

        public static float DynamicFriction(PhysicsMaterial a, PhysicsMaterial b) =>
            Combine(a.dynamicFriction, b.dynamicFriction, DominantMode(a.frictionCombine, b.frictionCombine));

        public static float StaticFriction(Collider a, Collider b) =>
            StaticFriction(Material(a), Material(b));

        public static float StaticFriction(PhysicsMaterial a, PhysicsMaterial b) =>
            Combine(a.staticFriction, b.staticFriction, DominantMode(a.frictionCombine, b.frictionCombine));

        public static float Bounciness(Collider a, Collider b) =>
            Bounciness(Material(a), Material(b));

        public static float Bounciness(PhysicsMaterial a, PhysicsMaterial b) =>
            Combine(a.bounciness, b.bounciness, DominantMode(a.bounceCombine, b.bounceCombine));

        public static PhysicsMaterialCombine DominantMode(PhysicsMaterialCombine a, PhysicsMaterialCombine b) =>
            Rank(b) > Rank(a) ? b : a;
            
        public static float Combine(float a, float b, PhysicsMaterialCombine mode)
        {
            if (mode == PhysicsMaterialCombine.Average)
                return (a + b) * 0.5f;
            if (mode == PhysicsMaterialCombine.Multiply)
                return a * b;
            if (mode == PhysicsMaterialCombine.Minimum)
                return Mathf.Min(a, b);
            if (mode == PhysicsMaterialCombine.Maximum)
                return Mathf.Max(a, b);

            throw new System.ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        static int Rank(PhysicsMaterialCombine mode)
        {
            if (mode == PhysicsMaterialCombine.Maximum)
                return 3;
            if (mode == PhysicsMaterialCombine.Multiply)
                return 2;
            if (mode == PhysicsMaterialCombine.Minimum)
                return 1;
            if (mode == PhysicsMaterialCombine.Average)
                return 0;

            throw new System.ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        static PhysicsMaterial Material(Collider collider)
        {
            PhysicsMaterial material = collider.sharedMaterial;
            return material != null ? material : defaultMaterial;
        }

        static readonly PhysicsMaterial defaultMaterial = new()
        {
            dynamicFriction = 0.6f,
            staticFriction = 0.6f,
            bounciness = 0.9f,
            frictionCombine = PhysicsMaterialCombine.Average,
            bounceCombine = PhysicsMaterialCombine.Average,
        };
    }
}
