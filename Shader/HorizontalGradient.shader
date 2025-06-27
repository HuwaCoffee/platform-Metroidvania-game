Shader "UI/HorizontalGradient" 
{
    Properties {
        _StartColor ("Start Color", Color) = (0,0,0,1) // 左侧颜色（纯黑）
        _EndColor ("End Color", Color) = (0,0,0,0)     // 右侧颜色（透明）
    }
    SubShader {
        Tags { 
            "Queue"="Transparent"       // 在透明队列渲染
            "RenderType"="Transparent"  // 标记为透明类型
        }
        Blend SrcAlpha OneMinusSrcAlpha // 混合模式：基于Alpha混合

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION; // 模型空间顶点坐标
                float2 uv : TEXCOORD0;   // 纹理坐标
            };

            struct v2f {
                float2 uv : TEXCOORD0;    // 传递UV到片元着色器
                float4 vertex : SV_POSITION; // 裁剪空间坐标
            };

            fixed4 _StartColor, _EndColor;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 转换到裁剪空间
                o.uv = v.uv; // 传递UV
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 横向渐变：根据UV的X值插值颜色
                fixed4 col = lerp(_StartColor, _EndColor, i.uv.x);
                return col;
            }
            ENDCG
        }
    }
}