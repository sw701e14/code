using System;
using System.Linq;

namespace DatabaseImportTest
{
    /// <summary>
    /// Provides methods that simplify the testing of private methods through reflection.
    /// </summary>
    public static class PrivateTesting
    {
        /// <summary>
        /// Invokes a private instance method on <paramref name="obj"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result from the invocation.</typeparam>
        /// <param name="obj">The object on which the method is invoked.</param>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments for the method call; <c>null</c> specifies no arguments.</param>
        /// <returns>The result of calling <paramref name="method"/> on <paramref name="obj"/>.</returns>
        public static TResult InvokePrivate<TResult>(this object obj, string method, params object[] args)
        {
            var type = obj.GetType();

            if (args == null)
                args = new object[0];

            if (args.Any(o => o == null))
                throw new ArgumentNullException("args", "This method for invoking private methods does not support null-values.");

            var argTypes = (from o in args select o.GetType()).ToArray();

            var _method = type.GetMethod(method, argTypes);
            return (TResult)_method.Invoke(obj, args);
        }

        /// <summary>
        /// Invokes a static private method on a type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result from the invocation.</typeparam>
        /// <param name="type">The type in which the static method is defined.</param>
        /// <param name="method">The name of the method.</param>
        /// <param name="args">The arguments for the method call; <c>null</c> specifies no arguments.</param>
        /// <returns>The result of calling <paramref name="method"/>.</returns>
        public static TResult InvokeStaticPrivate<TResult>(this Type type, string method, params object[] args)
        {
            if (args == null)
                args = new object[0];

            if (args.Any(o => o == null))
                throw new ArgumentNullException("args", "This method for invoking private methods does not support null-values.");

            var argTypes = (from o in args select o.GetType()).ToArray();

            var _method = type.GetMethod(method,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                null, argTypes, null);
            return (TResult)_method.Invoke(null, args);
        }
    }
}
