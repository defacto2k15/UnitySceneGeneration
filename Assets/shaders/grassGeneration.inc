#include "noise.inc"
#include "common.inc"

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
float calculateXBendStrength( half inWindValue, half4 windDirection, half4 plantDirection, half initialBendingValue, half plantBendingStiffness ){
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
	half directionCoefficient = sqrt(generateDirectionCoef( windDirection, plantDirection.zyxw)); //rotated as we check if wind is blowing on side
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
	return normalize(max(inVal, half4(0.01, 0.01, 0.01, 0)));
}

// returns values from -1 to 1
half generateRandomWindMoveMultiplier( half bendingStrength, half randSeed ){
	half currentNoiseValue = noise1D(_Time[3]);
	half randomSinusDegreeOffsetAccordingToPosition = randSeed*10;
	half randomNoiseMultiplier = lerp(0.8, 1.2, currentNoiseValue);
	half bendStrengthMultiplier = lerp(0.4, 0.6, sin(pow(bendingStrength,2) * fPI()));  
	half frequencyMultiplier = lerp(0.5, 1.5, pow(bendingStrength,2));

	return  sin(_Time[3] * frequencyMultiplier * 2 + randomSinusDegreeOffsetAccordingToPosition*30) * randomNoiseMultiplier*bendStrengthMultiplier ;
}

// first element is xStrength, second is yStrength. Each strength has values from -1 to 1
half2 generateStrengths( half l_BendingStrength, half l_InitialBendingValue, half l_PlantBendingStiffness, half4 l_WindDirection, half4 l_PlantDirection, half l_RandSeed){
		l_WindDirection = assertNotZero(l_WindDirection);
		l_PlantDirection = assertNotZero(l_PlantDirection);

		half randomWindMultiplier =  generateRandomWindMoveMultiplier(l_BendingStrength, l_RandSeed);
		half xBendStrength = remapNeg(calculateXBendStrength(l_BendingStrength, l_WindDirection, l_PlantDirection, l_InitialBendingValue, l_PlantBendingStiffness));
		xBendStrength *= ( 1 + 0.2* randomWindMultiplier ); 

		half yBendStrength = remapNeg(calculateYBendStrength(l_BendingStrength, l_WindDirection, l_PlantDirection));
		yBendStrength *= ( 1 + 0.2* randomWindMultiplier ); 
		return half2( xBendStrength, yBendStrength);
} 

half calculateZ( half xBendStrength, half l ){
	return xBendStrength*(  (sin(l*fPI()/2-0.5*fPI())+1)/2)  * (1 + (sin(fPI()/2 * abs(xBendStrength))))*0.9;
}

half calculateY (half xBendStrength, half l){
	return l *  cos(lerp( 0,fPI()/2,  abs(xBendStrength)))*0.9;
}

half calculateX(half yBendStrength, half vertexX, half vertexY ){
	half remapedXVertex = remapNeg(vertexX);
	// now xVertex has values from -1 + 1
	return remap( remapedXVertex  + vertexY*(1+(pow(vertexY ,2))) * (yBendStrength*2));
}

#endif
