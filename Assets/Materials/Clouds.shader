Shader "Unlit/Clouds"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_CloudScale("CloudScale", float) = 0.5
		_Darkness("Darkness", float) = 0.55
		_Softness("Softness", float) = 0.2

		_CloudColor("CloudColor", Color) = (1,1,1,1)
		_FBMFactor("FBMFactor", float) = 3

		_Distance("Distance", float) = 1
	}
	SubShader
	{
		Tags {"RenderType"="Opaque"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float _CloudScale;
			float _Darkness;
			float4 _CloudColor;
			float _Softness;
			float _FBMFactor;
			float _Distance;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float saturate(float num)
			{
				return clamp(num, 0.0, 1.0);
			}

			float noise(float2 uv)
			{
				return tex2D(_MainTex, uv);
			}

			float2 rotate(float2 uv, float amount)
			{
				float sinRot = sin(amount);
				float cosRot = cos(amount);
				float2x2 rotMatrix = float2x2(cosRot, -sinRot, sinRot, cosRot);
				return mul(rotMatrix, uv);
			}

			float2 rotate(float2 uv)
			{
				float curlStrain = 1.0;

				uv = uv + noise(uv*0.2) * 0.005;
				return rotate(uv, curlStrain);
			}

			float fbm(float2 uv, int noiseOctaves)
			{
				float f = 0.0;
				float total = 0.0;
				float mul = 0.5;
				
				float timeScale = 5.0;

				for (int i = 0; i < noiseOctaves; i++)
				{
					float2 time = float2(_Time.y, _Time.y);
					f += noise(uv + time*0.008*timeScale*(1.0-mul))*mul;
					total += mul;
					uv *= _FBMFactor;
					uv = rotate(uv); // note bdsowers - this could be removed without a huge sacrifice
					mul *= 0.5; // note bdsowers - changes this multiplier to get a different softness
				}

				return f/total;
			}

			fixed4 cloud(v2f i)
			{
				/*int pixels = 50;
				float u = i.uv.x;
				float v = i.uv.y;
				u = trunc(u * pixels) / pixels;
				v = trunc(v * pixels) / pixels;
				float2 screenUV = float2(u, v);*/
				
				float2 screenUV = i.uv;
				float2 uv = screenUV * _CloudScale;

				float cover = _Darkness * 1.1 + 0.1;

				float brightness = 1.0;
				brightness = brightness * (1.8 - cover);

				float timeScale = 0.00025;
				int noiseOctaves = 4;

				float2 time = float2(_Time.y, _Time.y);
				float color1 = fbm(uv-0.5+time*timeScale, noiseOctaves);
				float color2 = fbm(uv-10.5*time*timeScale * 0.9, noiseOctaves);

				float softness = _Softness;

				float clouds1 = smoothstep(1.0-cover, min((1.0-cover)+softness*2.0,1.0), color1);
				float clouds2 = smoothstep(1.0-cover, min((1.0-cover)+softness, 1.0), color2);

				float cloudsFromComb = saturate(clouds1+clouds2);

				float4 skyColor = float4(0.6, 0.8, 1.0, 1.0);
				float cloudCol = saturate(saturate(1.0-pow(color1,1.0)*0.2)*brightness);
				float4 clouds1Color = float4(cloudCol,cloudCol,cloudCol,1.0) * _CloudColor;
				float4 clouds2Color = lerp(clouds1Color*0.95,skyColor,0.25);
				float4 cloudColComb = lerp(clouds1Color,clouds2Color,saturate(clouds2-clouds1));
				
				float4 finalColor = lerp(skyColor, cloudColComb, cloudsFromComb * max(_Distance, 0));
				
				return finalColor;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return cloud(i);
			}

			ENDCG
		}
	}
}
