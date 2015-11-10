using System;
using System.Collections.Generic;
using System.Linq;
using Automation.Extensions;

namespace Automation.Core.Aspects
{
    public class InvocationData
    {
        public string MethodName { get; set; }
        public IEnumerable<ParameterData> ParametersData { get; set; }
        public Type TargetType { get; set; }
        public object ReturnValue { get; set; }

        public override string ToString()
        {
            var prmContent = ParametersData
                .Select(prm => prm.ToString())
                .Aggregate((first, scond) => "<{0}> <{1}>".AsFormat(first, scond));

            return "Method Name: {0} Parameters: {1}".AsFormat(MethodName, prmContent);
        }
    }
}