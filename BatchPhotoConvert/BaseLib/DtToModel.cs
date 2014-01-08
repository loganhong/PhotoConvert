//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace BatchPhotoConvert
//{
//    public class DtToModel<T> where T : class,new()
//    {
//        public delegate void SetValue<PT>(PT value);
//        private static Delegate CreateSetDelegate(T model, string propertyName)
//        {
//            try
//            {
//                //var type = model.GetType();
//                //MethodInfo mi = type.GetProperty(propertyName).PropertyType;
//                //PropertyInfo  pro
//                //var pro = type.GetProperty(propertyName);
//                //pro.GetSetMethod();
//                MethodInfo mi = model.GetType().GetProperty(propertyName).GetSetMethod();
//                //这里构造泛型委托类型
//                Type delType = typeof(SetValue<>).MakeGenericType(GetPropertyType(propertyName));

//                return Delegate.CreateDelegate(delType, model, mi);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }

//        }

//        private static Type GetPropertyType(string propertyName)
//        {
//            return typeof(T).GetProperty(propertyName).PropertyType;
//        }

//        public static IList<T> DtToModelList(DataTable dt)
//        {
//            IList<T> list = new List<T>();
//            if (dt == null || dt.Rows.Count < 1)
//            {
//                return list;
//            }

//            Delegate setDelegate;
//            foreach (DataRow dr in dt.Rows)
//            {
//                T model = new T();
//                foreach (DataColumn dc in dt.Columns)
//                {
//                    setDelegate = CreateSetDelegate(model, dc.ColumnName);
//                    //setDelegate.DynamicInvoke(Convert.ChangeType(dr[dc.ColumnName],GetPropertyType(dc.ColumnName)));
//                    setDelegate.DynamicInvoke(Convert.ChangeType(dr[dc.ColumnName], GetPropertyType(dc.ColumnName)));
//                }
//                list.Add(model);
//            }
//            return list;
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BatchPhotoConvert
{
    public delegate void SetValue<T>(T value);

    public class DtToModel<T> where T : class,new()
    {
        private static Delegate CreateSetValueDelegate(T model, string propertyName)
        {
            try
            {
                var type = model.GetType();
                var pro = type.GetProperty(propertyName);
                MethodInfo mi = model.GetType().GetProperty(propertyName).GetSetMethod();
                //这里构造泛型委托类型
                Type delType = typeof(SetValue<>).MakeGenericType(GetPropertyType(propertyName));
                
                return Delegate.CreateDelegate(delType, model, mi);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private static Type GetPropertyType(string propertyName)
        {
            return typeof(T).GetProperty(propertyName).PropertyType;
        }

        public static IList<T> DtToModelList(DataTable dt)
        {
            try
            {
                IList<T> list = new List<T>();
                if (dt == null || dt.Rows.Count < 1)
                {
                    return list;
                }

                Delegate setValueDelegate;
                foreach (DataRow dr in dt.Rows)
                {
                    T model = new T();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        setValueDelegate = CreateSetValueDelegate(model, dc.ColumnName);
                        //setDelegate.DynamicInvoke(Convert.ChangeType(dr[dc.ColumnName],GetPropertyType(dc.ColumnName)));
                        setValueDelegate.DynamicInvoke(ChangeType(dr[dc.ColumnName], GetPropertyType(dc.ColumnName)));
                        //var pro = model.GetType().GetProperty(dc.ColumnName);
                        //pro.SetValue(model, dr[dc.ColumnName], null);
                    }
                    list.Add(model);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType &&
                conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {

                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter
                    = new System.ComponentModel.NullableConverter(conversionType);

                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }


        static public object ChangeType2(object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            //if (!type.IsInterface && type.IsGenericType)
            //{
            //    Type innerType = type.GetGenericArguments()[0];
            //    object innerValue = QueryHelper.ChangeType(value, innerType);
            //    return Activator.CreateInstance(type, new object[] { innerValue });
            //}
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }
    }
}



