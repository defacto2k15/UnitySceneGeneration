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
#endif