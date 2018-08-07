/// <summary>
/// Модель данных - точка.
/// </summary>
namespace InverseTest.Grbl.Models
{
    public class GPoint
    {
        /// <summary>
        /// Позиция по оси X.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Позиция по оси Y.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Позиция по оси Z.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Позиция по оси A.
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Позиция по оси B.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// Позиция по оси C.
        /// </summary>
        public double C { get; set; }

        /// <summary>
        /// Позиция по оси D.
        /// </summary>
        public double D { get; set; }

        /// <summary>
        /// Позиция по оси E.
        /// </summary>
        public double E { get; set; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GPoint()
        {
            X = 0;
            Y = 0;
            Z = 0;
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            E = 0;
        }
    }
}
