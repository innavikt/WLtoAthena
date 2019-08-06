using System;
using System.Collections.Generic;
using System.Text;
using WealthLab;

namespace v1
{
    class TestStrategyHelper : StrategyHelper
    {
        public override string Name
        {
            get { return "Test Strategy"; }
        }
        public override Guid ID
        {
            get { return new Guid("379FC23F-1C30-42E5-BB50-E90EB43345F7"); }
        }
        public override string Author
        {
            get { return "Inna Software"; }
        }
        public override Type WealthScriptType
        {
            get { return typeof(TestStrategyScript); }
        }
        public override string Description
        {
            get { return "Test Strategy Description"; }
        }
        public override DateTime CreationDate
        {
            get { return new DateTime(2019, 08, 06); }
        }
        public override DateTime LastModifiedDate
        {
            get { return DateTime.Now; }
        }
    }
}
