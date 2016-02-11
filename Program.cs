using System;

using SimpleInjector;

namespace sitest
{
    interface INonGenericInterface
    {
    }

    class ImplOfNonGenericInterface : INonGenericInterface
    {
    }

    class NonGenericConsumerA
    {
        public NonGenericConsumerA(INonGenericInterface _)
        {
        }
    }

    class NonGenericConsumerB
    {
        public NonGenericConsumerB(INonGenericInterface _)
        {
        }
    }

    interface IGenericInterface<T>
    {
    }

    class ImplOfGenericInterface : IGenericInterface<object>
    {
    }

    class GenericConsumerA
    {
        public GenericConsumerA(IGenericInterface<object> _)
        {
        }
    }

    class GenericConsumerB
    {
        public GenericConsumerB(IGenericInterface<object> _)
        {
        }
    }

    class Program
    {
        private static void NonGenericTest()
        {
            var container = new Container();
            RegisterDependency<NonGenericConsumerA, INonGenericInterface>(container, () => new ImplOfNonGenericInterface());
            RegisterDependency<NonGenericConsumerB, INonGenericInterface>(container, () => new ImplOfNonGenericInterface());
            container.GetInstance<NonGenericConsumerA>();
            container.GetInstance<NonGenericConsumerB>();
        }

        private static void GenericTest()
        {
            var container = new Container();
            RegisterDependency<GenericConsumerA, IGenericInterface<object>>(container, () => new ImplOfGenericInterface());
            RegisterDependency<GenericConsumerB, IGenericInterface<object>>(container, () => new ImplOfGenericInterface());
            container.GetInstance<GenericConsumerA>();
            container.GetInstance<GenericConsumerB>();
        }

        private static void RegisterDependency<TImplementation, TDependency>(Container container, Func<TDependency> factory)
            where TImplementation : class
            where TDependency : class
        {
            var registration = Lifestyle.Singleton.CreateRegistration(factory, container);
            container.RegisterConditional(typeof(TDependency), registration,
                context =>
                {
                    var matched = context.Consumer.ImplementationType == typeof(TImplementation);
                    Console.WriteLine("Tested {0} against {1}, matched: {2}, handled: {3}.", context.Consumer.ImplementationType.Name, typeof(TImplementation).Name, matched, context.Handled);
                    return matched;
                });
            container.RegisterSingleton<TImplementation>();
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Non-generic test:");
                NonGenericTest();
                Console.WriteLine("Generic test:");
                GenericTest();
                Console.WriteLine("Done.");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
            }
        }
    }
}
