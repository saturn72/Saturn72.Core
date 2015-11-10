using System.Collections.Generic;
using Automation.Core.Validation;
using Automation.Extensions;

namespace Automation.Core.Activity
{
    public static class RequestBaseExtension
    {
        public static ValidationPoint RunRequestValidationPoints(this RequestBase request)
        {
            Guard.NotNull(request);
            if (request.ValidationPoints.IsNull())
                return null;

            var vPoints = new List<ValidationPoint>();
            request.ValidationPoints.ForEachItem(vp => vPoints.Add(vp()));

            var vpArr = vPoints.ToArray();
            var result = vpArr[0];

            for (var i = 1; i < vpArr.Length; i++)
                result = result + vpArr[i];

            return result;
        }
    }
}