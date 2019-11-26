namespace ReflectionDelegatesDemo
{
    using System.Collections.Generic;
    public class HomeController
    {
        public HomeController()
            => this.Data = new Dictionary<string, object>
            {
                ["Name"] = "Reflection Delegates"
            };

        [Data]
        public IDictionary<string, object> Data { get; set; }
    }
}
