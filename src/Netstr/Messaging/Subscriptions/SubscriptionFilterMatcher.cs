﻿using Netstr.Extensions;
using Netstr.Messaging.Models;

namespace Netstr.Messaging.Subscriptions
{
    public static class SubscriptionFilterMatcher
    {
        /// <summary>
        /// Returns whether the given event <paramref name="e"/> satisfies conditions in <paramref name="filter"/>
        /// </summary>
        public static bool IsMatch(SubscriptionFilter filter, Event e)
        {
            Func<bool>[] filters = [
                () => filter.Ids.EmptyOrAny(x => x == e.Id),
                () => filter.Authors.EmptyOrAny(x => x == e.PublicKey),
                () => filter.Kinds.EmptyOrAny(x => x == e.Kind),
                () => !filter.Since.HasValue || filter.Since <= e.CreatedAt,
                () => !filter.Until.HasValue || filter.Until >= e.CreatedAt,
                () => filter.OrTags.All(tag => e.Tags.Any(x => tag.Key == x[0] && tag.Value.Contains(x[1]))),
                () => filter.AndTags.All(tag => tag.Value.All(tagValue => e.Tags.Any(eTag => eTag[0] == tag.Key && eTag[1] == tagValue)))
            ];

            return filters.All(x => x());
        }
    }
}
