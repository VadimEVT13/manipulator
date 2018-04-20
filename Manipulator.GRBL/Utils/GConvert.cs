using System;
using System.Text.RegularExpressions;
using Manipulator.GRBL.Models;

/// <summary>
/// Класс для конвертации моделей контроллера.
/// </summary>
namespace Manipulator.GRBL.Utils
{
    public class GConvert
    {
        /// <summary>
        /// Регулярное выражение состояния устройства.
        /// </summary>
        private static Regex statePattern = new Regex("<([A-z]+)"
            + ",MPos:([0-9]+.[0-9]+),([0-9]+.[0-9]+),"
            + "([0-9]+.[0-9]+),([0-9]+.[0-9]+),"
            + "([0-9]+.[0-9]+),([0-9]+.[0-9]+),"
            + "([0-9]+.[0-9]+),([0-9]+.[0-9]+)"
            + ",WPos:([0-9]+.[0-9]+),([0-9]+.[0-9]+),"
            + "([0-9]+.[0-9]+),([0-9]+.[0-9]+),"
            + "([0-9]+.[0-9]+),([0-9]+.[0-9]+),"
            + "([0-9]+.[0-9]+),([0-9]+.[0-9]+)>");
        /// <summary>
        /// Позиция значения состояния контроллера.
        /// </summary>
        private static int STATE = 1;
        /// <summary>
        /// Позиция значения локального X в выражении.
        /// </summary>
        private static int LOCAL_X = 2;
        /// <summary>
        /// Позиция значения локального Y в выражении.
        /// </summary>
        private static int LOCAL_Y = 3;
        /// <summary>
        /// Позиция значения локального Z в выражении.
        /// </summary>
        private static int LOCAL_Z = 4;
        /// <summary>
        /// Позиция значения локального A в выражении.
        /// </summary>
        private static int LOCAL_A = 5;
        /// <summary>
        /// Позиция значения локального B в выражении.
        /// </summary>
        private static int LOCAL_B = 6;
        /// <summary>
        /// Позиция значения локального C в выражении.
        /// </summary>
        private static int LOCAL_C = 7;
        /// <summary>
        /// Позиция значения локального D в выражении.
        /// </summary>
        private static int LOCAL_D = 8;
        /// <summary>
        /// Позиция значения локального E в выражении.
        /// </summary>
        private static int LOCAL_E = 9;
        /// <summary>
        /// Позиция значения глобального X в выражении.
        /// </summary>
        private static int GLOBAL_X = 10;
        /// <summary>
        /// Позиция значения глобального Y в выражении.
        /// </summary>
        private static int GLOBAL_Y = 11;
        /// <summary>
        /// Позиция значения глобального Z в выражении.
        /// </summary>
        private static int GLOBAL_Z = 12;
        /// <summary>
        /// Позиция значения глобального A в выражении.
        /// </summary>
        private static int GLOBAL_A = 13;
        /// <summary>
        /// Позиция значения глобального B в выражении.
        /// </summary>
        private static int GLOBAL_B = 14;
        /// <summary>
        /// Позиция значения глобального C в выражении.
        /// </summary>
        private static int GLOBAL_C = 15;
        /// <summary>
        /// Позиция значения глобального D в выражении.
        /// </summary>
        private static int GLOBAL_D = 16;
        /// <summary>
        /// Позиция значения глобального E в выражении.
        /// </summary>
        private static int GLOBAL_E = 17;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        private GConvert() { }

        /// <summary>
        /// Получение состояния по строке.
        /// </summary>
        /// <param name="response">строка состояния</param>
        /// <returns>состояние</returns>
        public static GState ToState(String response)
        {
            GState result = null;
            Match m = statePattern.Match(response);
            if (m.Success)
            {
                result = new GState
                {
                    Status = GConvert.ToStatus(m.Groups[STATE].Value),
                    Local = new GPoint
                    {
                        X = Convert.ToDouble(m.Groups[LOCAL_X].Value.Replace(".", ",")),
                        Y = Convert.ToDouble(m.Groups[LOCAL_Y].Value.Replace(".", ",")),
                        Z = Convert.ToDouble(m.Groups[LOCAL_Z].Value.Replace(".", ",")),
                        A = Convert.ToDouble(m.Groups[LOCAL_A].Value.Replace(".", ",")),
                        B = Convert.ToDouble(m.Groups[LOCAL_B].Value.Replace(".", ",")),
                        C = Convert.ToDouble(m.Groups[LOCAL_C].Value.Replace(".", ",")),
                        D = Convert.ToDouble(m.Groups[LOCAL_D].Value.Replace(".", ",")),
                        E = Convert.ToDouble(m.Groups[LOCAL_E].Value.Replace(".", ","))
                    },
                    Global = new GPoint
                    {
                        X = Convert.ToDouble(m.Groups[GLOBAL_X].Value.Replace(".", ",")),
                        Y = Convert.ToDouble(m.Groups[GLOBAL_Y].Value.Replace(".", ",")),
                        Z = Convert.ToDouble(m.Groups[GLOBAL_Z].Value.Replace(".", ",")),
                        A = Convert.ToDouble(m.Groups[GLOBAL_A].Value.Replace(".", ",")),
                        B = Convert.ToDouble(m.Groups[GLOBAL_B].Value.Replace(".", ",")),
                        C = Convert.ToDouble(m.Groups[GLOBAL_C].Value.Replace(".", ",")),
                        D = Convert.ToDouble(m.Groups[GLOBAL_D].Value.Replace(".", ",")),
                        E = Convert.ToDouble(m.Groups[GLOBAL_E].Value.Replace(".", ","))
                    }
                };
            }
            return result;
        }

        /// <summary>
        /// Получение состояния по названию.
        /// </summary>
        /// <param name="name">название состояния</param>
        /// <returns>состояние</returns>
        public static GStatus ToStatus(String name)
        {
            switch (name)
            {
                case "Idle":
                    return GStatus.IDLE;
                case "Run":
                    return GStatus.RUN;
                case "Hold":
                    return GStatus.HOLD;
                case "Door":
                    return GStatus.DOOR;
                case "Home":
                    return GStatus.HOME;
                case "Check":
                    return GStatus.CHECK;
                default:
                    return GStatus.ALARM;
            }
        }

        /// <summary>
        /// Получения названия по состоянию.
        /// </summary>
        /// <param name="state">состояние</param>
        /// <returns>название состояния</returns>
        public static String ToString(GStatus state)
        {
            switch (state)
            {
                case GStatus.IDLE:
                    return "Idle";
                case GStatus.RUN:
                    return "Run";
                case GStatus.HOLD:
                    return "Hold";
                case GStatus.DOOR:
                    return "Door";
                case GStatus.HOME:
                    return "Home";
                case GStatus.CHECK:
                    return "Check";
                case GStatus.DISCONNECT:
                    return "Disconnect";
                default:
                    return "Alarm";
            }
        }
    }
}
