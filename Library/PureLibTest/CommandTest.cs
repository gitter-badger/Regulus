﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandTest.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the CommandTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace PureLibraryTest
{
	[TestClass]
	public class CommandTest
	{
		[TestMethod]
		public void TestCommandAnalysisWithParameters()
		{
			var analysis = new Command.Analysis("login [ account ,    password, result]");

			Assert.AreEqual("login", analysis.Command);
			Assert.AreEqual("result", analysis.Parameters[2]);
			Assert.AreEqual("account", analysis.Parameters[0]);
			Assert.AreEqual("password", analysis.Parameters[1]);
		}

		[TestMethod]
		public void TestCommandAnalysisNoParameters()
		{
			var analysis = new Command.Analysis("login");

			Assert.AreEqual("login", analysis.Command);
			Assert.AreEqual(0, analysis.Parameters.Length);
		}

		[TestMethod]
		public void TestCommandRegister0()
		{
			var command = new Command();
			var cr = new CommandRegister<ICallTester>("Function1", new string[0], command, caller => caller.Function1());
			var callTester = Substitute.For<ICallTester>();
			cr.Register(callTester);
			command.Run("Function1", new string[0]);
			callTester.Received(1).Function1();
			cr.Unregister();
		}

		[TestMethod]
		public void TestCommandRegister1()
		{
			// data
			var command = new Command();
			var cr = new CommandRegister<ICallTester, int>("Function2", new[]
			{
				"arg1"
			}, command, (caller, arg1) => caller.Function2(arg1));
			var callTester = Substitute.For<ICallTester>();

			// test
			cr.Register(callTester);
			command.Run("Function2", new[]
			{
				"1"
			});

			// verify
			cr.Unregister();
		}

		[TestMethod]
		public void TestCommandRegister2()
		{
			var command = new Command();
			var cr = new CommandRegisterReturn<ICallTester, int>("Function3", new string[]
			{
			}, command, caller => caller.Function3(), ret => { });
			var callTester = Substitute.For<ICallTester>();
			cr.Register(callTester);
			command.Run("Function3", new string[]
			{
			});
			callTester.Received(1).Function3();
			cr.Unregister();
		}

		[TestMethod]
		public void TestCommandRegister3()
		{
			var command = new Command();
			var cr = new CommandRegisterReturn<ICallTester, int, byte, float, int>("Function4", new string[]
			{
			}, command, (caller, arg1, arg2, arg3) => caller.Function4(arg1, arg2, arg3), ret => { });
			var callTester = Substitute.For<ICallTester>();
			cr.Register(callTester);
			command.Run("Function4", new[]
			{
				"1", 
				"2", 
				"3"
			});
			callTester.Received(1).Function4(Arg.Any<int>(), Arg.Any<byte>(), Arg.Any<float>());
			cr.Unregister();
		}
	}
}