Shader "Custom/UnlitWithShadowAndEdge"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _MaxBrightnessColor ("Max Brightness Color", Color) = (1,1,1,1)
        _EdgeStrength ("Edge Bleach Strength", Range(0, 1)) = 0.5
        _EdgeWidth ("Edge Bleach Width", Range(0, 1)) = 0.2
        _TiltY ("Tilt Brightness along Y", Range(-1, 1)) = 0.0
        _EraseTop ("Erase From Top (0-1)", Range(0, 1)) = 0.0
        _EraseBottom ("Erase From Bottom (0-1)", Range(0, 1)) = 1.0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float4 _EdgeColor;
            float4 _MaxBrightnessColor;
            float _EdgeStrength;
            float _EdgeWidth;
            float _TiltY;
            float _EraseTop;
            float _EraseBottom;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                if (1.0 - _EraseTop < i.uv.y || i.uv.y < 1.0 - _EraseBottom)
                    return float4(0., 0., 0., 0.);

                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

                float2 edgeDist = min(i.uv, 1.0 - i.uv);
                float minDist = min(edgeDist.x, edgeDist.y);

                float edgeFactor = pow(smoothstep(_EdgeWidth, 0.0, minDist), 5);
                col.rgba = lerp(col.rgba, _EdgeColor, edgeFactor * _EdgeStrength);
                col.rgb *= 1.0 + _TiltY * (i.uv.y - 0.5) * 2.0;
                col.rgb = min(col.rgb, _MaxBrightnessColor.rgb);

                return col;
            }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    FallBack "Diffuse"
}
