Shader "Custom/testSurfaceShader23" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_BendingStrength ("BendingStrength", Range(-1,1)) = 0.0
		_InitialBendingVal ("InitialBendingValue", Range(-1, 1)) = 0.0
		_PlantBendingStiffness("PlantBendingStiffness", Range(0,1)) = 0.5
		_DirectionData("WindDirection", Vector) = (0.0,0.0, 0.0, 0.0)
		_PlantDirection("PlantDirection", Vector) = (0.0,0.0, 0.0, 0.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _BendingStrength;
		half _InitialBendingVal;
		half _PlantBendingStiffness;
		half4 _WindDirection;
		half4 _PlantDirection;
		fixed4 _Color;
		
		half fPI(){
			return 3.14;
		}

		half stiffnesInternalFunc( half x){
			half a = lerp(0.15, 0.7, _PlantBendingStiffness);
			return (pow(sin(x*fPI()*0.5),a*2))+1;
		}

		half remapNeg(half x){ // from (0,1) to (-1,1)
			return (x-0.5)*2;
		}

		half remap(half x){ // from (-1,1) to (0,1)
			return (x+1)/2;
		}

		half stiffnessFactor(half x){ // dostaje liczbe od 0 do 1. Zwraca od 0 do 1
			half rx = remapNeg(x);
			return (stiffnesInternalFunc(pow(rx,2)) * step(0,rx) +  (2-stiffnesInternalFunc(pow(rx,2)))*(1-step(0,rx)))/2;
		}

		half generateDirectionCoef( half4 windDirection, half4 plantDirection){
			half cosVal = dot(normalize(windDirection.xy),normalize(plantDirection.xy));
			return remap(cosVal);
		}

		float bendStrength( half inWindValue, half4 windDirection, half4 plantDirection ){
			const half minStrength = 0.1;
			const half maxStrength = 0.9;

			half normalizedInitialBendingVal =  remap(_InitialBendingVal);

			half initialBendingPowCoefficient = 0.5 - 1.6*normalizedInitialBendingVal + 9*normalizedInitialBendingVal*normalizedInitialBendingVal;

			half normalized = remap(inWindValue);
			half val = pow( cos( 3.14*(normalized*1.2-0.2-1)/2.4),initialBendingPowCoefficient);
			half valueAfterStiffnessComputation = stiffnessFactor(val );

			half directionCoefficient = generateDirectionCoef( windDirection, plantDirection);

			return lerp(minStrength, maxStrength,valueAfterStiffnessComputation ) * directionCoefficient;
		}

		float bendingAngle(float bendStrengthValue ){
			return radians(90 - lerp(-90,90,bendStrengthValue));
		}

		void vert(inout appdata_full v){
			_WindDirection = half4(1,4,0,0);
			_PlantDirection = half4(1,2,0,0);

			//v.vertex.x += abs(sin(_Time[0]*v.vertex.x));
			float l = v.texcoord[1];

			const float PI = 3.14159;
			float angle = bendingAngle(bendStrength(_BendingStrength, _WindDirection, _PlantDirection));
			//v.vertex.x += l*cos(angle);
			v.vertex.z = l*cos(angle);
			v.vertex.y = abs(l*sin(angle));

			//v.vertex.z = ( generateDirectionCoef( _WindDirection, _PlantDirection))/7;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.bbb;
			// BendingStrength and smoothness come from slider variables
			//o.BendingStrength = _BendingStrength;
			//o.Smoothness = _Glossiness;

			//o.Alpha = c.a*2;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
