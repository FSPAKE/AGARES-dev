using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>
    /// Parse部分の参照を見るための置換クラス
    /// </summary>
    public static class Parse
    {
        public static bool ToBool(string parm) => bool.Parse(parm);
        public static byte ToByte(string parm) => byte.Parse(parm);
        public static sbyte ToSbyte(string parm) => sbyte.Parse(parm);
        public static char ToChar(string parm) => char.Parse(parm);
        public static decimal ToDecimal(string parm) => decimal.Parse(parm);
        public static double ToDouble(string parm) => double.Parse(parm);
        public static float ToFloat(string parm) => float.Parse(parm);
        public static int ToInt(string parm) => int.Parse(parm);
        public static uint ToUInt(string parm) => uint.Parse(parm);
        public static long ToLong(string parm) => long.Parse(parm);
        public static ulong ToULong(string parm) => ulong.Parse(parm);
        public static short ToShort(string parm) => short.Parse(parm);
        public static ushort ToUShort(string parm) => ushort.Parse(parm);
    }
}
