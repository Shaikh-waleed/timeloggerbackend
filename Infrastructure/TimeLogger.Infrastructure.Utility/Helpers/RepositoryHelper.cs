using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public static class RepositoryHelper
    {
        public static object GetMergedValues(EntityEntry entityEntry, IEnumerable<string> updatedProperties)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var properties = entityEntry?.Members?.Where(x => updatedProperties.ToList().Contains(x.Metadata.Name));

            foreach (var property in properties)
                result.Add(property.Metadata.Name, property.CurrentValue);

            return result;
        }
    }
}
