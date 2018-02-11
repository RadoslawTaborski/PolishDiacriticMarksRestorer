using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;

namespace PolishDiacriticMarksRestorer
{
    public class SerializeStatic
    {
        public static bool Save(Type staticClass, string filename)
        {
            try
            {
                var fields = staticClass.GetFields(BindingFlags.Static | BindingFlags.Public);
                var a = new object[fields.Length, 2];
                var i = 0;
                foreach (var field in fields)
                {
                    a[i, 0] = field.Name;
                    a[i, 1] = field.GetValue(null);
                    i++;
                }

                Stream f = File.Open(filename, FileMode.Create);
                var formatter = new SoapFormatter();
                formatter.Serialize(f, a);
                f.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Load(Type staticClass, string filename)
        {
            try
            {
                var fields = staticClass.GetFields(BindingFlags.Static | BindingFlags.Public);
                Stream f = File.Open(filename, FileMode.Open);
                var formatter = new SoapFormatter();
                var a = formatter.Deserialize(f) as object[,];
                f.Close();
                if (a != null && a.GetLength(0) != fields.Length) return false;
                var i = 0;
                foreach (var field in fields)
                {
                    if (a != null && field.Name == (a[i, 0] as string))
                    {
                        field.SetValue(null, a[i, 1]);
                    }
                    i++;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
