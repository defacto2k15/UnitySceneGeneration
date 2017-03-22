Shader "Custom/BillboardTransparent.Instanced"{
	Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_BendingStrength ("BendingStrength", Range(0,1)) = 0.0
		_InitialBendingValue ("InitialBendingValue", Range(-1, 1)) = 0.0
		_PlantBendingStiffness("PlantBendingStiffness", Range(0,1)) = 0.5
		_WindDirection("WindDirection", Vector) = (0.0,0.0, 0.0, 0.0)
		_PlantDirection("PlantDirection", Vector) = (0.0,0.0, 0.0, 0.0)
		_MinUv("MinUv", float) = 0
		_MaxUv("MaxUv", float) = 0.5
		_RandSeed("RandSeed", float) = 0
	}
	SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull Off
        LOD 200
		ZWrite On
		//ColorMask 0
		
		CGPROGRAM
		#define UNITY_MAX_INSTANCE_COUNT 250 //as we are nearly max buffer size, instaead of 500 per draw we'll use 250 per draw
		//#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma  surface surf Standard fullforwardshadows vertex:vert alpha:fade

        #pragma target 3.0
		#pragma multi_compile_instancing

		// Config maxcount. See manual page.
		// #pragma instancing_options


		#include "billboardGrassShader.inc"
		sampler2D _MainTex; 

		UNITY_INSTANCING_CBUFFER_START(Props)  
			UNITY_DEFINE_INSTANCED_PROP(fixed4,_Color)	 
			UNITY_DEFINE_INSTANCED_PROP(fixed, _BendingStrength )	
			UNITY_DEFINE_INSTANCED_PROP(half,  _InitialBendingValue )	
			UNITY_DEFINE_INSTANCED_PROP(fixed, _PlantBendingStiffness) 
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _WindDirection) 
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _PlantDirection) 
			UNITY_DEFINE_INSTANCED_PROP(float, _MinUv)
			UNITY_DEFINE_INSTANCED_PROP(float, _MaxUv)
			UNITY_DEFINE_INSTANCED_PROP(half, _RandSeed) 
		UNITY_INSTANCING_CBUFFER_END

		void vert(inout appdata_full v, out Input o){
			billboard_vert(v,o,
				UNITY_ACCESS_INSTANCED_PROP(_BendingStrength),
				UNITY_ACCESS_INSTANCED_PROP(_InitialBendingValue), 
				UNITY_ACCESS_INSTANCED_PROP(_PlantBendingStiffness), 
				UNITY_ACCESS_INSTANCED_PROP(_WindDirection), 
				UNITY_ACCESS_INSTANCED_PROP(_PlantDirection), 
				UNITY_ACCESS_INSTANCED_PROP(_RandSeed), 
				UNITY_ACCESS_INSTANCED_PROP(_MinUv), 
				UNITY_ACCESS_INSTANCED_PROP(_MaxUv));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			billboard_surf(IN, o, 
				_MainTex,
				UNITY_ACCESS_INSTANCED_PROP(_MinUv), 
				UNITY_ACCESS_INSTANCED_PROP(_MaxUv)); 
		}
		ENDCG
	}
	FallBack "Diffuse"
}

