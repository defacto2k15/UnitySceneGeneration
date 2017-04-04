Shader "Custom/FragTestShader"
	{
	Properties 
	{
		_MainTex ("", 2D) = "white" {}
	}
 
	SubShader 
	{
		
		Pass
		{
		ZTest Always Cull Off ZWrite Off Fog { Mode Off } //Rendering settings
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" 
			//we include "UnityCG.cginc" to use the appdata_img struct
			#include "billboardGrassGenerator.inc"
    
			struct v2f {
				float4 pos : POSITION; // niezbedna wartosc by dzialal shader
				half2 uv : TEXCOORD0;
			};
   
			//Our Vertex Shader 
			v2f vert (appdata_img v){
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex); //niezbedna linijka by dzialal shader
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return o; 
			}
    
			sampler2D _MainTex; //Reference in Pass is necessary to let us use this variable in shaders
    
			//Our Fragment Shader
			fixed4 frag (v2f i) : Color{
				// sample texture and return it
				half2 pos = i.uv;
				pos.x = remapNeg(pos.x);
				fixed4 col = billboard_grass_generator_surf( pos, 22, 100);
				col.w = step(0.1,col.w); 
				return col;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}