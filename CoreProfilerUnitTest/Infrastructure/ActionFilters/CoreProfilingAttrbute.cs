using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreProfiler;

namespace CoreProfilerUnitTest.Infrastructure.ActionFilters
{
    public class CoreProfilingAttrbute : ActionFilterAttribute
    {
        public string ProfilingName { get; set; }

        public IDisposable ProfilingStep { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (string.IsNullOrWhiteSpace(this.ProfilingName))
            {
                this.ProfilingName = context.ActionDescriptor.DisplayName;
            }

            this.ProfilingStep = ProfilingSession.Current.Step(this.ProfilingName);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            this.ProfilingStep?.Dispose();
        }
    }
}