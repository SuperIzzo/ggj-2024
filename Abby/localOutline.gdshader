shader_type spatial;
render_mode unshaded;

// higher values are smaller width.
uniform float outline_width : hint_range(0.0, 2.0, 0.1) = 0.6;

uniform vec4 base_color = vec4(1.0, 0.0, 1.0, 1.0);

float fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), amount);
}

void fragment()
{
	vec2 uv = SCREEN_UV;
	vec2 pos = vec2(0.5,0.5);
	float x = length(pos-uv);
	
	
	float basic_fresnel = fresnel(outline_width, NORMAL, VIEW);
	basic_fresnel = step(0.5, basic_fresnel);
	
	ALBEDO = base_color.rgb * x - basic_fresnel;
}