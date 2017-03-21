Shader "Custom/BillboardTransparent"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_BendingStrength ("BendingStrength", Range(0,1)) = 0.0
		_InitialBendingValue ("InitialBendingValue", Range(-1, 1)) = 0.0
		_PlantBendingStiffness("PlantBendingStiffness", Range(0,1)) = 0.5
		_WindDirection("WindDirection", Vector) = (0.0,0.0, 0.0, 0.0)
		_PlantDirection("PlantDirection", Vector) = (0.0,0.0, 0.0, 0.0)
		_MinUv("MinUv", float) = 0
		_MaxUv("MaxUv", float) = 0.5
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull Off
        LOD 200
		ZWrite On
		ColorMask 0
   
        CGPROGRAM
 
        #pragma  surface surf Standard fullforwardshadows alpha:fade vertex:vert
        #pragma target 3.0
 

		#include "common.inc"
		#include "noise.inc"
		#include "grassGeneration.inc"

        sampler2D _MainTex;
		half _BendingStrength;
		half _InitialBendingValue;
		half _PlantBendingStiffness;
		half4 _WindDirection;
		half4 _PlantDirection; 
		fixed4 _Color;
		float _MinUv;
		float _MaxUv;
 
        struct Input {
            float2 uv_MainTex;
			half debColor;
        };
	
		void vert(inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float l = v.vertex.y; // height of vertex from 0 to 1
			half2 strengths = generateStrengths( _BendingStrength, _InitialBendingValue, _PlantBendingStiffness, _WindDirection, _PlantDirection);
			half xBendStrength = strengths.x;
			half yBendStrength = strengths.y;

			o.debColor = xBendStrength;
			//v.vertex.z = calculateZ(xBendStrength, l);
			//v.vertex.y = calculateY(xBendStrength, l);
			//v.vertex.x = calculateX(yBendStrength, v.vertex.x, v.vertex.y);
		}  
 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			IN.uv_MainTex.x = lerp(_MinUv, _MaxUv, IN.uv_MainTex.x);	
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			c.g = 0;
			c.b = 0;
			c.r = IN.debColor;
            o.Albedo = c.rgb;
            o.Metallic = 0.0;
            o.Smoothness = 0.5;
			if( IN.uv_MainTex.y > 0.95 ){
				o.Alpha = 0.0;
			} else {
				o.Alpha = c.a;
			}
        }
        ENDCG
    }
    FallBack "Standard"
}
 