shader_type spatial;
render_mode unshaded;

uniform vec4 albedo : source_color = vec4(1.0);
//uniform vec4 wire_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float wire_width : hint_range(0.0, 40.0) = 5.0;
uniform float wire_smoothness : hint_range(0.0, 0.1) = 0.01;

varying vec3 barys;

vec3 palette( float t)
{
    vec3 a = vec3(0.848, 0.500, 0.588);
    vec3 b = vec3(0.718, 0.500, 0.500);
    vec3 c = vec3(0.750, 1.000, 0.667);
    vec3 d = vec3(-0.082, -0.042, 0.408);
    
    return a + b*cos( 6.28318*(c*t+d) );
}

void vertex() {
	
	
	
	int index = VERTEX_ID % 3;
	switch (index) {
		case 0:
			barys = vec3(1.0, 0.0, 0.0);
			break;
		case 1:
			barys = vec3(0.0, 1.0, 0.0);
			break;
		case 2:
			barys = vec3(0.0, 0.0, 1.0);
			break;
	}
}

void fragment()
{
	vec2 uv = fract(UV * 2.);
	vec2 pos = vec2(0.5,0.5);
	float x = length(pos-uv);

	vec3 col = palette(length(uv));
	
	vec3 z = vec3(0.0);
	x = sin(x * 12. + TIME) / 2.;    
	x = abs(x);    
    x = pow(0.1 / x, 1.5);
	
	z += col * x;
	
	vec4 wire_color = vec4(z * 2., 1.0);
	
	vec3 deltas = fwidth(barys / 1.8);
	vec3 barys_s = smoothstep(deltas * wire_width - wire_smoothness, deltas * wire_width + wire_smoothness, barys);
	float wires = min(barys_s.x, min(barys_s.y, barys_s.z));
	ALBEDO = mix(wire_color.rgb, z, wires);
	ALPHA = 0.6;
}