attribute vec4 a_position;

varying vec3 v_coord;

void main() {
	v_coord = a_position.xyz;
	gl_Position = a_position;
}