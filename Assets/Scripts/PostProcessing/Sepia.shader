Shader "Hidden/Custom/Sepia"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Blend;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            
			float outputRed = (color.r * .393) + (color.g *.769) + (color.b * .189);
			float outputGreen = (color.r * .349) + (color.g *.686) + (color.b * .168);
			float outputBlue = (color.r * .272) + (color.g *.534) + (color.b * .131);

			color.rgb = float3(outputRed, outputGreen, outputBlue);
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