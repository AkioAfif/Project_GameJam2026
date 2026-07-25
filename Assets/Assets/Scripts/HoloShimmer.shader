Shader "Custom/HoloShimmer"
{
    Properties
    {
        _ShimmerColor1 ("Shimmer Color 1", Color) = (0.3, 0.8, 1.0, 1)
        _ShimmerColor2 ("Shimmer Color 2", Color) = (1.0, 0.3, 0.8, 1)
        _ShimmerColor3 ("Shimmer Color 3", Color) = (0.5, 1.0, 0.3, 1)
        _ShimmerSpeed ("Shimmer Interval", Range(0.5, 5)) = 1.0
        _ShimmerWidth ("Shimmer Width", Range(0.01, 0.3)) = 0.08
        _ShimmerIntensity ("Shimmer Intensity", Range(0, 5)) = 2.5
        _FresnelPower ("Edge Glow Power", Range(0.5, 5)) = 2.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent+1" "RenderType" = "Transparent" }

        Pass
        {
            Name "HOLOSHIMMER"
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float2 uv : TEXCOORD3;
            };

            fixed4 _ShimmerColor1;
            fixed4 _ShimmerColor2;
            fixed4 _ShimmerColor3;
            float _ShimmerSpeed;
            float _ShimmerWidth;
            float _ShimmerIntensity;
            float _FresnelPower;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Fresnel edge glow — tepi objek lebih terang
                float fresnel = pow(1.0 - saturate(dot(i.worldNormal, i.viewDir)), _FresnelPower);

                // Hitung posisi shimmer band (bergerak diagonal melintasi permukaan)
                float diagonal = i.worldPos.x + i.worldPos.y + i.worldPos.z;

                // Interval shimmer: band muncul setiap _ShimmerSpeed detik
                float timePos = frac(_Time.y / _ShimmerSpeed);
                float bandCenter = timePos * 10.0 - 5.0; // Band bergerak dari -5 ke +5

                // Shimmer band — garis terang yang bergerak
                float distToBand = abs(frac(diagonal * 0.2) - timePos);
                distToBand = min(distToBand, 1.0 - distToBand); // Wrap around
                float shimmerBand = smoothstep(_ShimmerWidth, 0.0, distToBand);

                // Rainbow color shift berdasarkan posisi di permukaan
                float colorPhase = frac(diagonal * 0.15 + _Time.y * 0.3);
                fixed4 shimmerColor;
                if (colorPhase < 0.33)
                    shimmerColor = lerp(_ShimmerColor1, _ShimmerColor2, colorPhase * 3.0);
                else if (colorPhase < 0.66)
                    shimmerColor = lerp(_ShimmerColor2, _ShimmerColor3, (colorPhase - 0.33) * 3.0);
                else
                    shimmerColor = lerp(_ShimmerColor3, _ShimmerColor1, (colorPhase - 0.66) * 3.0);

                // Gabungkan: shimmer band + subtle edge glow
                float shimmerAlpha = shimmerBand * _ShimmerIntensity * 0.7;
                float edgeAlpha = fresnel * 0.15; // Edge glow tipis terus-menerus

                fixed4 col = shimmerColor;
                col.a = shimmerAlpha + edgeAlpha;
                col.rgb *= _ShimmerIntensity;

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
