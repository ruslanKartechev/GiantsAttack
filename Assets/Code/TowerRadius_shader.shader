Shader "SleepDev/Rad"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Power("Power", float) = 1
        _Threshold("Threshold", float) = .5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
//        Blend One Zero
        LOD 100

        Pass
        {
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
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _Power;
            fixed _Threshold;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed ax = abs(i.uv.x - .5);
                fixed ay = abs(i.uv.y - .5);
                fixed alf = sqrt(ax*ax + ay*ay);
                if(alf > .5)
                {
                    return fixed4(0,0,0,0);
                }
                fixed inverse = saturate(_Threshold - alf);
                inverse = (pow(inverse, _Power));
                alf = saturate(alf - inverse + _Threshold);
                fixed4 col = _Color * alf;
                col.a = _Color.a * alf;
                return col;
            }
            ENDCG
        }
    }
}
