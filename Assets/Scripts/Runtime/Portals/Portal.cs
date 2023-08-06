using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour 
{
    [Header ("Main Settings")]
    public Portal linkedPortal;
    public MeshRenderer screen;
    public int recursionLimit = 5;

    [Header ("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    private RenderTexture viewTexture;
    private Camera portalCam;
    private Camera playerCam;
    private Material firstRecursionMat;
    private List<PortalTraveller> trackedTravellers;
    private MeshFilter screenMeshFilter;

    void Awake () 
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera> ();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller> ();
        screenMeshFilter = screen.GetComponent<MeshFilter> ();
        screen.material.SetInt ("displayMask", 1);
    }

    void LateUpdate () 
    {
        HandleTravellers ();
    }

    void HandleTravellers () 
    {

        for (int i = 0; i < trackedTravellers.Count; i++) 
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign (Vector3.Dot (offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign (Vector3.Dot (traveller.previousOffsetFromPortal, transform.forward));
            // 传送检测，检测是否穿过传送门到另一边
            if (portalSide != portalSideOld) 
            {
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport (transform, linkedPortal.transform, m.GetColumn (3), m.rotation);
                traveller.graphicsClone.transform.SetPositionAndRotation (positionOld, rotOld);
                // 不能依赖OnTriggerEnter/Exit来调用下一帧，因为它取决于FixedUpdate运行的时间，不立刻移除可能被多次传送
                linkedPortal.OnTravellerEnterPortal (traveller);
                trackedTravellers.RemoveAt (i);
                i--;

            } 
            else 
            {
                traveller.graphicsClone.transform.SetPositionAndRotation (m.GetColumn (3), m.rotation);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    /// <summary>
    /// 在为当前帧渲染任何传送门相机之前调用
    /// </summary>
    public void PrePortalRender () 
    {
        foreach (var traveller in trackedTravellers) 
        {
            UpdateSliceParams (traveller);
        }
    }

    /// <summary>
    /// 手动渲染传送门相机
    /// </summary>
    public void Render () 
    {
        // 如果玩家没有看到相连的另一个传送门，则跳过此传送门相机的渲染
        if (!CameraUtility.VisibleFromCamera (linkedPortal.screen, playerCam)) 
        {
            return;
        }

        CreateViewTexture ();

        var localToWorldMatrix = playerCam.transform.localToWorldMatrix;
        var renderPositions = new Vector3[recursionLimit];
        var renderRotations = new Quaternion[recursionLimit];

        int startIndex = 0;
        portalCam.projectionMatrix = playerCam.projectionMatrix;
        for (int i = 0; i < recursionLimit; i++) 
        {
            if (i > 0) 
            {
                // 如果相连的传送门在此传送门中不可见，则无需递归渲染
                if (!CameraUtility.BoundsOverlap (screenMeshFilter, linkedPortal.screenMeshFilter, portalCam)) 
                {
                    break;
                }
            }
            localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            int renderOrderIndex = recursionLimit - i - 1;
            renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn (3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

            portalCam.transform.SetPositionAndRotation (renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        // 隐藏传送门屏幕以看到传送门屏幕之后的画面
        screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        linkedPortal.screen.material.SetInt ("displayMask", 0);

        for (int i = startIndex; i < recursionLimit; i++) 
        {
            portalCam.transform.SetPositionAndRotation (renderPositions[i], renderRotations[i]);
            SetNearClipPlane ();
            HandleClipping ();
            portalCam.Render ();

            if (i == startIndex) 
            {
                linkedPortal.screen.material.SetInt ("displayMask", 1);
            }
        }

        // 取消渲染前传送门屏幕的隐藏
        screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    /// <summary>
    /// 通过调整偏移尽量减轻因调整传送门屏幕厚度带来的模型的微小接缝
    /// </summary>
    void HandleClipping () 
    {
        const float hideDst = -1000;
        const float showDst = 1000;
        float screenThickness = linkedPortal.ProtectScreenFromClipping (portalCam.transform.position);

        foreach (var traveller in trackedTravellers) 
        {
            if (SameSideOfPortal (traveller.transform.position, portalCamPos)) 
            {
                traveller.SetSliceOffsetDst (hideDst, false);
            } 
            else 
            {
                traveller.SetSliceOffsetDst (showDst, false);
            }

            // 确保克隆已正确裁剪，以防通过此传送门可见：
            int cloneSideOfLinkedPortal = -SideOfPortal (traveller.transform.position);
            bool camSameSideAsClone = linkedPortal.SideOfPortal (portalCamPos) == cloneSideOfLinkedPortal;
            if (camSameSideAsClone) 
            {
                traveller.SetSliceOffsetDst (screenThickness, true);
            } 
            else 
            {
                traveller.SetSliceOffsetDst (-screenThickness, true);
            }
        }

        var offsetFromPortalToCam = portalCamPos - transform.position;
        foreach (var linkedTraveller in linkedPortal.trackedTravellers) 
        {
            var travellerPos = linkedTraveller.graphicsObject.transform.position;
            var clonePos = linkedTraveller.graphicsClone.transform.position;
            // 处理连接传送门的克隆对象
            bool cloneOnSameSideAsCam = linkedPortal.SideOfPortal (travellerPos) != SideOfPortal (portalCamPos);
            if (cloneOnSameSideAsCam) 
            {
                linkedTraveller.SetSliceOffsetDst (hideDst, true);
            } 
            else 
            {
                linkedTraveller.SetSliceOffsetDst (showDst, true);
            }

            // 确保该传送门看到正确被裁剪的在连接的传送门那边的对象
            bool camSameSideAsTraveller = linkedPortal.SameSideOfPortal (linkedTraveller.transform.position, portalCamPos);
            if (camSameSideAsTraveller) 
            {
                linkedTraveller.SetSliceOffsetDst (screenThickness, false);
            } 
            else 
            {
                linkedTraveller.SetSliceOffsetDst (-screenThickness, false);
            }
        }
    }

    /// <summary>
    /// 在主相机渲染前，所有传送门渲染完成后调用
    /// </summary>
    public void PostPortalRender () 
    {
        foreach (var traveller in trackedTravellers) 
        {
            UpdateSliceParams (traveller);
        }
        ProtectScreenFromClipping (playerCam.transform.position);
    }

    void CreateViewTexture () 
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) 
        {
            if (viewTexture != null) 
            {
                viewTexture.Release ();
            }
            viewTexture = new RenderTexture (Screen.width, Screen.height, 0);
            // 把画面渲染到纹理上
            portalCam.targetTexture = viewTexture;
            // 把纹理贴到连接的传送门上
            linkedPortal.screen.material.SetTexture ("_MainTex", viewTexture);
        }
    }

    /// <summary>
    /// 设置入口屏幕的厚度，以便玩家通过时不会被相机近裁剪面裁剪,也不会与近裁剪面交错
    /// </summary>
    float ProtectScreenFromClipping (Vector3 viewPoint) 
    {
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan (playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = screen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot (transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3 (screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    void UpdateSliceParams (PortalTraveller traveller) 
    {
        // 计算裁剪用传送门法线
        int side = SideOfPortal (traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = linkedPortal.transform.forward * side;

        // 获取裁剪用传送门中心
        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = linkedPortal.transform.position;

        // 调整传送物体的裁剪偏移，防止跟主摄像头不在同一侧，夹在偏移后的传送门屏幕之间
        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        float screenThickness = screen.transform.localScale.z;

        bool playerSameSideAsTraveller = SameSideOfPortal (playerCam.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller) 
        {
            sliceOffsetDst = -screenThickness;
        }
        bool playerSameSideAsCloneAppearing = side != linkedPortal.SideOfPortal (playerCam.transform.position);
        if (!playerSameSideAsCloneAppearing) 
        {
            cloneSliceOffsetDst = -screenThickness;
        }

        for (int i = 0; i < traveller.originalMaterials.Length; i++) 
        {
            traveller.originalMaterials[i].SetVector ("sliceCentre", slicePos);
            traveller.originalMaterials[i].SetVector ("sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat ("sliceOffsetDst", sliceOffsetDst);

            traveller.cloneMaterials[i].SetVector ("sliceCentre", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector ("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat ("sliceOffsetDst", cloneSliceOffsetDst);

        }

    }

    /// <summary>
    /// 调整投影矩阵使相机近裁剪面倾斜，与传送门平面对齐，（会影响z buffer精度）
    /// 原理：http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
    /// </summary>
    void SetNearClipPlane () 
    {
        Transform clipPlane = transform;
        int dot = System.Math.Sign (Vector3.Dot (clipPlane.forward, transform.position - portalCam.transform.position));

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint (clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector (clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot (camSpacePos, camSpaceNormal) + nearClipOffset;

        // 如果两平面非常靠近，就不用斜近裁剪面了
        if (Mathf.Abs (camSpaceDst) > nearClipLimit) 
        {
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // 用主相机（玩家相机）计算并更新传送门相机，保持两边其他设定一致
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix (clipPlaneCameraSpace);
        } 
        else 
        {
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

    void OnTravellerEnterPortal (PortalTraveller traveller) 
    {
        if (!trackedTravellers.Contains (traveller)) 
        {
            traveller.EnterPortalThreshold ();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add (traveller);
        }
    }

    void OnTriggerEnter (Collider other) 
    {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller) 
        {
            OnTravellerEnterPortal (traveller);
        }
    }

    void OnTriggerExit (Collider other) 
    {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller && trackedTravellers.Contains (traveller)) 
        {
            traveller.ExitPortalThreshold ();
            trackedTravellers.Remove (traveller);
        }
    }


    int SideOfPortal (Vector3 pos) 
    {
        return System.Math.Sign (Vector3.Dot (pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal (Vector3 posA, Vector3 posB) 
    {
        return SideOfPortal (posA) == SideOfPortal (posB);
    }

    Vector3 portalCamPos 
    {
        get 
        {
            return portalCam.transform.position;
        }
    }

    void OnValidate () 
    {
        if (linkedPortal != null) 
        {
            linkedPortal.linkedPortal = this;
        }
    }
}