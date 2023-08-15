Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        
        Pass {
                    CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        sampler2D _MainTex;
        float _BlurSize;

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            float4 sum = float4(0, 0, 0, 0);
            float2 size = _BlurSize / _ScreenParams.xy;

            sum += tex2D(_MainTex, i.uv - 4 * size) * 0.05;
            sum += tex2D(_MainTex, i.uv - 3 * size) * 0.09;
            sum += tex2D(_MainTex, i.uv - 2 * size) * 0.12;
            sum += tex2D(_MainTex, i.uv - 1 * size) * 0.15;
            sum += tex2D(_MainTex, i.uv) * 0.16;
            sum += tex2D(_MainTex, i.uv + 1 * size) * 0.15;
            sum += tex2D(_MainTex, i.uv + 2 * size) * 0.12;
            sum += tex2D(_MainTex, i.uv + 3 * size) * 0.09;
            sum += tex2D(_MainTex, i.uv + 4 * size) * 0.05;

            sum.rgb = sum.rgb * 0.6f;

            return sum;
        }
        ENDCG
    }
    }
}