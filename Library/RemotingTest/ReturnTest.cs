﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Extension;
namespace RemotingTest
{
    [TestClass]
    public class ReturnTest
    {





        [TestMethod]
        public void RemotingTest()
        {
            System.Threading.SpinWait sw = new System.Threading.SpinWait();            
            Regulus.Utility.Launcher launcher = new Regulus.Utility.Launcher();
            Server server = new Server();
            Regulus.Remoting.Soul.Native.Server serverAppliction = new Regulus.Remoting.Soul.Native.Server(server, 12345);
            var empty = new EmptyInputView();
            var app = new Regulus.Framework.Client<IUser>(empty, empty);
            app.Selector.AddFactoty("1", new RemotingProvider());
            var userProvider = app.Selector.CreateUserProvider("1");
            var user = userProvider.Spawn("1");


            ITestReturn testReturn = null;
            user.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;
            user.TestReturnProvider.Return += (test) =>
            {
                testReturn = test;                
                Assert.AreEqual(1, user.TestReturnProvider.Returns.Length);

            };

            Regulus.Utility.CenterOfUpdateable updater = new Regulus.Utility.CenterOfUpdateable();
            updater.Add(app);
            launcher.Push(serverAppliction);
            launcher.Launch();
            bool enable = true;
            int result2 = 0;
            while (enable)
            {
                updater.Working();
                sw.SpinOnce();
                if (testReturn != null)
                {

                    testReturn.Test(1, 2).OnValue += (r) =>
                    {
                        r.Add(2, 3).OnValue += (r2) => { result2 = r2; };
                        enable = false;

                    };

                    
                    enable = false;
                    testReturn = null;
                    System.GC.Collect();
                }
            }

            updater.Working();

            while (result2 == 0)
                updater.Working();

            System.GC.Collect();

            Assert.AreEqual(0, user.TestReturnProvider.Returns.Length);
            Assert.AreEqual(-3, result2);

            launcher.Shutdown();

            user.Remoting.ConnectProvider.Supply -= ConnectProvider_Supply;
            updater.Shutdown();
        }
        [TestMethod]
        public void StandalongTest()
        {
            var server = new Server();

            var empty = new EmptyInputView();
            var app = new Regulus.Framework.Client<IUser>(empty, empty);
            app.Selector.AddFactoty("1", new StandalongProvider(server));
            var userProvider =  app.Selector.CreateUserProvider("1");
            var user = userProvider.Spawn("1");

            ITestReturn testReturn = null;
            user.Remoting.ConnectProvider.Supply += ConnectProvider_Supply;
            user.TestReturnProvider.Return += (test)=>
            {
                testReturn = test;
                Assert.AreEqual(1, user.TestReturnProvider.Returns.Length);

            };
            
            Regulus.Utility.CenterOfUpdateable updater = new Regulus.Utility.CenterOfUpdateable();
            updater.Add(app);
            updater.Add(server);
            bool enable = true;
            int result2 = 0;

            
            while (enable)
            {

                updater.Working();
                
                
                if(testReturn!= null)
                {
                    testReturn.Test(1, 2).OnValue += (r) =>
                    {
                        r.Add(2, 3).OnValue += (r2) => { result2 = r2  ; };                        
                        enable = false;
                        
                    };

                    testReturn = null;
                    
                }
            }

            updater.Working();
            
            while (result2 == 0)
                updater.Working();

            System.GC.Collect();

            
            
            Assert.AreEqual(0, user.TestReturnProvider.Returns.Length);
            Assert.AreEqual(-3, result2);

            user.Remoting.ConnectProvider.Supply -= ConnectProvider_Supply;
            updater.Shutdown();
        }

        

        void ConnectProvider_Supply(Regulus.Utility.IConnect obj)
        {
            obj.Connect("127.0.0.1", 12345);
        }

        
    }
}
