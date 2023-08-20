using UnityEngine;

/// <summary>
/// 优化渲染用相机工具类
/// </summary>
public static class CameraUtility
{
    static readonly Vector3[] cubeCornerOffsets =
    {
        new Vector3 (1, 1, 1),
        new Vector3 (-1, 1, 1),
        new Vector3 (-1, -1, 1),
        new Vector3 (-1, -1, -1),
        new Vector3 (-1, 1, -1),
        new Vector3 (1, -1, -1),
        new Vector3 (1, 1, -1),
        new Vector3 (1, -1, 1),
    };

    /// <summary>
    /// 判断renderer是否在camera视锥体内，用于剔除传送门贴图渲染
    /// </summary>
    public static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    /// <summary>
    /// 简单用边界判断传送门是否可被链接的传送门看到
    /// </summary>
    public static bool BoundsOverlap(MeshFilter nearObject, MeshFilter farObject, Camera camera)
    {

        var near = GetScreenRectFromBounds(nearObject, camera);
        var far = GetScreenRectFromBounds(farObject, camera);

        // z轴重叠
        if (far.zMax > near.zMin)
        {
            // x轴分量无重叠
            if (far.xMax < near.xMin || far.xMin > near.xMax)
            {
                return false;
            }
            // y轴分量无重叠
            if (far.yMax < near.yMin || far.yMin > near.yMax)
            {
                return false;
            }
            // 存在重叠
            return true;
        }
        return false;
    }

    public static MinMax3D GetScreenRectFromBounds(MeshFilter renderer, Camera mainCamera)
    {
        MinMax3D minMax = new MinMax3D(float.MaxValue, float.MinValue);

        Vector3[] screenBoundsExtents = new Vector3[8];
        var localBounds = renderer.sharedMesh.bounds;
        bool anyPointIsInFrontOfCamera = false;

        for (int i = 0; i < 8; i++)
        {
            Vector3 localSpaceCorner = localBounds.center + Vector3.Scale(localBounds.extents, cubeCornerOffsets[i]);
            Vector3 worldSpaceCorner = renderer.transform.TransformPoint(localSpaceCorner);
            Vector3 viewportSpaceCorner = mainCamera.WorldToViewportPoint(worldSpaceCorner);

            if (viewportSpaceCorner.z > 0)
            {
                anyPointIsInFrontOfCamera = true;
            }
            else
            {
                // 如果点在摄影机后面，它会翻转到另一侧，逆转到相反边缘来纠正
                viewportSpaceCorner.x = (viewportSpaceCorner.x <= 0.5f) ? 1 : 0;
                viewportSpaceCorner.y = (viewportSpaceCorner.y <= 0.5f) ? 1 : 0;
            }

            // 使用新的角点更新边界
            minMax.AddPoint(viewportSpaceCorner);
        }

        // 所有点都在摄像机后面，返回空白边界
        if (!anyPointIsInFrontOfCamera)
        {
            return new MinMax3D();
        }

        return minMax;
    }

    public struct MinMax3D
    {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
        public float zMin;
        public float zMax;

        public MinMax3D(float min, float max)
        {
            this.xMin = min;
            this.xMax = max;
            this.yMin = min;
            this.yMax = max;
            this.zMin = min;
            this.zMax = max;
        }

        public void AddPoint(Vector3 point)
        {
            xMin = Mathf.Min(xMin, point.x);
            xMax = Mathf.Max(xMax, point.x);
            yMin = Mathf.Min(yMin, point.y);
            yMax = Mathf.Max(yMax, point.y);
            zMin = Mathf.Min(zMin, point.z);
            zMax = Mathf.Max(zMax, point.z);
        }
    }

}