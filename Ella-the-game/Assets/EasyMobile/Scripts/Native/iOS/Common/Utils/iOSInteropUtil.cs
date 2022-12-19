#if UNITY_IOS
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EasyMobile.iOS.Foundation;
using CF_Functions = EasyMobile.iOS.CoreFoundation.CFFunctions;

namespace EasyMobile.Internal.iOS
{
    internal static class iOSInteropUtil
    {
        /// Dictionary<frameworkName, Dictionary<constantName, constantValue>>.
        private static Dictionary<string, Dictionary<string, NSString>> sConstantMap = new Dictionary<string, Dictionary<string, NSString>>();

        /// <summary>
        /// Lookups the value of a string constant in the specified 
        /// native system framework at runtime. Found constants are
        /// cached for subsequent queries.
        /// This variant receives an expression of the property representing
        /// the constant and extract its name for the search.
        /// </summary>
        /// <returns>The string constant.</returns>
        /// <param name="constantExp">Constant exp.</param>
        /// <param name="frameworkName">Framework name.</param>
        public static NSString LookupStringConstant(Expression<Func<NSString>> constantExp, string frameworkName)
        {
            return LookupStringConstant(ReflectionUtil.GetMemberNameFromExpression(constantExp), frameworkName);
        }

        /// <summary>
        /// Lookups the value of a string constant in the specified 
        /// native system framework at runtime. Found constants are
        /// cached for subsequent queries.
        /// </summary>
        /// <returns>The string constant.</returns>
        /// <param name="constantName">Constant name.</param>
        public static NSString LookupStringConstant(string constantName, string frameworkName)
        {
            if (constantName == null || frameworkName == null)
                return null;

            // If there's no existing dict for the queried framework, add one.
            if (!sConstantMap.ContainsKey(frameworkName))
                sConstantMap[frameworkName] = new Dictionary<string, NSString>();

            // Check the existing dict for the queried framework:
            // if the constant is stored previously, just return it,
            // otherwise, look it up from native side, store it and return.
            var storedConsts = sConstantMap[frameworkName];
            NSString constant;
                
            if (!storedConsts.TryGetValue(constantName, out constant))
            {
                constant = DoLookupStringConstant(constantName, frameworkName);
                
                if (constant != null)
                    storedConsts[constantName] = constant;
            }
                
            return constant;
        }

        private static NSString DoLookupStringConstant(string constantName, string frameworkName)
        {
            var ptr = C.InteropUtil_lookupStringConstantInSystemFramework(constantName, frameworkName);

            if (ptr != IntPtr.Zero)
            {
                var constant = new NSString(ptr);
                CF_Functions.CFRelease(ptr);
                return constant;
            }
            else
            {
                return null;
            }
        }

        #region C wrapper

        private static class C
        {
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr InteropUtil_lookupStringConstantInSystemFramework(string constantName, string frameworkName);
        }

        #endregion
    }
}
#endif
