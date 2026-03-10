using UnityEngine;

public static class TransformUtils
{
    public static void FaceTarget(Transform self, Transform target)
    {
        if (self == null || target == null) return;

        Vector3 scale = self.localScale;

        scale.x = target.position.x < self.position.x
            ? -Mathf.Abs(scale.x)
            : Mathf.Abs(scale.x);

        self.localScale = scale;
    }
}