namespace MandelBrotWasm.Logic
{
    public class RenderRequestedEventArgs : EventArgs
    {
        public int MaxIterations { get; set; } = 50;
        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;
        public double Zoom { get; set; } = 1.0;
    }
}
