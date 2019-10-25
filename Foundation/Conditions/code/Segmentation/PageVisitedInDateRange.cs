using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Foundation.Conditions.Segmentation
{
    public class PageVisitedInDateRange : ICondition, IContactSearchQueryFactory
    {
        public Guid ItemId { get; set; }

        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            Contact contact = context.Fact<Contact>();
            if (contact.Interactions == null)
                return false;
            return contact.Interactions.Any(i => i.Events.Any(e =>
            {
                if (e.DefinitionId == Foundation.Conditions.Constants.PageViewEventId && e.ItemId == ItemId && e.Timestamp >= MinDate)
                    return e.Timestamp <= MaxDate;
                return false;
            }));
        }

        public Expression<Func<Contact, bool>> CreateContactSearchQuery(IContactSearchQueryContext context)
        {
            return contact => contact.Interactions
            .Any(i => i.Events
                .Any(e => e.DefinitionId == Foundation.Conditions.Constants.PageViewEventId && e.ItemId == this.ItemId && e.Timestamp >= this.MinDate && e.Timestamp <= this.MaxDate));
        }
    }
}