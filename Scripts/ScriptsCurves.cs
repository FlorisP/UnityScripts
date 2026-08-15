using UnityEngine;

namespace Scripts
{
    public static class Curves
    {
        // Linear
        public static AnimationCurve Linear =>
            AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public static AnimationCurve Linear10 =>
            AnimationCurve.Linear(0f, 1f, 1f, 0f);

        // EaseInOut: sinus-achtig (traag-snel-traag)
        public static AnimationCurve EaseInOut =>
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public static AnimationCurve EaseInOut10 =>
            AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        // EaseOut: snel begin, ease (afvlakken) aan het eind
        public static AnimationCurve EaseOut => new(
            new Keyframe(0f, 0f, 0f, 2f),
            new Keyframe(1f, 1f, 0f, 0f)
        );

        public static AnimationCurve EaseOut10 => new(
            new Keyframe(0f, 1f, 0f, -2f),
            new Keyframe(1f, 0f, 0f, 0f)
        );

        public static AnimationCurve EaseOutStrong => new(
            new Keyframe(0f, 0f, 0f, 5f),
            new Keyframe(0.2f, 0.7f, 1f, 1f),
            new Keyframe(1f, 1f, 0f, 0f)
        );

        public static AnimationCurve EaseOutStrong10 => new(
            new Keyframe(0f, 1f, 0f, -5f),
            new Keyframe(0.2f, 0.3f, -1f, -1f),
            new Keyframe(1f, 0f, 0f, 0f)
        );

        // EaseIn: ease (traag) aan het begin, steil eind
        public static AnimationCurve EaseIn => new(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 1f, 2f, 0f)
        );

        public static AnimationCurve EaseIn10 => new(
            new Keyframe(0f, 1f, 0f, 0f),
            new Keyframe(1f, 0f, -2f, 0f)
        );

        public static AnimationCurve EaseInStrong => new(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.8f, 0.3f, 0.2f, 0.4f),
            new Keyframe(1f, 1f, 4f, 0f)
        );

        public static AnimationCurve EaseInStrong10 => new(
            new Keyframe(0f, 1f, 0f, 0f),
            new Keyframe(0.8f, 0.7f, -0.2f, -0.4f),
            new Keyframe(1f, 0f, -4f, 0f)
        );
    }
}
