using CourseLibrary.API.Services;
using System.Linq.Dynamic.Core;

namespace CourseLibrary.API.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source,
            string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictonary)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (mappingDictonary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictonary));
            }

            if (string.IsNullOrEmpty(orderBy))
            {
                return source;
            }

            var orderByString = string.Empty;

            // the orderBy string is seperated by "," so we split it
            var orderByAfterSplit = orderBy.Split(','); 

            // apply each orderby clause
            foreach (var orderByClause in orderByAfterSplit)
            {
                // trim the orderBy clause, as it might contain leading 
                // or trailing spaces. Can't trim the var in foreach,
                // so use another var.
                var trimmedOrderByClause = orderByClause.Trim();

                // if the sort option ends with " desc" we order
                // descending, otherwise ascending
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // remove "asc" or "desc" from the orderBy clause, so we 
                // get the property name to look for in the mapping dictionary

                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                // find the matching property
                if (!mappingDictonary.ContainsKey(propertyName)) {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                //  get the PropertyMappingValue
                var propertyMappingValue = mappingDictonary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentException("propertyMappingValue");
                }

                // revert sort order if necessary
                if (propertyMappingValue.Revert)
                {
                    orderDescending = !orderDescending;
                }

                // run through the property names
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");
                }

                return source.OrderBy(orderByString);
            }
        }
    }
}
