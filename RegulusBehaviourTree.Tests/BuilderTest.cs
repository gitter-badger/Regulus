using System.Linq.Expressions;
using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.BehaviourTree;

namespace Regulus.BehaviourTree.Tests
{
    /// <summary>此類別包含 Builder 的參數化單元測試</summary>
    [TestClass]
    
    public partial class BuilderTest
    {

        /// <summary>.ctor() 的測試虛設常式</summary>

        public Builder ConstructorTest()
        {
            Builder target = new Builder();
            return target;
            // TODO: 將判斷提示加入 方法 BuilderTest.ConstructorTest()
        }

        /// <summary>Action(!!0, Expression`1&lt;Func`2&lt;!!0,Func`2&lt;Single,TICKRESULT&gt;&gt;&gt;, Expression`1&lt;Func`2&lt;!!0,Action&gt;&gt;, Expression`1&lt;Func`2&lt;!!0,Action&gt;&gt;) 的測試虛設常式</summary>

        public Builder ActionTest<T>(
            Builder target,
            Expression<Func<T>>  instnace,
            Expression<Func<T, Func<float, TICKRESULT>>> tick,
            Expression<Func<T, Action>> start,
            Expression<Func<T, Action>> end
        )
        {
            Builder result = target.Action<T>(instnace, tick, start, end);
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.ActionTest(Builder, !!0, Expression`1<Func`2<!!0,Func`2<Single,TICKRESULT>>>, Expression`1<Func`2<!!0,Action>>, Expression`1<Func`2<!!0,Action>>)
        }

        /// <summary>Action(IAction) 的測試虛設常式</summary>
        
        public Builder ActionTest01(Builder target, Expression<Func<IAction>> action)
        {
            Builder result = target.Action(action);
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.ActionTest01(Builder, IAction)
        }

        /// <summary>Action(Func`2&lt;Single,TICKRESULT&gt;) 的測試虛設常式</summary>
        
        public Builder ActionTest02(Builder target, Func<float, TICKRESULT> func)
        {
            Builder result = target.Action(func);
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.ActionTest02(Builder, Func`2<Single,TICKRESULT>)
        }

        /// <summary>Build() 的測試虛設常式</summary>
        
        public ITicker BuildTest(Builder target)
        {
            ITicker result = target.Build();
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.BuildTest(Builder)
        }

        /// <summary>End() 的測試虛設常式</summary>
        
        public Builder EndTest(Builder target)
        {
            Builder result = target.End();
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.EndTest(Builder)
        }

        /// <summary>Parallel(String, Int32, Int32) 的測試虛設常式</summary>
        
        public Builder ParallelTest(
            Builder target,
            string name,
            int num_required_to_fail,
            int num_required_to_succeed
        )
        {
            Builder result = target.Parallel( num_required_to_fail, num_required_to_succeed);
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.ParallelTest(Builder, String, Int32, Int32)
        }

        /// <summary>Selector() 的測試虛設常式</summary>
        
        public Builder SelectorTest(Builder target)
        {
            Builder result = target.Selector();
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.SelectorTest(Builder)
        }

        /// <summary>Sequence() 的測試虛設常式</summary>
        
        public Builder SequenceTest(Builder target)
        {
            Builder result = target.Sequence();
            
            return result;
            // TODO: 將判斷提示加入 方法 BuilderTest.SequenceTest(Builder)
        }
    }
}
