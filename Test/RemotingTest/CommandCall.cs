﻿using System;
using System.Runtime.Remoting.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;


using NSubstitute;

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Extension;
using Regulus.Utility;

namespace RemotingTest
{
	[TestClass]
	public class CommandCall
	{
		[TestMethod]
		public void TestCommandCall()
		{
			var param = Substitute.For<CommandParam>();
			var called = false;
			param.Types = new[]
			{
				typeof(string)
			};
			param.Callback = new Action<string>(msg => { called = true; });

			var command = new Command();
			command.Register("123", param);
			command.Run(
				"123", 
				new[]
				{
					" Hello World."
				});

			Assert.AreEqual(true, called);
		}

		[TestMethod]
		public void TestCommandAdd()
		{
			var param = Substitute.For<CommandParam>();
			float value = 0;
			param.Types = new[]
			{
				typeof(int), 
				typeof(int)
			};
			param.ReturnType = typeof(float);

			param.Callback = new Func<int, int, float>((a, b) => { return a + b; });
			param.Return = new Action<float>(val => { value = val; });

			var command = new Command();
			command.Register("123", param);
			command.Run(
				"123", 
				new[]
				{
					"1", 
					"2"
				});

			Assert.AreEqual(3, value);
		}

		[TestMethod]
		public void TestGPIBinder()
		{
			var command = new Command();
            var tester = Substitute.For<IBinderTest>();
            var notifier = new TestNotifier(tester);
            

            var binder = new GPIBinder<IBinderTest>(notifier, command);
		    IBootable boot = binder;
            boot.Launch();
            
            
            binder.Bind(t => t.Function1());
            binder.Bind<int>((t, arg) => t.Function2(arg));
		    bool returnValue = false;
		    int returnProperty = 0;
            binder.Bind(t => t.Function3(), ret => { returnValue = true; });

            binder.Bind(t => t.Property1 , ret => { returnProperty = 12345; });

            notifier.InvokeSupply();

            command.Run("Function1", new string[0]);
            command.Run("Function2", new string[] {"10"});
            command.Run("Function3", new string[0] );
            command.Run("Property1", new string[0]);




            boot.Shutdown();

            tester.Received().Function1();
            tester.Received().Function2( Arg.Any<int>());

            Assert.AreEqual(true , returnValue);
            Assert.AreEqual(12345, returnProperty);
        }


	    class TestNotifier : INotifier<IBinderTest>
	    {
	        private readonly IBinderTest _Tester;

	        private event Action<IBinderTest> _Supply;
            public TestNotifier(IBinderTest tester)
	        {
	            _Tester = tester;
	            
	        }

	        public void InvokeSupply()
	        {
	            _Supply(_Tester);
	        }
	        event Action<IBinderTest> INotifier<IBinderTest>.Return
	        {
	            add { }
	            remove {  }
	        }

	        event Action<IBinderTest> INotifier<IBinderTest>.Supply
	        {
                add { _Supply += value; }
                remove { _Supply -= value; }
            }

	        event Action<IBinderTest> INotifier<IBinderTest>.Unsupply
	        {
	            add {  }
	            remove {  }
	        }

	        IBinderTest[] INotifier<IBinderTest>.Ghosts
	        {
	            get
	            {
	                return new[]
	                {
	                    _Tester
	                };
	            }
	        }

	        IBinderTest[] INotifier<IBinderTest>.Returns
	        {
	            get {
                    return new[]
                   {
                        _Tester
                    };
                }
	        }
	    }
    }
}
