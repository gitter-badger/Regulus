﻿using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
namespace PureLibTest
{
    [Binding]
    public class CSVSteps
    {
        public class TestData
        {
            public string field1 {get;set;}
            public int field2 {get;set;}
            public float field3 {get;set;}

        }
        
        [Given(@"資料是(.*)")]
        public void Given資料是(string p0)
        {
            ScenarioContext.Current.Set<string>( p0 , "Text");
            
        }
        [Given(@"段落符號為""(.*)""")]
        public void Given段落符號為(string p0)
        {
            ScenarioContext.Current.Set<string>(p0, "Paragraph");
        }

        [Given(@"分格符號為""(.*)""")]
        public void Given分格符號為(string p0)
        {
            ScenarioContext.Current.Set<string>(p0 , "Separator" );
        }

        [When(@"執行解析")]
        public void When執行解析()
        {
            var text  = ScenarioContext.Current.Get<string>("Text");
            var paragraph = ScenarioContext.Current.Get<string>("Paragraph");
            var separator = ScenarioContext.Current.Get<string>("Separator");

            TestData[] testDatas = Regulus.Utility.CSV.Parse<TestData>(text, separator, paragraph);
                        
            ScenarioContext.Current.Set<TestData[]>(testDatas, "Datas");        
        }
        
        [Then(@"結果為")]
        public void Then結果為(Table table)
        {
            var testDatas = ScenarioContext.Current.Get<TestData[]>("Datas" );
            table.CompareToSet(testDatas);
        }
    }
}