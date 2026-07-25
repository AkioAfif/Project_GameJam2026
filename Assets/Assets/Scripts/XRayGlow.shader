Shader "Custom/XRayGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (0, 1, 0.5, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2.0
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2.0
        _FresnelPower ("Fresnel Power", Range(0.1, 5)) = 1.5
    }

    SubShader
    {
        // Pass 1: X-Ray silhouette (renders behind walls)
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }

        Pass
        {
            Name "XRAY"
            // Render even when behind other objects
            ZTest Greater
            ZWrite Off
            Blend SrcAlpha One
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            fixed4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float _FresnelPower;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Fresnel effect for glowing edges
                float fresnel = pow(1.0 - saturate(dot(i.worldNormal, i.viewDir)), _FresnelPower);

                // Pulsing effect
                float pulse = 0.7 + 0.3 * sin(_Time.y * _PulseSpeed);

                fixed4 col = _GlowColor;
                col.a = fresnel * _GlowIntensity * pulse * 0.6;
                col.rgb *= _GlowIntensity * pulse;

                return col;
            }
            ENDCG
        }

        // Pass 2: Normal visible glow (renders in front, when visible)
        Pass
        {
            Name "GLOW"
            ZTest LEqual
            ZWrite Off
            Blend SrcAlpha One
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            fixed4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float _FresnelPower;

            v2f vert(appdata v)
            {
                v2f o;
                // Slightly expand vertices for glow outline
                float3 expandedPos = v.vertex.xyz + v.normal * 0.02;
                o.vertex = UnityObjectToClipPos(float4(expandedPos, 1.0));
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float fresnel = pow(1.0 - saturate(dot(i.worldNormal, i.viewDir)), _FresnelPower);
                float pulse = 0.7 + 0.3 * sin(_Time.y * _PulseSpeed);

                fixed4 col = _GlowColor;
                col.a = fresnel * _GlowIntensity * pulse * 0.4;
                col.rgb *= _GlowIntensity * pulse;

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
