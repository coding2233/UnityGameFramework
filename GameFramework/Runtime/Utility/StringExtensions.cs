using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public static class StringExtensions
    {
        public static bool ToBool(this string value)
        {
            bool result = false;
            bool.TryParse(value.ToLower(),out result);
            return result;
        }

        public static int ToInt32(this string value)
        {
            int result = 0;
            int.TryParse(value,out result);
            return result;
        }

        public static long ToInt64(this string value)
        {
            long result = 0;
            long.TryParse(value,out result);
            return result;
        }

        public static float ToFloat(this string value)
        {
            float result = 0;
            float.TryParse(value,out result);
            return result;
        }

        public static double ToDouble(this string value)
        {
            double result = 0;
            double.TryParse(value,out result);
            return result;
        }

        public static Vector2 ToVector2(this string value)
        {
            Vector2 result = Vector2.zero;
            string[] args =  value.Split(',');
            if(args.Length==2)
            {
                result.x=args[0].ToFloat();
                result.y=args[1].ToFloat();
            }
            return result;
        }

        public static Vector3 ToVector3(this string value)
        {
            Vector3 result = Vector3.zero;
            string[] args =  value.Split(',');
            if(args.Length==3)
            {
                result.x=args[0].ToFloat();
                result.y=args[1].ToFloat();
                result.z=args[2].ToFloat();
            }
            return result;
        }

        public static Color ToColor(this string value)
        {
            Color result = Color.white;
            UnityEngine.ColorUtility.TryParseHtmlString(value.Trim(),out result);
            return result;
        }
   
    }
}

