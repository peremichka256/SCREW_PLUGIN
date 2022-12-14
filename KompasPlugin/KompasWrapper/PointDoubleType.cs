namespace Kompas
{
    /// <summary>
    /// Класс точки со значением double
    /// </summary>
    public class PointDoubleType
    {
        /// <summary>
        /// Х координата
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y координата
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="x">Х координата</param>
        /// <param name="y">Y координата</param>
        public PointDoubleType(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}