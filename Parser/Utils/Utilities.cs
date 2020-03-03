using System;
using System.Collections.Generic;
using System.Text;

namespace Informa.IntelligenceOne.Parser.Utilities
{
    class StringFunctions
    {
        public static void Replace(ref StringBuilder builder, object data)
        {
            Replace(ref builder, data, 0);
        }
        public static void Replace(ref StringBuilder builder, object data,int option)
        {
            if (data != null)
            {
                if(option == Const.Parser.UPPER_CASE_VALUE)
                    Replace(ref builder, data.ToString().ToUpper());
                else
                    Replace(ref builder, data.ToString());
            }
            else
                Replace(ref builder, string.Empty);
        }

        public static void Replace(ref StringBuilder builder, string data)
        {
            if (builder == null)
                builder = new StringBuilder();
            else
                builder.Clear();

            builder.Append(data);
        }

        public static bool IsString(string v)
        {
            return v != null && v != string.Empty;
        }

        public static int ConvertToInt(object v, int defaultValue)
        {
            if (v == null)
                return defaultValue;

            return ConvertToInt(v.ToString(), defaultValue);
        }
        public static int ConvertToInt(string v, int defaultValue)
        {
            int vValue = defaultValue;
            try
            {
                vValue = Convert.ToInt32(v);
            }
            catch { }

            return vValue;
        }

    }
}
