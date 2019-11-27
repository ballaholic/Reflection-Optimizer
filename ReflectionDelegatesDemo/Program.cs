namespace ReflectionDelegatesDemo
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections.Generic;

    public class Program
    {
        public static void Main()
        {
            var homeController = new HomeController();
            var homeControllerType = homeController.GetType();

            var property = homeControllerType.GetProperties()
                .FirstOrDefault(x => x.IsDefined(typeof(DataAttribute), true));

            var getMethod = property.GetMethod;

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                var dict = (IDictionary<string, object>)getMethod.Invoke(homeController, Array.Empty<object>());
            }

            Console.WriteLine(stopwatch.Elapsed);

            var deleg = PropertyHelper.MakeFastPropertyGetter<IDictionary<string, object>>(property);

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                var dict = deleg(homeController);
            }

            Console.WriteLine(stopwatch.Elapsed);

        }
    }
}
