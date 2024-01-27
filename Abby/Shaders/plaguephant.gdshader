shader_type spatial;
// render_mode;

uniform vec4 light_color : source_color = vec4( 0.529411764705882, 0.95683485641634156, 0.1563153113511153, 1.0 );
uniform vec4 dark_color : source_color = vec4( 0.196078431372549, 0.4196078431372549, 0.196078431372549, 1.0 );

uniform float front_random_scale = 1.6;
uniform float back_random_scale = -6.12;
uniform float front_marble_scale = 15.4;
uniform float back_marble_scale = 20.1;

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

float noise_tex( vec2 p )
{
	return (
		value_noise( p * 0.984864 ) * 0.5
	+	value_noise( p * 2.543 ) * 0.25
	+	value_noise( p * 9.543543 ) * 0.125
	+	value_noise( p * 21.65436 ) * 0.0625
	+	value_noise( p * 42.0 ) * 0.03125
	+	value_noise( p * 87.135148 ) * 0.015625
	+	value_noise( p * 340.66534654 ) * 0.0078125
	);
}

void fragment( )
{
	float a = noise_tex( UV * front_random_scale );
	float b = noise_tex( UV * back_random_scale );

	ALBEDO = mix( dark_color, light_color, clamp( abs( cos( a * front_marble_scale ) + sin( b * back_marble_scale ) * 0.4 ), 0.0, 1.0 ) + sin(TIME) ).rgb;
	ALPHA = 0.6 * dark_color.y;
}