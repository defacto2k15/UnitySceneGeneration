﻿
Shader "Custom/testSurfaceShader23.Instanced" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_BendingStrength ("BendingStrength", Range(0,1)) = 0.0
		_InitialBendingValue ("InitialBendingValue", Range(-1, 1)) = 0.0
		_PlantBendingStiffness("PlantBendingStiffness", Range(0,1)) = 0.5
		_WindDirection("WindDirection", Vector) = (0.0,0.0, 0.0, 0.0)
		_PlantDirection("PlantDirection", Vector) = (0.0,0.0, 0.0, 0.0)
		_RandSeed("RandSeed", Range(0,1)) = 0
		_DbgColor("DgbColor", Vector) = (1.0,1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		// And generate the shadow pass with instancing support
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		// Enable instancing for this shader
		#pragma multi_compile_instancing

		// Config maxcount. See manual page.
		// #pragma instancing_options


		#include "singleGrassGeneration.inc"  

		UNITY_INSTANCING_CBUFFER_START(Props)  
			UNITY_DEFINE_INSTANCED_PROP(fixed4,_Color)	 
			UNITY_DEFINE_INSTANCED_PROP(fixed, _BendingStrength )	
			UNITY_DEFINE_INSTANCED_PROP(half,  _InitialBendingValue )	
			UNITY_DEFINE_INSTANCED_PROP(fixed, _PlantBendingStiffness) 
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _WindDirection) 
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _PlantDirection) 
			UNITY_DEFINE_INSTANCED_PROP(half, _RandSeed) 
			UNITY_DEFINE_INSTANCED_PROP(half4, _DbgColor) 
		UNITY_INSTANCING_CBUFFER_END

		void vert(inout appdata_full v, out Input o){
			grass_vert(v, o, 
				UNITY_ACCESS_INSTANCED_PROP(_BendingStrength), 
				UNITY_ACCESS_INSTANCED_PROP(_InitialBendingValue), 
				UNITY_ACCESS_INSTANCED_PROP(_PlantBendingStiffness), 
				UNITY_ACCESS_INSTANCED_PROP(_WindDirection), 
				UNITY_ACCESS_INSTANCED_PROP(_PlantDirection),
				UNITY_ACCESS_INSTANCED_PROP(_Color),
				UNITY_ACCESS_INSTANCED_PROP(_RandSeed), 1); 
			half4 _Scale = half4(1,1,0,0);
			//v.vertex = mul(UNITY_MATRIX_MVP, v.vertex*_Scale);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			grass_surf(IN, o, UNITY_ACCESS_INSTANCED_PROP(_Color)); 
			
			half4 dbgColor = 	UNITY_ACCESS_INSTANCED_PROP(_DbgColor);
			o.Albedo *= dbgColor;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
