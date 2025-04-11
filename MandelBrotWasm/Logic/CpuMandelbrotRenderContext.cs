﻿using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MandelBrotWasm.Logic
{
    public class CpuMandelbrotRenderContext : MandelbrotRenderContext
    {
        private Canvas2DContext _context;

        public CpuMandelbrotRenderContext(BECanvasComponent beCanvasComponent,IJSRuntime jSRuntime) : base(beCanvasComponent, jSRuntime)
        {
        }

        public override async Task Render()
        {
            if(_context == null)
            {
                _context = await BECanvasComponent.CreateCanvas2DAsync();
            }

            await ResizeAsync();

            Image<Rgba32> image = new Image<Rgba32>(_width, _height);
            //Resize(_width / 4, _height / 4);

            ComplexPlaneInfo complexPlaneInfo = GetComplexPlaneInfo();
            int cx = 0;
            int cy = 0;
            float prevProgress = 0;

            for (double x0 = complexPlaneInfo.startX; x0 < complexPlaneInfo.endX; x0 += complexPlaneInfo.stepX)
            {
                for (double y0 = complexPlaneInfo.startY; y0 < complexPlaneInfo.endY; y0 += complexPlaneInfo.stepY)
                {
                    double x2 = 0;
                    double y2 = 0;
                    double w = 0;
                    int i = 0;

                    while(x2 + y2 < 4 && i < MaxIterations)
                    {
                        double x = x2 - y2 + x0;
                        double y = w - x2 - y2 + y0;

                        x2 = x * x;
                        y2 = y * y;

                        w = (x + y) * (x + y);

                        i++;
                    }

                    if (cx < _width && cy < _height)
                    {
                        byte r = (byte)(255 * (1.0 - i / (float)MaxIterations));
                        image[cx, cy] = new Rgba32(r,r,r,255);
                    }

                    cy++;


                    float progress = (float)(cx * _height + cy) / (_width * _height);
                    if(progress - prevProgress > 0.0025)
                    {
                        await Task.Delay(1);
                        prevProgress = progress;
                        SetProgress(progress);
                    }
                }

                cy = 0;
                cx++;
            }

            lastRender = image;
            lastComplexPlane = complexPlaneInfo;

            string imgBase64 = image.ToBase64String(SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);

            await JSRuntime.InvokeVoidAsync("renderImgData", imgBase64);

            Present();
        }
    }
}
