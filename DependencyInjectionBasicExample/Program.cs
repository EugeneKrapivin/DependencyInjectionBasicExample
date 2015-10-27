using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DependencyInjectionBasicExample
{
    #region interfaces
    public interface INeedSomeOneToDoWork
    {
        void MakeHimWork();
    }

    public interface IWork
    {
        void DoWork();
    }

    public interface ITool
    {
        void UseTool();
    }
    #endregion

    #region implementations
    public class ScrewDriver : ITool
    {
        public void UseTool()
        {
            Console.WriteLine("screwing around");
        }
    }
    public class Hammer : ITool
    {
        public void UseTool()
        {
            Console.WriteLine("I'm hammered");
        }
    }
    public class Worker : IWork
    {
        readonly IEnumerable<ITool> _tools;

        public Worker(IEnumerable<ITool> tools)
        {
            if (tools == null)
            {
                throw new ArgumentNullException(nameof(tools));
            }
            _tools = tools;
        }
        public void DoWork()
        {
            Console.WriteLine("Begin work...");
            foreach(var tool in _tools)
            {
                tool.UseTool();
            }
            Console.WriteLine("Time to rest now...");
        }
    }
    public class NeedSomeOneToDoWork : INeedSomeOneToDoWork
    {
        private readonly IWork _worker;

        public NeedSomeOneToDoWork(IWork worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }
            _worker = worker;
        }

        public void MakeHimWork()
        {
            _worker.DoWork();
        }
    }
    #endregion

    class Program
    {
        #region setup the container
        static IWindsorContainer SetupContainer()
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel)); // so we could resolve collections
            
            // register our components
            container.Register(
                Component.For<IWork>().ImplementedBy<Worker>().LifestyleTransient(),
                Component.For<INeedSomeOneToDoWork>().ImplementedBy<NeedSomeOneToDoWork>().LifestyleTransient(),
                Component.For<ITool>().ImplementedBy<ScrewDriver>().LifestyleTransient(),
                Component.For<ITool>().ImplementedBy<Hammer>().LifestyleTransient());

            return container;
        }
        #endregion

        static void Main(string[] args)
        {
            // the first resolve will typically be in the WebAPI controller resolver
            // in the case of a win service it will be the root of the application class, the whole application will run from the container
            var master = SetupContainer().Resolve<INeedSomeOneToDoWork>();

            master.MakeHimWork();
        }
    }
}
