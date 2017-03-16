﻿#include "noise.inc"

#ifndef GRASS_GENERATION_INC
#define GRASS_GENERATION_INC

half stiffnesInternalFunc( half x, half plantBendingStiffness){
	half a = lerp(0.15, 0.7, plantBendingStiffness);
	return (pow(sin(x*fPI()*0.5),a*2))+1;
}

half stiffnessFactor(half x, half plantBendingStiffness){ // dostaje liczbe od 0 do 1. Zwraca od 0 do 1
	half rx = remapNeg(x);
	return (stiffnesInternalFunc(pow(rx,2), plantBendingStiffness) * step(0,rx) +  (2-stiffnesInternalFunc(pow(rx,2),  plantBendingStiffness))*(1-step(0,rx)))/2;
}

half generateDirectionCoef( half4 windDirection, half4 plantDirection){
	half cosVal = dot(normalize(windDirection.xy),normalize(plantDirection.xy));
	return(cosVal);
}

// inWindValue - value from 0 to 1
// returns value from 0 to 1
float calculateXBendStrength( half inWindValue, half4 windDirection, half4 plantDirection, half4 initialBendingValue, half plantBendingStiffness ){
	half normalizedInitialBendingVal =  remap(initialBendingValue );

	half initialBendingPowCoefficient = 0.5 - 1.6*normalizedInitialBendingVal + 9*normalizedInitialBendingVal*normalizedInitialBendingVal;

	half directionCoefficient = generateDirectionCoef( windDirection, plantDirection);
	half normalized = remap(inWindValue * directionCoefficient);
	// now normalized has value from -1 to 1, which also sets the direction of bend

	half val = pow( cos( 3.14*(normalized*1.2-0.2-1)/2.4),initialBendingPowCoefficient);
	half valueAfterStiffnessComputation = stiffnessFactor(val, plantBendingStiffness );

	const half minStrength = 0.1;
	const half maxStrength = 0.9;
	return lerp(minStrength, maxStrength,valueAfterStiffnessComputation  );
}

// inWindValue - value from 0 to 1
// returns value from 0 to 1
float calculateYBendStrength( half inWindValue, half4 windDirection, half4 plantDirection ){
	half directionCoefficient = sqrt(generateDirectionCoef( windDirection, plantDirection.yxzw)); //rotated as we check if wind is blowing on side
	half normalized = remap(inWindValue * directionCoefficient);
	// now normalized has value from -1 to 1, which also sets the direction of bend

	const half minStrength = 0.1;
	const half maxStrength = 0.9;
	return lerp(minStrength, maxStrength,normalized  );
}

float bendingAngle(float bendStrengthValue ){
	return radians(90 - lerp(-90,90,bendStrengthValue));
}

half4 assertNotZero(half4 inVal){
	if( inVal.x == 0 && inVal.y == 0 && inVal.z == 0 && inVal.w == 0 ){
		return half4(0.01,0,0,0);
	} else {
		return inVal;
	}
}

// returns values from -1 to 1
half generateRandomWindMoveMultiplier( half bendingStrength ){
	half currentNoiseValue = noise1D(_Time[3]);
	half randomSinusDegreeOffsetAccordingToPosition 
		= rand((UNITY_MATRIX_T_MV[0]+UNITY_MATRIX_T_MV[1]+UNITY_MATRIX_T_MV[2]+UNITY_MATRIX_T_MV[3]));
	half randomNoiseMultiplier = lerp(0.8, 1.2, currentNoiseValue);
	half bendStrengthMultiplier = lerp(0.4, 0.6, sin(pow(bendingStrength,2) * fPI()));  
	half frequencyMultiplier = lerp(0.5, 1.5, pow(bendingStrength,2));

	return  sin(_Time[3] * frequencyMultiplier * 2 + randomSinusDegreeOffsetAccordingToPosition*3) * randomNoiseMultiplier*bendStrengthMultiplier ;
}

half calculateNormal(half yPos, half zPos ){
	zPos = max(zPos, 0.001); // not to divide by 0
	half angle = atan( yPos / zPos );
	return normalize( half3(1.0, cos(angle), -sin(angle)));
}

		struct Input {
			float2 objectSpacePos;
			float3 normal; 
			float3 viewDir;
		};

		void grass_vert(inout appdata_full v, out Input o, 
				half l_BendingStrength, half l_InitialBendingValue, half l_PlantBendingStiffness, half4 l_WindDirection, half4 l_PlantDirection, fixed4 l_Color  ){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			l_WindDirection = assertNotZero(l_WindDirection);
			l_PlantDirection = assertNotZero(l_PlantDirection);
			o.objectSpacePos = v.vertex.xy;

			float l = v.texcoord[1]; // height of vertex from 0 to 1

			half randomWindMultiplier =  generateRandomWindMoveMultiplier(l_BendingStrength);
			half xBendStrength = remapNeg(calculateXBendStrength(l_BendingStrength, l_WindDirection, l_PlantDirection, l_InitialBendingValue, l_PlantBendingStiffness));
			xBendStrength *= ( 1 + 0.2* randomWindMultiplier ); 

			half yBendStrength = remapNeg(calculateYBendStrength(l_BendingStrength, l_WindDirection, l_PlantDirection));
			yBendStrength *= ( 1 + 0.2* randomWindMultiplier ); 

			v.vertex.z = xBendStrength*(  (sin(l*fPI()/2-0.5*fPI())+1)/2)  * (1 + (sin(fPI()/2 * abs(xBendStrength))))*0.9;
			v.vertex.y = l *  sin(lerp( 0,fPI()/2, 1- abs(xBendStrength)));

			// v.vertex.x has from 0 to 1
			half remapedXVertex = remapNeg(v.vertex.x);
			// now xVertex has values from -1 + 1
			v.vertex.x =   remap( remapedXVertex  + v.vertex.y*(1+(pow(v.vertex.y ,2))) * (yBendStrength*2));

			// calculating normals
			o.normal = v.normal = calculateNormal(v.vertex.y, v.vertex.z);
		}

		void grass_surf (Input IN, inout SurfaceOutputStandard o, fixed4 l_Color) {
			// IN.objectSpacePos.x  is from -0.5 to 0.5. We change it to from 0 to 1, and then multiply
			half fixedXPos = (IN.objectSpacePos.x + 0.5) ;
			// now it has values from 0 to 1
			fixed4 c = l_Color * 0.9 + 0.2* (1-pow(abs((fixedXPos*2-1)),0.3));
			//c.x = dot(IN.viewDir, float3(0, 0, 1));

			o.Albedo = c;
			o.Emission = half3(0,0,0);
			o.Metallic = 0.8;
			o.Smoothness = 0.8;

			o.Normal = IN.normal;
		}

#endif