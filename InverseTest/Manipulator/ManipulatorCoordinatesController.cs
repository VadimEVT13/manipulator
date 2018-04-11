using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InverseTest.ManipulatorV2;

namespace InverseTest.Manipulator
{


    /// <summary>
    /// Переводит углы большого манипулятора в углы трехмерной модели манипулятора
    /// </summary>


    public class ManipulatorCoordinatesController
    {

        private const double T1START_ANGLE = 0;
        private const double T2START_ANGLE = -90;
        private const double T3START_ANGLE = 90;
        private const double T4START_ANGLE = 90;
        private const double T5START_ANGLE = 90;

        public ManipulatorCoordinatesController(){}



        /// <summary>
        /// Переводит угол поворота ребрка T1 относительно начального положения 
        /// в угол онтосителльно начального положения трехмерной модели манипулятора
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public double T1GlobalToLocal(double angle)
        {
            return -(angle - T1START_ANGLE);
        }

        public double T2GlobalToLocal(double angle)
        {
             return -angle-T2START_ANGLE;
        }

        public double T3GlobalToLocal(double angle)
        {
            return (angle - T3START_ANGLE );
        }

        public double T4GlobalToLocal(double angle)
        {
            return angle;
        }

        public double T5GlobalToLocal(double angle)
        {
            return (angle - T5START_ANGLE);
        }



        /// <summary>
        /// Переводит угол поворота ребрка T1 относительно положения трехмерной модели манипулятора 
        /// в угол онтосителльно начального положения манипулятора
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public double T1LocalToGlobal(double angle)
        {
            return -angle;
        }

        public double T2LocalToGlobal(double angle)
        {
            return (-angle + T2START_ANGLE);
        }

        public double T3LocalToGlobal(double angle)
        {
            return (angle + T3START_ANGLE);
        }

        public double T4LocalToGlobal(double angle)
        {
            return angle;
        }

        public double T5LocalToGlobal(double angle)
        {
            return (angle + T5START_ANGLE);
        }
    }
}
