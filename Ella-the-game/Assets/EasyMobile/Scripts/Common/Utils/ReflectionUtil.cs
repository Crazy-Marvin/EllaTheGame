using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace EasyMobile.Internal
{
    internal static class ReflectionUtil
    {
        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <returns>The method name.</returns>
        /// <param name="func">Func.</param>
        public static string GetMethodName(Delegate method)
        {
            return method.Method.Name;
        }

        /// <summary>
        /// Creates an object of the output type and copies values of all public 
        /// properties and fields of the input object to the matching public
        /// properties and fields of the output object.
        /// This method is intended to be a convenient replacement for manual copying codes
        /// when there is a need for converting objects of two similar classes.
        /// </summary>
        /// <returns>The object data.</returns>
        /// <param name="inObj">In object.</param>
        /// <typeparam name="TIn">The 1st type parameter.</typeparam>
        /// <typeparam name="TOut">The 2nd type parameter.</typeparam>
        public static TOut CopyObjectData<TIn, TOut>(TIn inObj) where TOut : new()
        {
            if (inObj is Enum)
                throw new NotImplementedException("Input must be class not enum!");

            if (inObj == null)
                return default(TOut);

            TOut outObj = new TOut();

            Type fromType = inObj.GetType();
            Type toType = outObj.GetType();

            // Copy properties
            PropertyInfo[] inProps = fromType.GetProperties();

            foreach (PropertyInfo inProp in inProps)
            {
                PropertyInfo outProp = toType.GetProperty(inProp.Name);

                if (outProp != null && outProp.CanWrite)
                {
                    object value = inProp.GetValue(inObj, null);
                    outProp.SetValue(outObj, value, null);
                }
            }

            // Copy fields
            FieldInfo[] inFields = fromType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo inField in inFields)
            {
                FieldInfo outField = toType.GetField(inField.Name);

                if (outField != null && outField.IsPublic)
                {
                    object value = inField.GetValue(inObj);
                    outField.SetValue(outObj, value);
                }
            }

            return outObj;
        }

        /// <summary>
        /// Gets the member name from expression, e.g. "this.SomeProperty".
        /// </summary>
        /// <returns>The member name from expression.</returns>
        /// <param name="propertyExpression">Property expression.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string GetMemberNameFromExpression<T>(Expression<Func<T>> propertyExpression)
        {
            var memExp = propertyExpression.Body as MemberExpression;
            return memExp != null ? memExp.Member.Name : null;
        }
    }
}