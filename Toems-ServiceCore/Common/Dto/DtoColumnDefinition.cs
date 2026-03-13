using System;
using System.Linq.Expressions;
using Toems_Common.Entity;

namespace Toems_Common.Dto
{
    public class DtoColumnDefinition
    {
        public Expression<Func<EntityComputer, object>> Property { get; set; }
        public string Title { get; set; }
    }
}