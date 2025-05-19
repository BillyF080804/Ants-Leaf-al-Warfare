using UnityEngine;

public class LerpLibrary {
    public static Vector2 UILerp(Vector2 start, Vector2 end, LerpType lerpType, float time) {
        switch (lerpType) {
            case LerpType.Linear:
                time = LinearEase(time);
                break;
            case LerpType.In:
                time = InEase(time);
                break;
            case LerpType.Out:
                time = OutEase(time);
                break;
            case LerpType.InOut:
                time = InOutEase(time);
                break;
            case LerpType.InBounce:
                time = InBounceEase(time);
                break;
            case LerpType.OutBounce:
                time = OutBounceEase(time);
                break;
            case LerpType.InElastic:
                time = InElasticEase(time);
                break;
            case LerpType.OutElastic:
                time = OutElasticEase(time);
                break;
            case LerpType.InBack:
                time = InBackEase(time);
                break;
            case LerpType.OutBack:
                time = OutBackEase(time);
                break;
            case LerpType.InOutBack:
                time = InOutBackEase(time);
                break;
            default:
                Debug.LogError("Error: Incorrect Lerp Type Used.");
                break;
        }

        return new Vector2(
            start.x + (end.x - start.x) * time,
            start.y + (end.y - start.y) * time
        );
    }

    public static Vector3 ObjectLerp(Vector3 start, Vector3 end, LerpType lerpType, float time) {
        switch (lerpType) {
            case LerpType.Linear:
                time = LinearEase(time);
                break;
            case LerpType.In:
                time = InEase(time);
                break;
            case LerpType.Out:
                time = OutEase(time);
                break;
            case LerpType.InOut:
                time = InOutEase(time);
                break;
            case LerpType.InBounce:
                time = InBounceEase(time);
                break;
            case LerpType.OutBounce:
                time = OutBounceEase(time);
                break;
            case LerpType.InElastic:
                time = InElasticEase(time);
                break;
            case LerpType.OutElastic:
                time = OutElasticEase(time);
                break;
            case LerpType.InBack:
                time = InBackEase(time);
                break;
            case LerpType.OutBack:
                time = OutBackEase(time);
                break;
            case LerpType.InOutBack:
                time = InOutBackEase(time);
                break;
            default:
                Debug.LogError("Error: Incorrect Lerp Type Used.");
                break;
        }

        return new Vector3(
            start.x + (end.x - start.x) * time,
            start.y + (end.y - start.y) * time,
            start.z + (end.z - start.z) * time
        );
    }

    public static Color ColorLerp(Color startColor, Color endColor, LerpType lerpType, float time) {
        switch (lerpType) {
            case LerpType.Linear:
                time = LinearEase(time);
                break;
            case LerpType.In:
                time = InEase(time);
                break;
            case LerpType.Out:
                time = OutEase(time);
                break;
            case LerpType.InOut:
                time = InOutEase(time);
                break;
            case LerpType.InBounce:
                time = InBounceEase(time);
                break;
            case LerpType.OutBounce:
                time = OutBounceEase(time);
                break;
            case LerpType.InElastic:
                time = InElasticEase(time);
                break;
            case LerpType.OutElastic:
                time = OutElasticEase(time);
                break;
            case LerpType.InBack:
                time = InBackEase(time);
                break;
            case LerpType.OutBack:
                time = OutBackEase(time);
                break;
            case LerpType.InOutBack:
                time = InOutBackEase(time);
                break;
            default:
                Debug.LogError("Error: Incorrect Lerp Type Used.");
                break;
        }

        return new Color(
            startColor.r + (endColor.r - startColor.r) * time,
            startColor.g + (endColor.g - startColor.g) * time,
            startColor.b + (endColor.b - startColor.b) * time
        );
    }

    public static float LinearEase(float t) {
        return t;
    }

    public static float InEase(float t) {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }

    public static float OutEase(float t) {
        return Mathf.Sin((t * Mathf.PI) / 2);
    }

    public static float InOutEase(float t) {
        return -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
    }

    public static float OutBounceEase(float t) {
        const float a = 7.5625f;
        const float b = 2.75f;

        if (t < 1 / b) {
            return a * t * t;
        }
        else if (t < 2 / b) {
            return a * (t -= 1.5f / b) * t + 0.75f;
        }
        else if (t < 2.5f / b) {
            return a * (t -= 2.25f / b) * t + 0.9375f;
        }
        else {
            return a * (t -= 2.625f / b) * t + 0.985375f;
        }
    }

    public static float InBounceEase(float t) {
        return 1 - OutBounceEase(1 - t);
    }

    //? means return, : means if/else if
    public static float InElasticEase(float t) {
        const float a = (2 * Mathf.PI) / 3;

        return t == 0 ? 0
            : t == 1 ? 1
            : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * a);
    }

    public static float OutElasticEase(float t) {
        const float a = (2 * Mathf.PI) / 3;

        return t == 0 ? 0
            : t == 1 ? 1
            : -Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * a) + 1;
    }

    public static float InBackEase(float t) {
        const float a = 1.70158f;
        const float b = a + 1;

        return b * t * t * t - a * t * t;
    }

    public static float OutBackEase(float t) {
        const float a = 1.70158f;
        const float b = a + 1;

        return 1 + b * Mathf.Pow(t - 1, 3) + a * Mathf.Pow(t - 1, 2);
    }

    public static float InOutBackEase(float t) {
        const float a = 1.70158f;
        const float b = a * 1.525f;

        return t < 0.5f
            ? (Mathf.Pow(2 * t, 2) * ((b + 1) * 2 * t - b)) / 2
            : (Mathf.Pow(2 * t - 2, 2) * ((b + 1) * (t * 2 - 2) + b) + 2) / 2;
    }
}

public enum LerpType {
    Linear,
    In,
    Out,
    InOut,
    InBounce,
    OutBounce,
    InElastic,
    OutElastic,
    InBack,
    OutBack,
    InOutBack
}