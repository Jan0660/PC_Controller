using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Communication
{
    public enum CodesEnum : int
    {
        Error, Success,
        True, False,
    }

    public class Codes
    {
        // Yes i know that all of this is shit code and the scalability is just nonexistent
        // im probalby never gonna add more values to this so ¯\_(ツ)_/¯
        // this is what you do when enums arent enough
        CodesEnum code;

        public static Codes Error { get; } = new Codes(CodesEnum.Error);
        public static Codes Success { get; } = new Codes(CodesEnum.Success);
        public static Codes True { get; } = new Codes(CodesEnum.True);
        public static Codes False { get; } = new Codes(CodesEnum.False);

        public Codes(CodesEnum cod)
        {
            code = cod;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(CodesEnum), code);
        }

        public static Codes FromString(string str)
        {
            return new Codes((CodesEnum)Enum.Parse(typeof(CodesEnum), str, true));
        }

        /// <summary>
        /// Returns true if the code is "Success" or "True"
        /// </summary>
        public bool ToBool()
        {
            if (code == CodesEnum.True | code == CodesEnum.Success)
                return true;
            else return false;
        }

        /// <summary>
        /// Sets the code to True or False
        /// </summary>
        public static Codes FromBool(bool b)
        {
            if (b)
            {
                return new Codes(CodesEnum.True);
            }
            else
            {
                return new Codes(CodesEnum.False);
            }
        }
    }
}
