precision highp float;

uniform vec2 u_start;
uniform vec2 u_end;
uniform int u_maxIterations;

varying vec3 v_coord;

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main()
{
    vec2 v_n = (v_coord.xy + vec2(1.0)) / 2.0 * (u_end - u_start);
    vec2 c0 = u_start + v_n;
    vec2 c2= vec2(0.0);
    float w = 0.0;
    float i = 0.0;
    float smoothI = 0.0;

    for(int j = 0; j < 10000; j++){
         if(c2.x + c2.y < 4.0 && i < float(u_maxIterations))
         {
            float x = c2.x - c2.y + c0.x;
            float y = w-c2.x-c2.y+c0.y;

            c2.x = x * x;
            c2.y = y * y;

            w = (x + y) * (x + y);

            i += 1.0;
         }else 
         {
             break;
         }
    }

     if (i < float(u_maxIterations)) {
        float logZn = log(c2.x * c2.x + c2.y * c2.y) / 2.0;
        float nu = log(logZn / log(2.0)) / log(2.0);
        smoothI = i + 1.0 - nu;
    } else {
        smoothI = float(u_maxIterations);
    }

    float r = smoothI / float(u_maxIterations);

    vec3 hsv = vec3(0.7 - r * 0.7,1.0,smoothstep(0.01,0.5,1.0-r));

    /*if(r == 1.0){
        hsv = vec3(0.0,0.0,0.0);
    }*/

    gl_FragColor = vec4(hsv2rgb(hsv),1.0);
} 