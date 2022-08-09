using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using KendoCRUDService.Models.Response;
using KendoCRUDService.Models.Request;

namespace KendoCRUDService.Common
{
    public class GroupingExtension
    {
        public static IList ApplyGrouping<T>(IList data, List<GroupRequest> groups)
        {
            if (groups.Count > 0)
            {
                string field = groups.FirstOrDefault().Field;
                int count = data.Count;

                if (count <= 0)
                {
                    return null;
                }

                var value = CommonExtension.GetValueByPropertyName(data[0], field);
                var result = new List<Group>();
                var aggregates = groups.FirstOrDefault().Aggregates;

                bool hasSubgroups = groups.Count > 1;
                var groupItem = AddGroup<T>(field, hasSubgroups, value);

                for (int i = 0; i < count; i++)
                {
                    var item = data[i];
                    var currentValue = CommonExtension.GetValueByPropertyName(item, field);

                    if (!currentValue.Equals(value))
                    {
                        if (groups.Count > 1)
                        {
                            var subGroups = new List<GroupRequest>(groups);
                            subGroups.Remove(subGroups.FirstOrDefault());
                            groupItem.Items = ApplyGrouping<T>(groupItem.Items, subGroups);
                        }

                        result.Add(groupItem);

                        groupItem = AddGroup<T>(field, hasSubgroups, currentValue);
                        value = currentValue;
                    }

                    groupItem.Items.Add(item);
                    groupItem.Aggregates = AggregateExtension.CalculateAggregates(aggregates, groupItem.Items);

                    if (groupItem.HasSubgroups)
                    {
                        groupItem.SubgroupCount = groupItem.Items.Count;
                    }
                    else
                    {
                        groupItem.ItemCount = groupItem.Items.Count;
                    }
                }

                if (groups.Count > 1)
                {
                    var subGroups = new List<GroupRequest>(groups);
                    subGroups.Remove(subGroups.FirstOrDefault());
                    groupItem.Items = ApplyGrouping<T>(groupItem.Items, subGroups);
                }

                result.Add(groupItem);

                return result;
            }

            return null;
        }

        private static Group AddGroup<T>(string field, bool hasSubgroups, object value)
        {
            return new Group()
            {
                Field = field,
                HasSubgroups = hasSubgroups,
                Value = value,
                Items = new List<T>(),
                Aggregates = new Dictionary<string, Dictionary<string, string>>()
            };
        }
    }
}