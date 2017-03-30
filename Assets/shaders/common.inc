#ifndef COMMON_INC
#define COMMON_INC
	half fPI(){
		return 3.14;
	}

	half remapNeg(half x){ // from (0,1) to (-1,1)
		return (x-0.5)*2;
	}

	half remap(half x){ // from (-1,1) to (0,1)
		return (x+1)/2;
	}

	half4 assertNotZero(half4 inVal){
		return normalize(max(inVal, half4(0.01, 0.01, 0.01, 0)));
	}

	half invLerp(half min, half max, half value){
		return clamp((value - min) / (max-min),0,1);
	}

	half weightedAverage( half4 values, half4 weights ){
		return (values.x * weights.x +
				values.y * weights.y +
				values.z * weights.z +
				values.w * weights.w) / 
					( weights.x + weights.y + weights.z + weights.w);
	}
#endif