using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrongType.EFCore.Npgsql
{
    public static class CustomNpgsqlConventionBuilderExtension
    {
        public static DbContextOptionsBuilder UseCustomStrongTypeNpgsqlBuilder(this DbContextOptionsBuilder options)
        {
            options.ReplaceService<IProviderConventionSetBuilder, CustomNpgsqlConventionSetBuilder>();
            return options;
        }
    }
}
