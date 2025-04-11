using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;

namespace MandelBrotWasm.Logic
{
    public class GpuMandelbrotRenderContext : MandelbrotRenderContext
    {
        private WebGLContext _context;
        private WebGLProgram _shaderProgram;
        private WebGLBuffer _vertexBuffer;

        private float[] _screenVertices = {-1,-1,0,  1,-1,0,  1,1,0,  -1,1,0};
        
        private WebGLUniformLocation _uniformLocationStart;
        private WebGLUniformLocation _uniformLocationEnd;

        public GpuMandelbrotRenderContext(BECanvasComponent beCanvasComponent,IJSRuntime jSRuntime) : base(beCanvasComponent, jSRuntime)
        {
        }

        public override async Task Render()
        {
            if(_context == null)
            {
                await InitializeWebGL();
            }

            await ResizeAsync();
            ComplexPlaneInfo complexPlane = GetComplexPlaneInfo();


            // Render Mandelbrot set using WebGL
            await _context!.ClearAsync(BufferBits.COLOR_BUFFER_BIT);

            await _context.UseProgramAsync(_shaderProgram);
            await _context.UniformAsync(_uniformLocationStart, (float)complexPlane.startX, (float)complexPlane.startY);
            await _context.UniformAsync(_uniformLocationEnd, (float)complexPlane.endX, (float)complexPlane.endY);

            await _context.BindBufferAsync(BufferType.ARRAY_BUFFER, _vertexBuffer);
            
            await _context.DrawArraysAsync(Primitive.TRIANGLE_FAN, 0, 4);

            Present();
        }

        private async Task InitializeWebGL()
        {
            // Initialize WebGL context and shaders here
            // Load shaders, create buffers, etc.
            _context = await BECanvasComponent.CreateWebGLAsync();

            await _context.ViewportAsync(0, 0, _width, _height);
            await _context.ClearColorAsync(0, 0, 0, 1);

            // Load and compile shaders
            string vertexShaderSource = await LoadShader("VertexShader.glsl");
            string fragmentShaderSource = await LoadShader("FragmentShader.glsl");
            WebGLShader vertexShader = await CompileShader(vertexShaderSource, ShaderType.VERTEX_SHADER);
            WebGLShader fragmentShader = await CompileShader(fragmentShaderSource, ShaderType.FRAGMENT_SHADER);

            // Create and link shader program
            _shaderProgram = await _context.CreateProgramAsync();
            await _context.AttachShaderAsync(_shaderProgram, vertexShader);
            await _context.AttachShaderAsync(_shaderProgram, fragmentShader);
            await _context.LinkProgramAsync(_shaderProgram);

            string programLog = await _context.GetProgramInfoLogAsync(_shaderProgram);
            if (!string.IsNullOrEmpty(programLog))
            {
                throw new Exception($"Program link error: {programLog}");
            }

            // Get uniform locations
            _uniformLocationStart = await _context.GetUniformLocationAsync(_shaderProgram, "u_start");
            _uniformLocationEnd = await _context.GetUniformLocationAsync(_shaderProgram, "u_end");

            // Set attribute locations
            uint positionLocation = (uint)await _context.GetAttribLocationAsync(_shaderProgram, "a_position");

            // Screen vertex buffer
            _vertexBuffer = await _context.CreateBufferAsync();
            await _context.BindBufferAsync(BufferType.ARRAY_BUFFER, _vertexBuffer);
            await _context.BufferDataAsync(BufferType.ARRAY_BUFFER, _screenVertices, BufferUsageHint.STATIC_DRAW);

            await _context.EnableVertexAttribArrayAsync(positionLocation);
            await _context.VertexAttribPointerAsync(positionLocation, 3, DataType.FLOAT, false, 0, 0);

            await _context.BindBufferAsync(BufferType.ARRAY_BUFFER, null); 
        }

        private async Task<WebGLShader> CompileShader(string source, ShaderType shaderType)
        {
            WebGLShader shader = await _context.CreateShaderAsync(shaderType);
            await _context.ShaderSourceAsync(shader, source);
            await _context.CompileShaderAsync(shader);

            string shaderLog = await _context.GetShaderInfoLogAsync(shader);
            if (!string.IsNullOrEmpty(shaderLog))
            {
                Console.Error.WriteLine($"{shaderType}: Shader compilation error: {shaderLog}");
                throw new Exception($"{shaderType}: Shader compilation error: {shaderLog}");
            }

            return shader;
        }

        private async Task<string> LoadShader(string shaderName)
        {
            // Load shader source code from file
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"{assembly.GetName().Name}.Resources.{shaderName}";

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Shader resource '{resourceName}' not found.");
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
