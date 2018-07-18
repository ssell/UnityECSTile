Shader "ECS/Sprite" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Rect("Sub-Rectangle", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex VertMain
            #pragma fragment FragMain
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct VertInput
            {
                float4 position : POSITION;
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct FragInput
            {
                float4 position : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                //UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
            };

            FragInput VertMain(VertInput input)
            {
                FragInput output;

                UNITY_SETUP_INSTANCE_ID(input);

                output.position = UnityObjectToClipPos(input.position);
                output.position = UnityPixelSnap(output.position);
                output.color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                output.uv = input.uv;

                return output;
            }

            fixed4 FragMain(FragInput input) : SV_Target
            {
                return input.color;
            }
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}