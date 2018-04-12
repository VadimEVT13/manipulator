namespace InverseTest.Manipulator
{
    /// <summary>
    /// Контроллер преобразования глобальных координат модели в локальные координаты манипулятора.
    /// </summary>
    public static class ManipulatorPositionController
    {
        /// <summary>
        /// Начальное положение первого колена.
        /// </summary>
        private static double T1_POSITION = 90;
        /// <summary>
        /// Начальное положение второго колена.
        /// </summary>
        private static double T2_POSITION = 90;
        /// <summary>
        /// Начальное положение третьего колена.
        /// </summary>
        private static double T3_POSITION = 90;
        /// <summary>
        /// Начальное положение четвертого колена.
        /// </summary>
        private static double T4_POSITION = 0;
        /// <summary>
        /// Начальное положение пятого колена.
        /// </summary>
        private static double T5_POSITION = 90;

        /// <summary>
        /// Преобразование положения первого колена из глобального в локальное.
        /// </summary>
        /// <param name="angle">глобальная координата</param>
        /// <returns>локальная координата</returns>
        public static double T1GlobalToLocal(double angle)
        {
            return angle + T1_POSITION;
        }

        /// <summary>
        /// Преобразование положения первого колена из локального в глобальное.
        /// </summary>
        /// <param name="angle">локальная координата</param>
        /// <returns>глобальная координата</returns>
        public static double T1LocalToGlobal(double angle)
        {
            return angle - T1_POSITION;
        }

        /// <summary>
        /// Преобразование положения второго колена из глобального в локальное.
        /// </summary>
        /// <param name="angle">глобальная координата</param>
        /// <returns>локальная координата</returns>
        public static double T2GlobalToLocal(double angle)
        {
             return -angle + T2_POSITION;
        }

        /// <summary>
        /// Преобразование положения второго колена из локального в глобальное.
        /// </summary>
        /// <param name="angle">локальная координата</param>
        /// <returns>глобальная координата</returns>
        public static double T2LocalToGlobal(double angle)
        {
            return -(angle - T2_POSITION);
        }

        /// <summary>
        /// Преобразование положения третьего колена из глобального в локальное.
        /// </summary>
        /// <param name="angle">глобальная координата</param>
        /// <returns>локальная координата</returns>
        public static double T3GlobalToLocal(double angle)
        {
            return angle + T3_POSITION;
        }

        /// <summary>
        /// Преобразование положения третьего колена из локального в глобальное.
        /// </summary>
        /// <param name="angle">локальная координата</param>
        /// <returns>глобальная координата</returns>
        public static double T3LocalToGlobal(double angle)
        {
            return angle - T3_POSITION;
        }

        /// <summary>
        /// Преобразование положения четвертого колена из глобального в локальное.
        /// </summary>
        /// <param name="angle">глобальная координата</param>
        /// <returns>локальная координата</returns>
        public static double T4GlobalToLocal(double angle)
        {
            return angle - T4_POSITION;
        }

        /// <summary>
        /// Преобразование положения четвертого колена из локального в глобальное.
        /// </summary>
        /// <param name="angle">локальная координата</param>
        /// <returns>глобальная координата</returns>
        public static double T4LocalToGlobal(double angle)
        {
            return angle + T4_POSITION;
        }

        /// <summary>
        /// Преобразование положения пятого колена из глобального в локальное.
        /// </summary>
        /// <param name="angle">глобальная координата</param>
        /// <returns>локальная координата</returns>
        public static double T5GlobalToLocal(double angle)
        {
            return angle + T5_POSITION;
        }

        /// <summary>
        /// Преобразование положения пятого колена из локального в глобальное.
        /// </summary>
        /// <param name="angle">локальная координата</param>
        /// <returns>глобальная координата</returns>
        public static double T5LocalToGlobal(double angle)
        {
            return angle - T5_POSITION;
        }
    }
}
