#version 330 core

in vec2 ndc;
out vec4 fragColor;

uniform vec2 resolution;
uniform float time;
uniform int iter;
uniform float zoom;
uniform vec2 cameraPosition;
uniform vec3 color;

vec2 squareImaginary(vec2 number)
{
	return vec2(pow(number.x, 2) - pow(number.y, 2), 2 * number.x * number.y);
}

float iterateMandelbrot(vec2 coord)
{
    coord *= 1 / zoom;
    coord += cameraPosition;
	vec2 z = vec2(0,0);
	for(int i = 0; i < iter; i++)
    {
		z = squareImaginary(z) + coord;
        float iFloat = i;
        float iterFloat = iter;
		if(length(z) > 2) return iFloat / iterFloat;
	}
	return 1;
}

void main()
{
    float aspect = resolution.x / resolution.y;
    vec2 uv = vec2(ndc.x, ndc.y / aspect);
	fragColor = vec4(color * iterateMandelbrot(uv), 1);
}