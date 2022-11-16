using KendoCoreService.Models.Request;
using KendoCoreService.Models.Response;
using System.Collections;

namespace KendoCoreService.Extensions
{
    public static class GroupingExtension
    {
        public static IQueryable Group<T>(this IQueryable<T> data, List<GroupRequest> groups)
        {
            bool multi = groups.Count > 1;

            // Apply the first grouping manually.
            var grouped = data.GroupBy(x => CommonExtension.GetValueByPropertyName(x, groups[0].Field))
                .GroupSort(groups[0].Dir)
                .Select(x => new Group
                {
                    Value = x.Key,
                    Field = groups[0].Field,
                    SubgroupCount = multi ? x.Count() : 0,
                    ItemCount = multi ? 0 : x.Count(),
                    HasSubgroups = multi,
                    Items = multi ? AddGroup(x, groups, 1) : x.ToList(),
                    Aggregates = x.AsQueryable().CalculateAggregates(groups[0].Aggregates)
                });

            return grouped;
        }

        private static IList AddGroup<T>(IGrouping<object, T> data, List<GroupRequest> groups, int index)
        {
            bool isLast = groups.Count - 1 == index;
            int nextIndex = index + 1;

            if (isLast)
            {
                return data.GroupBy(x => CommonExtension.GetValueByPropertyName(x, groups[index].Field))
                .AsQueryable()
                .GroupSort(groups[index].Dir)
                .Select(x => new Group
                {
                    Value = x.Key,
                    Field = groups[index].Field,
                    ItemCount = x.Count(),
                    Items = x.ToList(),
                    Aggregates = x.AsQueryable().CalculateAggregates(groups[index].Aggregates)
                }).ToList();
            }

            return data.GroupBy(x => CommonExtension.GetValueByPropertyName(x, groups[index].Field))
                .AsQueryable()
                .GroupSort(groups[index].Dir)
                .Select(x => new Group
                {
                    Value = x.Key,
                    Field = groups[index].Field,
                    SubgroupCount = x.Count(),
                    HasSubgroups = true,
                    Items = AddGroup(x, groups, nextIndex),
                    Aggregates = x.AsQueryable().CalculateAggregates(groups[index].Aggregates)
                }).ToList();
        }
    }
}
