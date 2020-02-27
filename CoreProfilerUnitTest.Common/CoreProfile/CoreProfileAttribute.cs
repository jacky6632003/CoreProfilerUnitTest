using Dora.Interception;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreProfilerUnitTest.Common.CoreProfile
{
    public class CoreProfileAttribute : InterceptorAttribute
    {
        /// <summary>
        /// 以執行位置為監控描述
        /// </summary>
        public CoreProfileAttribute()
        {
        }

        /// <summary>
        /// 以傳入參數為監控描述
        /// </summary>
        /// <param name="stepName"></param>
        public CoreProfileAttribute(string stepName)
        {
            StepName = stepName;
        }

        public string StepName { get; set; }

        /// <summary>
        /// 方法性能監控
        /// </summary>
        /// <param name="bulBuilder"></param>
        public override void Use(IInterceptorChainBuilder builder)
        {
            builder.Use<CoreProfileInterception>(Order, StepName ?? "");
        }
    }
}