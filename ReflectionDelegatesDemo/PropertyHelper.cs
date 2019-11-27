namespace ReflectionDelegatesDemo
{
    using System;
    using System.Reflection;
    using System.Collections.Concurrent;

    public class PropertyHelper
    {
        private static readonly ConcurrentDictionary<string, Delegate> cache = new ConcurrentDictionary<string, Delegate>();

        private static readonly MethodInfo CallInnerDelegateMethod =
            typeof(PropertyHelper).GetMethod(nameof(CallInnerDelegate), BindingFlags.NonPublic | BindingFlags.Static);

        public static Func<object, TResult> MakeFastPropertyGetter<TResult>(PropertyInfo property)
            => (Func<object, TResult>)cache.GetOrAdd(property.Name, key =>
            {
                MethodInfo getMethod = property.GetMethod;

                Type declaringClass = property.DeclaringType;

                Type typeOfResult = typeof(TResult);

                // <=> Func<ControllerType, TResult>
                Type getMethodDelegateType = typeof(Func<,>).MakeGenericType(declaringClass, typeOfResult);

                // <=> controller => controller.Data
                Delegate getMethodDelegate = getMethod.CreateDelegate(getMethodDelegateType);

                // Constructed a generic method CallInnerDelegate<ControllerType, TResult>
                var callInnerGenericMethodWithTypes = CallInnerDelegateMethod.MakeGenericMethod(declaringClass, typeOfResult);

                var result = (Delegate)callInnerGenericMethodWithTypes.Invoke(null, new[] { getMethodDelegate });

                return result;
            });

        private static Func<object, TResult> CallInnerDelegate<TClass, TResult>(Func<TClass, TResult> deleg)
            => instance => deleg((TClass)instance);

    }
}
