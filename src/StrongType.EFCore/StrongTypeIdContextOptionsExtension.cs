using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StrongType.EFCore
{
    public static class DbContextOptionsBuilderExtension
    {
        public static DbContextOptionsBuilder UseStrongTypeIdConventions(this DbContextOptionsBuilder options)
        {
            var extension = new StrongTypeIdContextOptionsExtension();
            ((IDbContextOptionsBuilderInfrastructure)options).AddOrUpdateExtension(extension);

            return options;
        }
    }

    public class StrongTypeIdContextOptionsExtension : IDbContextOptionsExtension
    {
        public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

        public void ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkStrongTypeIdConventions();
        }

        public void Validate(IDbContextOptions options)
        {
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private string _logFragment = string.Empty;

            public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension) { }

            private new StrongTypeIdContextOptionsExtension Extension
                => (StrongTypeIdContextOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    return "StrongTypeId Convention";
                }
            }

			public override long GetServiceProviderHashCode()
			{
                return LogFragment.GetHashCode();
			}

			public override void PopulateDebugInfo([NotNullAttribute] IDictionary<string, string> debugInfo)
			{
			}
		}
    }
}
