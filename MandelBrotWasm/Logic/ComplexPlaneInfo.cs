namespace MandelBrotWasm.Logic
{
    public struct ComplexPlaneInfo
    {
        public double startX;
        public double startY;
        public double endX;
        public double endY;
        public double stepX;
        public double stepY;

        public ComplexPlaneInfo(double startX, double startY, double endX, double endY, double stepX, double stepY)
        {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
            this.stepX = stepX;
            this.stepY = stepY;
        }

        public ComplexPlaneInfo(int width, int height, double scale, double offsetX, double offsetY)
        {
            this.startX = -2.0 / scale + offsetX;
            this.startY = -1.12 / scale + offsetY;
            this.endX = 0.47 / scale + offsetX;
            this.endY = 1.12 / scale + offsetY;
            this.stepX = (endX - startX) / width;
            this.stepY = (endY - startY) / height;
        }
    }
}
