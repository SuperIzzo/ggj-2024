shader_type spatial;

uniform sampler2D SCREEN_TEXTURE: hint_screen_texture, filter_linear_mipmap;

uniform vec2 amp = vec2(0.2, 0.1);
uniform float freq = 3.0;
uniform vec2 worldTime = vec2(3.0,2.5);
uniform vec3 color = vec3(1.0,1.0,1.0);


float wave(vec2 pos, float time){
	return (amp.x  * sin(pos.x * freq + time * worldTime.x)) + (amp.y * sin(pos.y * freq + time * worldTime.x));
}


void vertex(){
	VERTEX.x += wave(VERTEX.xz, TIME);
}

void fragment() {
   
vec2 uv = UV;
	vec2 pos = vec2(0.5,0.5);
	float x = length(pos-uv);
	x = sin(x * 10.0 * TIME);
		
	float yeet = smoothstep(0.0, 1.0, sin(TIME));
	float yeet2 = smoothstep(0.0, 1.0, cos(TIME));
	float yeet3 = smoothstep(0.0, 1.0, sin(TIME)*-1.0);
	
	vec3 albedo = vec3(color.x * yeet, color.y * yeet2, color.z * yeet3) * abs(x);
   
	ALBEDO = albedo;
	
}
