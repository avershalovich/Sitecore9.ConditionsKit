using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Foundation.Conditions.Segmentation
{
    public class PatternCardMatches : ICondition, IContactSearchQueryFactory
    {
        public Guid MatchedPatternId { get; set; }

        public Guid ProfileDefinitionId { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            Contact contact = context.Fact<Contact>();
            var scores = contact.ContactBehaviorProfile()?.Scores;
            if (scores != null)
            {
                return scores.Any(y =>
                {
                    if (!(y.Value.ProfileDefinitionId == ProfileDefinitionId))
                        return false;
                    Guid? matchedPatternId1 = y.Value.MatchedPatternId;
                    Guid matchedPatternId2 = MatchedPatternId;
                    if (!matchedPatternId1.HasValue)
                        return false;
                   
                    return matchedPatternId1.GetValueOrDefault() == matchedPatternId2;
                });
            }
            return true;
        }

        public Expression<Func<Contact, bool>> CreateContactSearchQuery(IContactSearchQueryContext context)
        {
            return contact => contact.ContactBehaviorProfile().Scores
            .Any(y => y.Value.ProfileDefinitionId == ProfileDefinitionId && y.Value.MatchedPatternId == MatchedPatternId);
        }
    }
}