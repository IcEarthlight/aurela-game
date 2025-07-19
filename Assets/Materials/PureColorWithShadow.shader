Shader "Custom/PureColorWithShadowControl"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _ShadowStrength ("Shadow Strength", Range(0,1)) = 1
        _FresnelStrength ("Fresnel Strength", Range(-1,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                SHADOW_COORDS(0) // 用于阴影传递
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            fixed4 _Color;
            fixed4 _ShadowColor;
            float _ShadowStrength;
            float _FresnelStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW(o); // 传递阴影坐标

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed shadow = SHADOW_ATTENUATION(i);
                shadow = lerp(1.0, shadow, _ShadowStrength);

                float viewDot = dot(normalize(i.worldNormal), normalize(i.viewDir));
                float viewFade = 1.0 - saturate(viewDot); // 越斜越接近 1
                float alphaFade = lerp(1.0, viewFade, _FresnelStrength);

                fixed4 finalColor = lerp(_ShadowColor, _Color, shadow);
                finalColor.a = min(1, finalColor.a * alphaFade);

                return finalColor;
            }
            ENDCG
        }

        // 阴影投射支持
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
