precision mediump float;

uniform vec2 u_start;
uniform vec2 u_end;

varying vec3 v_coord;

void main()
{
    vec2 v_n = (v_coord.xy + vec2(1.0)) / 2.0 * (u_end - u_start);
    vec2 c0 = u_start + v_n;
    vec2 c2 = vec2(0.0);
    float w = 0.0;
    float i = 0.0;

    for(int j = 0; j < 50; j++){
         if(c2.x + c2.y < 4.0 && i < 50.0)
         {
            float x = c2.x - c2.y + c0.x;
            float y = w - c2.x - c2.y + c0.y;

            c2.x = x * x;
            c2.y = y * y;

            w = (x + y) * (x + y);

            i += 1.0;
         }else 
         {
             break;
         }
    }

    float r = 1.0 - i / 50.0;

    //gl_FragColor = vec4(v_coord + vec3(0.5),1.0);
    gl_FragColor = vec4(vec3(r),1.0);
} 