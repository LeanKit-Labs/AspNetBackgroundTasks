using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Hosting.Prig;
using LK.AspNetBackgroundTasks.Internal;
using NUnit.Framework;
using Urasandesu.Prig.Framework;

namespace UnitTests
{
    [TestFixture]
    public class BackgroundTaskManagerUnitTests
    {
        [Test]
        public void BTM_RegistersWithHostEnvironment()
        {
            IRegisteredObject registeredObject = null;

            using (new IndirectionsContext())
            {
                PHostingEnvironment.RegisterObjectIRegisteredObject().Body = obj => { registeredObject = obj; };
                var instance = new RegisteredTasks();
                var mre = new ManualResetEvent(false);
                instance.Run(() => mre.WaitOne());
                mre.Set();
            }

            Assert.IsNotNull(registeredObject);
        }

        [Test]
        public void BTM_BeforeShutdown_ShutdownNotSignaled()
        {
            using (new IndirectionsContext())
            {
                PHostingEnvironment.RegisterObjectIRegisteredObject().Body = _ => { };
                var instance = new RegisteredTasks();
                var mre = new ManualResetEvent(false);
                instance.Run(() => mre.WaitOne());
                mre.Set();
                Assert.IsFalse(instance.Shutdown.IsCancellationRequested);
            }
        }

        [Test]
        public void BTM_AfterShutdownRequest_SyncTaskStillRunning_ShutdownIsSignaled()
        {
            using (new IndirectionsContext())
            {
                IRegisteredObject registeredObject = null;
                PHostingEnvironment.RegisterObjectIRegisteredObject().Body = obj => { registeredObject = obj; };
                PHostingEnvironment.UnregisterObjectIRegisteredObject().Body = _ => { };

                var instance = new RegisteredTasks();
                var mre = new ManualResetEvent(false);
                instance.Run(() => mre.WaitOne());

                registeredObject.Stop(false);
                Assert.IsTrue(instance.Shutdown.IsCancellationRequested);

                mre.Set();
            }
        }

        [Test]
        public void BTM_AfterBlockingShutdown_UnregistersFromHostEnvironment()
        {
            var mutex = new object();
            IRegisteredObject registeredObject = null;

            using (new IndirectionsContext())
            {
               
                PHostingEnvironment.RegisterObjectIRegisteredObject().Body = obj =>
                {
                    lock (mutex)
                        registeredObject = obj;
                };
                PHostingEnvironment.UnregisterObjectIRegisteredObject().Body = obj =>
                {
                    lock (mutex)
                    {
                        Assert.AreSame(registeredObject, obj);
                        registeredObject = null;
                    }
                };

                var instance = new RegisteredTasks();
                var mre = new ManualResetEvent(false);
                instance.Run(() => mre.WaitOne());
                mre.Set();

                registeredObject.Stop(true);
               
                lock (mutex)
                {
                    Assert.IsNull(registeredObject);
                }
            }
        }

        [Test]
        public void BTM_BlockingShutdown_WaitsForSyncTaskToExit()
        {
            var mutex = new object();
            IRegisteredObject registeredObject = null;

            using (new IndirectionsContext())
            {
               
                PHostingEnvironment.RegisterObjectIRegisteredObject().Body = obj =>
                {
                    lock (mutex)
                        registeredObject = obj;
                };
                PHostingEnvironment.UnregisterObjectIRegisteredObject().Body = obj =>
                {
                    lock (mutex)
                    {
                        Assert.AreSame(registeredObject, obj);
                        registeredObject = null;
                    }
                };

                var instance = new RegisteredTasks();
                var mre = new ManualResetEvent(false);
                instance.Run(() => mre.WaitOne());

                var task = Task.Factory.StartNew(() => registeredObject.Stop(true));
                Assert.IsFalse(task.Wait(300));
                lock (mutex)
                    Assert.IsNotNull(registeredObject);
                mre.Set();
            }
        }
    }
}
