using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Foundation.Conditions.Segmentation
{
    public class PageVisitedInThePast : ICondition, IContactSearchQueryFactory
    {
        public Guid ItemId { get; set; }
        public int Number { get; set; }
        public TimeUnit Unit { get; set; }
       
        public bool Evaluate(IRuleExecutionContext context)
        {
            Contact contact = context.Fact<Contact>();
            DateTime minDate = DateTime.UtcNow.SubtractTimeUnit(Unit, Number);
            if (contact.Interactions == null)
                return false;
            return contact.Interactions.Any(i => i.Events.Any(e =>
            {
                if (e.DefinitionId == Foundation.Conditions.Constants.PageViewEventId && e.ItemId == ItemId)
                    return e.Timestamp >= minDate;
                return false;
            }));
        }

        public Expression<Func<Contact, bool>> CreateContactSearchQuery(IContactSearchQueryContext context)
        {
            DateTime minDate = DateTime.UtcNow.SubtractTimeUnit(Unit, Number);

            return contact => contact.Interactions
                .Any(i => i.Events
                    .Any(e => e.DefinitionId == Foundation.Conditions.Constants.PageViewEventId && e.ItemId == ItemId && e.Timestamp >= minDate));

        }
    }
}