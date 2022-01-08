// See https://aka.ms/new-console-template for more information

using pdp_lab4.impl;

namespace pdp_lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            var hosts = new[]
                {
                    "example.com/hello", "neverssl.com/online", "neverssl.com/"
                }
                .ToList();

            Console.WriteLine("----Callback----");
            var impl = new CallbackImpl();
            impl.run(hosts);

            Console.WriteLine("----Task----");
            var impl1 = new TaskRunner();
            impl1.Run(hosts);

            Console.WriteLine("----Async----");
            var impl2 = new TaskRunnerAsync();
            impl2.Run(hosts);
        }
    }
}
