#ifndef NOISE_INC
#define NOISE_INC

#include "common.inc"

// return from 0 to 1
half rand(half seed)
{
	return frac(sin( seed ) * 43758.5453);
}

// returns both values from -1 to 1
half2 randNorm(half2 seed)
{
	return half2(rand(seed.x*3123.13 + seed.y*574.1231), rand(seed.y*3123.13 + seed.x*574.1231));
}

// returns both values from -1 to 1
half2 rand(half2 seed)
{
	return 2*randNorm(seed)-1;
}

half rand2(half2 p)
{
	return frac(sin(half2(dot(p,half2(127.1,311.7)),dot(p,half2(269.5,183.3))))*43758.5453);
}

// zwraca od 0 do 1
half noise1D( half seed ){
	return lerp(rand(floor(seed)), rand(ceil(seed)), frac(seed)); 
}

#define GEN_fractalNoise( NAME, OCTAVES, NOISE_FUNCTION, MIN, MAX) \
half NAME (in half2 pos) { \
    half value = 0.0; \
    half amplitude = 0.5; \
    half frequency = 0.0; \
    for (int i = 0; i < OCTAVES; i++) { \
        value += amplitude * NOISE_FUNCTION(pos); \
        pos *= 2.0; \
        amplitude *= 0.5; \
    } \
    return (value-MIN)/(MAX-MIN); \
}

// 2D Noise based on Morgan McGuire @morgan3d //TODO zmień kod bo jest kradziony
// https://www.shadertoy.com/view/4dS3Wd
half simpleValueNoise2D (in half2 st) {
    half2 i = floor(st);
    half2 f = frac(st);

    // Four corners in 2D of a tile
    half a = rand2(i);
    half b = rand2(i + half2(1.0, 0.0));
    half c = rand2(i + half2(0.0, 1.0));
    half d = rand2(i + half2(1.0, 1.0));

    // Smooth Interpolation

    // Cubic Hermine Curve.  Same as SmoothStep()
    half2 u = smoothstep(half2(0,0),half2(1,1),f);
    // u = smoothstep(0.,1.,f);

    // Mix 4 coorners porcentages
    return lerp(a, b, u) + 
            (c - a)* u.y * (1.0 - u.x) + 
            (d - b) * u.x * u.y;
}

GEN_fractalNoise( fractal_simpleValueNoise2D_3, 3, simpleValueNoise2D, 0.05, 0.8)

// https://thebookofshaders.com/11/ inna funkcja smoothstep
half improvedValueNoise2D (in half2 st) {
    half2 i = floor(st);
    half2 f = frac(st);

    // Four corners in 2D of a tile
    half a = rand2(i);
    half b = rand2(i + half2(1.0, 0.0));
    half c = rand2(i + half2(0.0, 1.0));
    half d = rand2(i + half2(1.0, 1.0));

    // Smooth Interpolation

    half2 u = 6*pow(f,5) - 15*pow(f,4)+10*pow(f,3);

    // Mix 4 coorners porcentages
    return lerp(a, b, u) + 
            (c - a)* u.y * (1.0 - u.x) + 
            (d - b) * u.x * u.y;
}

GEN_fractalNoise( fractal_improvedValueNoise2D_3, 3, improvedValueNoise2D,0.05,0.8)


// gradient Perlin noise // todo wziete z https://www.shadertoy.com/view/XdXGW8
float simplePerlinNoise2D( in half2 p )
{
    half2 i = floor( p );
    half2 f = frac( p );
	
	half2 u = f*f*(3.0-2.0*f);

    float v =	 lerp(lerp(	dot( rand( i + half2(0,0) ), f - half2(0,0) ), 
							dot( rand( i + half2(1,0) ), f - half2(1,0) ), float(u.x)),
					lerp(	dot( rand( i + half2(0,1) ), f - half2(0,1) ), 
							dot( rand( i + half2(1,1) ), f - half2(1,1) ), float(u.x)), float(u.y));
	return remap(v);
}

GEN_fractalNoise( fractal_simplePerlinNoise2D_3, 3, simplePerlinNoise2D, 0.3, 0.55)

// Some useful functions
half3 mod289(half3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
half2 mod289(half2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
half3 permute(half3 x) { return mod289(((x*34.0)+1.0)*x); }

//
// Description : GLSL 2D simplex noise function
//      Author : Ian McEwan, Ashima Arts
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : 
//  Copyright (C) 2011 Ashima Arts. All rights reserved.
//  Distributed under the MIT License. See LICENSE file.
//  https://github.com/ashima/webgl-noise
//	https://thebookofshaders.com/edit.php#11/2d-snoise-clear.frag
//  TODO zrob do kodu w pracy odniesienie z kad masz to
// simplex gradient noise
float snoise2D(half2 v) {
    // Precompute values for skewed triangular grid
    const half4 C = half4(0.211324865405187,
                        // (3.0-sqrt(3.0))/6.0
                        0.366025403784439,  
                        // 0.5*(sqrt(3.0)-1.0)
                        -0.577350269189626,  
                        // -1.0 + 2.0 * C.x
                        0.024390243902439); 
                        // 1.0 / 41.0

    // First corner (x0)
    half2 i  = floor(v + dot(v, C.yy));
    half2 x0 = v - i + dot(i, C.xx);

    // Other two corners (x1, x2)
    half2 i1 = half2(0.0, 0.0);
    i1 = (x0.x > x0.y)? half2(1.0, 0.0):half2(0.0, 1.0); //wybor górnego algo dolnego trójkąta
    half2 x1 = x0.xy + C.xx - i1;
    half2 x2 = x0.xy + C.zz;

    // Do some permutations to avoid
    // truncation effects in permutation
    i = mod289(i);
    half3 p = permute(
            permute( i.y + half3(0.0, i1.y, 1.0))
                + i.x + half3(0.0, i1.x, 1.0 ));

    half3 m = max(0.5 - half3(
                        dot(x0,x0), //na sobie sanym, czyli do kwadratu
                        dot(x1,x1), 
                        dot(x2,x2)
                        ), 0.0);

    m = m*m ;
    m = m*m ;

    // Gradients: 
    //  41 pts uniformly over a line, mapped onto a diamond
    //  The ring size 17*17 = 289 is close to a multiple 
    //      of 41 (41*7 = 287)

    half3 x = 2.0 * frac(p * C.www) - 1.0; // c.www is 1/41
    // x is from -1 to 1
    half3 h = abs(x) - 0.5; // h is from -0.5 to 0.5
    half3 ox = floor(x + 0.5); // ox is -1 or 0 or 1
    half3 a0 = x - ox; // so a0 is from 0 to 1

    // Normalise gradients implicitly by scaling m
    // Approximation of: m *= inversesqrt(a0*a0 + h*h);
    m *= 1.79284291400159 - 0.85373472095314 * (a0*a0+h*h);

    // Compute final noise value at P
    half3 g = half3(0.0, 0.0, 0.0);
    g.x  = a0.x  * x0.x  + h.x  * x0.y;
    g.yz = a0.yz * half2(x1.x,x2.x) + h.yz * half2(x1.y,x2.y);
    return remap(130.0 * dot(m, g))/1.2;
}


#endif 