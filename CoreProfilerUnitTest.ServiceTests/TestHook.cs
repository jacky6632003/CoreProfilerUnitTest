using System;
using AutoMapper;
using CoreProfilerUnitTest.Service.Infrastructure.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreProfilerUnitTest.ServiceTests
{
    [TestClass]
    public class TestHook
    {
        private static IConfigurationProvider _configurationProvider;

        public static IConfigurationProvider MapperConfigurationProvider
        {
            get
            {
                return _configurationProvider ??= new MapperConfiguration
                (
                    x => { x.AddProfile<MappingProfile>(); }
                );
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
            config.AssertConfigurationIsValid();
        }
    }
}