using Blazor.Extensions;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MandelBrotWasm.Logic
{
    public abstract class MandelbrotRenderContext
    {
        public event EventHandler<float> OnProgress;
        public event EventHandler<EventArgs> OnPresent;

        public SetType SetType { get; set; } = SetType.Mandelbrot;
        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;
        public double Scale
        {
            get => _scale;
            set => _scale = double.Max(0.0000001, value);
        }
        public int MaxIterations
        {
            get => _maxIterations;
            set => _maxIterations = int.Max(1, value);
        }

        public int Width { get => _width; }
        public int Height { get => _height; }

        private double _scale = 1.0;
        private int _maxIterations = 50;

        protected int _width;
        protected int _height;
        protected BECanvasComponent BECanvasComponent { get; set; }
        protected IJSRuntime JSRuntime { get; set; }
        protected Image<Rgba32>? lastRender { get; set; }
        protected ComplexPlaneInfo? lastComplexPlane { get; set; }

        public MandelbrotRenderContext(BECanvasComponent beCanvasComponent, IJSRuntime jSRuntime)
        {
            BECanvasComponent = beCanvasComponent;
            JSRuntime = jSRuntime;

            _width = (int)BECanvasComponent.Width;
            _height = (int)BECanvasComponent.Height;
        }

        public abstract Task Render();

        public async Task Preview()
        {
            if (lastRender == null || !lastComplexPlane.HasValue)
            {
                return;
            }

            ComplexPlaneInfo complexPlaneInfo = GetComplexPlaneInfo();
            int cx = 0;
            int cy = 0;
            float prevProgress = 0;

            Image<Rgba32> image = new Image<Rgba32>(_width, _height);

            for (double x0 = complexPlaneInfo.startX; x0 < complexPlaneInfo.endX; x0 += complexPlaneInfo.stepX)
            {
                int x = (int)(cx + (complexPlaneInfo.startX - lastComplexPlane.Value.startX) / lastComplexPlane.Value.stepX);

                for (double y0 = complexPlaneInfo.startY; y0 < complexPlaneInfo.endY; y0 += complexPlaneInfo.stepY)
                {
                    int y = (int)(cy + (complexPlaneInfo.startY - lastComplexPlane.Value.startY) / lastComplexPlane.Value.stepY);

                    if (x >= 0 && y >= 0 && x < _width && y < _height)
                    {
                        if (cx < _width && cy < _height)
                            image[cx, cy] = lastRender[x, y];
                    }
                    else
                    {
                        if (cx < _width && cy < _height)
                            image[cx, cy] = new Rgba32(255, 255, 255, 255);
                    }

                    cy++;
                }

                cx++;
                cy = 0;

                float progress = (float)(cx * _height + cy) / (_width * _height);
                if (progress - prevProgress > 0.1)
                {
                    await Task.Delay(1);
                    prevProgress = progress;
                    SetProgress(progress);
                }
            }

            string imgBase64 = image.ToBase64String(SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);

            await JSRuntime.InvokeVoidAsync("renderImgData", imgBase64);
            Present();
        }

        public virtual async Task ResizeAsync()
        {
            BodySize bodySize = await JSRuntime.InvokeAsync<BodySize>("getBodySize");
            _width = (int)bodySize.Width;
            _height = (int)bodySize.Height;
        }

        public virtual void Resize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void AddOffsetPx(double offsetX, double offsetY)
        {
            OffsetX += offsetX / _width / _scale;
            OffsetY += offsetY / _height / _scale;
        }

        public void AddScale(double scale)
        {
            Scale += scale;
        }

        protected ComplexPlaneInfo GetComplexPlaneInfo() => new ComplexPlaneInfo(_width, _height, Scale, OffsetX, OffsetY);

        protected void Present()
        {
            OnPresent?.Invoke(this, EventArgs.Empty);
        }

        protected void SetProgress(float progress)
        {
            OnProgress?.Invoke(this, progress);
        }
    }
}
