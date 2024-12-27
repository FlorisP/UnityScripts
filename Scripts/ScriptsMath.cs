using System;
using UnityEngine;

namespace Scripts
{
    class MathBounds
    {
        public static T LowestAbs<T>(T n1, T n2) where T : IComparable<T>
        {
            if(Math.Abs(Convert.ToDouble(n1)) < Math.Abs(Convert.ToDouble(n2)))
                return n1;
            else
                return n2;
        }

        public static float BoundFloat(float value, Vector2 bounds) 
        {
            float width = Math.Abs(bounds.x - bounds.y);
            while (value < (bounds.x < bounds.y ? bounds.x : bounds.y)) value += width;
            while (value > (bounds.x > bounds.y ? bounds.x : bounds.y)) value -= width;
            return value;
        }

        /// <summary> Bounds Vector2(exclusive low/inclusive high) </summary> 
        public static int BoundInt(int value, Vector2Int bounds)
        {
            int width = Math.Abs(bounds.x - bounds.y);
            while (value < (bounds.x < bounds.y ? bounds.x : bounds.y)) value += width;
            while (value > (bounds.x > bounds.y ? bounds.x : bounds.y)) value -= width;
            return value;
        }

        public static int boundIntZero(int value, int bound)
        {
            if (value == -1)
                return bound;
            return BoundInt(value, new Vector2Int(-1,bound));
        }

    }

    class MathVectors
    {
        public static Vector3 MultiplyVectors(Vector3 vec1, Vector3 vec2)
            {return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);}
        public static Vector3 Abs(Vector3 vector)
            {return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));}
 
        public static Vector3 Vector3xChange(Vector3 original, float newValue)
            {return new Vector3(newValue, original.y, original.z);}
        public static Vector3 Vector3yChange(Vector3 original, float newValue)
            {return new Vector3(original.x, newValue, original.z);}
        public static Vector3 Vector3zChange(Vector3 original, float newValue)
            {return new Vector3(original.x, original.y, newValue);}
            
        public static float GetAngle360(Vector2 origin, Vector2 target)
        {
            Vector2 direction = target - origin;
            double angleRadians = Mathf.Atan2(-direction.x, direction.y); // Inverted Y for 0 = up
            double angleDegrees = angleRadians * (180 / Mathf.PI);
            return (float)((angleDegrees + 360) % 360);         // Ensure angle is in 0-360 range
        } 
   }

    class MathAngles
    {
        public static float BoundAngle(float angle){return Mathf.Repeat(angle + 180f, 360f) -180f;}


        public static Vector2 AngleToVector2(float angle) // 0*=(0,1) 90*=((1,0))
        {
            float radian = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
        }

        public static float Vector2ToAngle(Vector2 vector)
        {
            float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
            return angle;
        }

        public static float LerpAngle(float from, float to, float t, bool clamp = true)
        {
            if(clamp) t = Mathf.Clamp01(t);
            float delta = (to - from + 540.0f) % 360.0f - 180.0f;
            return BoundAngle(from + delta * t);
        }

        public static Vector2 RotateVector(Vector2 v, float degrees) // Clockwise
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(-radians);
            float cos = Mathf.Cos(-radians);
            return new Vector2( cos * v.x - sin * v.y, sin * v.x + cos * v.y );
        }

        public static float Vector2AngleDifference(Vector2 start, Vector2 end)
        {
            float angle = Vector2ToAngle(end) - Vector2ToAngle(start);
            return BoundAngle(angle);
        }


        /// <summary> Mathf.MoveTowardsAngle in Rads </summary> 
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float delta = Math.Abs(target - current);

            float leftDir = current + Math.Min(delta, maxDelta);
            float rightDir = current - Math.Min(delta, maxDelta);

            if (delta < Math.PI)       // Not Breaking Pi Circle
                return (current < target) ? leftDir : rightDir;
            else//(delta > Mathf.PI)    // Breaking Pi Circle
                return (current < target) ? rightDir : leftDir;
        }

        /// <summary> // Target angle is left (1) or right (-1) or ~same (0) of current angle. </summary> 
        public static int LeftOrRight(float current, float target, float threshold = 0f)
        {
            float delta = Mathf.Abs(target - current);
            if (Math.Abs(current - target) < threshold)
                return 0;

            if (delta < Mathf.PI)       // Not Breaking Pi Circle
                return (current < target) ? 1 : -1;
            else//(delta > Mathf.PI)    // Breaking Pi Circle
                return (current < target) ? -1 : 1;
        }
        
    }

    class MathGeometry
    {
        /// <summary> Inputs center/size/angle of box. Returns 4 cornerpoints (+ 4 sidepoints)</summary> \
        /// <param name="center"> Center of box </param>
        /// <param name="size"> Size of box</param>
        /// <param name="lerpOffset">  How much the center is offset, used by LERP </param>
        public static Vector2[] BoxFeaturesToPoints(Vector2 center, Vector2 size, float angle, bool isComplexBox = false, Vector2? lerpOffset = null)
        {
            Vector2[] points = new Vector2[4];
            size /= 2;
            points[0] = new Vector2(-1, 1) * size; // LeftUp
            points[1] = new Vector2(1, 1) * size; // RightUp
            points[2] = new Vector2(-1, -1) * size; // LeftDown
            points[3] = new Vector2(1, -1) * size; // RightDown

            for (int i = 0; i < 4; i++)
            {
                points[i] = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward) * points[i];
                points[i] += center;
            }

            Debug.DrawLine(points[0], points[1], Color.blue); Debug.DrawLine(points[2], points[3], Color.blue); // Horizontal
            Debug.DrawLine(points[0], points[2], Color.blue); Debug.DrawLine(points[1], points[3], Color.blue); // Vectival

            if (isComplexBox)
            {
                if (lerpOffset == null) lerpOffset = new Vector2(0.5f, 0.5f);

                Vector2[] morePoints = new Vector2[8];
                points.CopyTo(morePoints, 0);

                morePoints[4] = Vector2.Lerp(points[0], points[2], lerpOffset.Value.x);
                morePoints[5] = Vector2.Lerp(points[1], points[3], lerpOffset.Value.x);
                morePoints[6] = Vector2.Lerp(points[0], points[1], lerpOffset.Value.y);
                morePoints[7] = Vector2.Lerp(points[2], points[3], lerpOffset.Value.y);

                Debug.DrawLine(morePoints[4], morePoints[5], Color.blue); Debug.DrawLine(morePoints[6], morePoints[7], Color.blue);

                return morePoints;
            }
            return points;
        }
    }


}

