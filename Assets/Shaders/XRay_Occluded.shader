Shader "Custom/XRay_Occluded"
{
    Properties
    {
        _XRayColor ("XRay Color", Color) = (0, 0.8, 1, 0.5)
        _RimPower  ("Rim Power", Range(1, 6)) = 2.5
        _Intensity ("Emission Intensity", Range(1, 10)) = 3.0
    }

    SubShader
    {
        // No ZTest/ZWrite here — let the CustomPass RenderStateBlock own it
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent-1"
        }

        Pass
        {
            Name "XRayOccluded"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            // Intentionally NO ZTest here — RenderStateBlock overrides it
            Cull Back

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _XRayColor;
                float  _RimPower;
                float  _Intensity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 viewDirWS  : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 posWS   = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS  = normalize(GetWorldSpaceViewDir(posWS));
                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                // Fresnel rim — edges glow brighter
                float fresnel = pow(1.0 - saturate(dot(N, V)), _RimPower);

                float4 col    = _XRayColor;
                col.rgb      *= _Intensity;
                col.a        *= fresnel * 0.8 + 0.2;

                return col;
            }
            ENDHLSL
        }
    }
}