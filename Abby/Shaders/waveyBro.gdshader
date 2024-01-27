shader_type spatial;

uniform sampler2D SCREEN_TEXTURE: hint_screen_texture, filter_linear_mipmap;

uniform vec2 amp = vec2(0.2, 0.1);
uniform float freq = 3.0;
uniform vec2 worldTime = vec2(3.0,2.5);
uniform vec3 color = vec3(1.0,1.0,1.0);

//color
uniform float speed = 0.6;
uniform float ring_scale = 2.4;
uniform float wave_scale = 8.454;
uniform float random_scale = 4.6;
uniform float noise_scale = 0.03;

vec3 hsv2rgb( float h, float s, float v )
{
	return (
		(
			clamp(
				abs( fract( h + vec3( 0.0, 2.0, 1.0 ) / 3.0 ) * 6.0 - 3.0 ) - 1.0
			,	0.0
			,	1.0
			) - 1.0
		) * s + 1.0
	) * v;
}

vec2 random( vec2 pos )
{ 
	return fract(
		sin(
			vec2(
				dot(pos, vec2(12.9898,78.233))
			,	dot(pos, vec2(-148.998,-65.233))
			)
		) * 43758.5453
	);
}

float value_noise( vec2 pos )
{
	vec2 p = floor( pos );
	vec2 f = fract( pos );

	float v00 = random( p + vec2( 0.0, 0.0 ) ).x;
	float v10 = random( p + vec2( 1.0, 0.0 ) ).x;
	float v01 = random( p + vec2( 0.0, 1.0 ) ).x;
	float v11 = random( p + vec2( 1.0, 1.0 ) ).x;

	vec2 u = f * f * ( 3.0 - 2.0 * f );

	return mix( mix( v00, v10, u.x ), mix( v01, v11, u.x ), u.y );
}

float wave(vec2 pos, float time){
	return (amp.x  * sin(pos.x * freq + time * worldTime.x)) + (amp.y * sin(pos.y * freq + time * worldTime.x));
}


void vertex(){
	VERTEX.x += wave(VERTEX.yz, TIME);
}

void fragment() {
   
    //vec2 uv = UV * 0.2;
	//vec2 pos = vec2(0.5,0.5);
	//float x = length(pos-uv);
	///x = sin(x * 3.0 * TIME);
	//x = abs(x);
 
  
		
	float yeet = smoothstep(0.0, 1.0, sin(TIME));
	float yeet2 = smoothstep(0.0, 1.0, cos(TIME));
	float yeet3 = smoothstep(0.0, 1.0, sin(TIME)*-1.0);
	
	//vec3 albedo = vec3(color.x * yeet, color.y * yeet2, color.z * yeet3) * x;
	
    vec2 shift_uv = SCREEN_UV;
	shift_uv.x += value_noise( UV * random_scale + sin( TIME * speed ) );
	shift_uv.y += sin( cos( TIME ) * speed );
	float x = shift_uv.x + sin( shift_uv.y * wave_scale );
	float f = mod( x * ring_scale + random( UV ).x * noise_scale, 1.0 );

	ALBEDO = hsv2rgb( f, 1.0, 1.0 );
   
	
	
}
