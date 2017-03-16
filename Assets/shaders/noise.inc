#ifndef NOISE_INC
#define NOISE_INC
half rand(half seed)
{
	return frac(sin( seed ) * 43758.5453);
}

// zwraca od 0 do 1
half noise1D( half seed ){
	return lerp(rand(floor(seed)), rand(ceil(seed)), frac(seed)); 
}
#endif 
