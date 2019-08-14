Shader "Hidden/Custom/Pixelated"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Blend;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float aspect = 1920.0/1080.0;
			
			float pixelsX = 200;
			float pixelsY = pixelsX / aspect;

			float pixelSizeX = 1.0/pixelsX;
			float pixelSizeY = 1.0/pixelsY;

			float u = floor(i.texcoord.x/pixelSizeX) / pixelsX;
			float v = floor(i.texcoord.y/pixelSizeY) / pixelsY;
			float2 texCoord = float2(u, v);
			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texCoord);

            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}