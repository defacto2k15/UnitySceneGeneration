Shader "Custom/BillboardTransparent"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull Off
        LOD 200
		ZWrite On
		ColorMask 0
   
        CGPROGRAM
 
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
 
        sampler2D _MainTex;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Metallic = 0.0;
            o.Smoothness = 0.5;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Standard"
}
 