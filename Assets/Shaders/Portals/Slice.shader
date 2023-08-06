Shader "Custom/Slice"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        sliceNormal("normal", Vector) = (0,0,0,0)
        sliceCentre ("centre", Vector) = (0,0,0,0)
        sliceOffsetDst("offset", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType"="Geometry" }
        LOD 200

        CGPROGRAM
        // 基于物理的标准光源模型，并在所有光源类型上启用阴影
        #pragma surface surf Standard addshadow
        // 使用着色器模型3.0目标，以获得更好的照明效果
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // 传送门在世界坐标系的法线，从中心沿着这个方向的任何东西都是不可见的
        float3 sliceNormal;
        // 传送门在世界坐标系的中心
        float3 sliceCentre;
        // 中心偏移量，增加会使更多的网格可见，减少会使更少的网格可见
        float sliceOffsetDst;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 adjustedCentre = sliceCentre + sliceNormal * sliceOffsetDst;
            float3 offsetToSliceCentre = adjustedCentre - IN.worldPos;
            clip (dot(offsetToSliceCentre, sliceNormal));
            
            // Albedo来自由颜色着色的纹理
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "VertexLit"
}
