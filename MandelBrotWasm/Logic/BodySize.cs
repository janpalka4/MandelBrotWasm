namespace MandelBrotWasm.Logic
{
    public struct BodySize
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public BodySize(double width, double height)
        {
            Width = width;
            Height = height;
        }
        public BodySize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public BodySize()
        {
        }
    }
}
