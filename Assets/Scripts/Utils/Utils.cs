using UnityEngine;

public static class Utils
{
    public static void SetRenderLayerInChildren(Transform transform, int layerNumber)
    {
        foreach (Transform trans in transform.GetComponentsInChildren<Transform>(true))
        {
            if (trans.CompareTag("IgnoreLayerChange"))
                continue;
            trans.gameObject.layer = layerNumber;
        }
    }
    public static Vector3 GetRandomPositionAtCertainPoint(Vector3 point, float range = 2f)
    {
        return new Vector3(Random.Range(point.x - range, point.x + range), point.y, Random.Range(point.z - range, point.z + range));
    }
}
