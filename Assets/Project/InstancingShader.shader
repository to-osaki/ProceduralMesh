// https://docs.unity3d.com/ja/2020.3/Manual/GPUInstancing.html
Shader "Unlit/InstancingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct MeshProperties
            {
                float4x4 mat;
            };

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            StructuredBuffer<MeshProperties> _Properties;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                v2f o;
                float4 pos = mul(_Properties[instanceID].mat, v.vertex);
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return fixed4(1, 1, 1, 1);//col;
            }
            ENDHLSL
        }
    }
}
