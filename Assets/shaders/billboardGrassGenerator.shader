Shader "Custom/BillboardGeneratorTransparent"
{
    Properties
    {
		//_BendingStrength("BendingStrength", Range(-1,1)) = 0
		//_DbgVal("DbgVal", Range(0,1)) = 0
		//_DbgVal2("DbgVal2", Range(0,1)) = 0
		//_Offset("Offset", Range(-1,1)) = 0
		//_BaseWidth("BaseWidth", Range(0,1)) = 0
		//_ThinningCoef("ThinningCoef", Range(0,1)) = 0
		_BladesCount("BladesCount", Range(1,500)) = 1
		_RandomSeed("RandomSeed", Range(0,1)) = 1
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

		#include "billboardGrassGenerator.inc"

		//float _BendingStrength;
		//float _DbgVal;
		//float _DbgVal2;
		//float _Offset;
		//float _BaseWidth;
		//float _ThinningCoef;
		float _BladesCount;
		float _RandomSeed;

		struct Input{
			float2 pos;
		};

		void vert(inout appdata_full v, out Input o){
			o.pos = (v.vertex.xz + 5) / 10 ;
			// now o.pos has values from 0 to 1
		}   

 
        void surf (Input IN, inout SurfaceOutputStandard o)  
        {
			float2 pos = IN.pos ;
			pos.x = remapNeg(pos.x);
			// pos.x is from -1 to 1
			// pos.y is from 0 to 1

			half4 color = billboard_grass_generator_surf(pos, _RandomSeed, _BladesCount);

			o.Albedo = color.xyz;
			o.Alpha = step(0.1,color.w); 
        }
        ENDCG
    }
    FallBack "Standard"
} 