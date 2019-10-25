using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;

namespace Foundation.Conditions.Segmentation
{
    public class ProfileKeyMatches : ICondition//, IContactSearchQueryFactory
    {
        public Guid ProfileKeyDefinitionId { get; set; }

        public Guid ProfileDefinitionId { get; set; }

        public double Number { get; set; }

        public NumericOperationType Comparison { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            Contact contact = context.Fact<Contact>();
            if (Comparison == NumericOperationType.IsNotEqualTo)
            {
                if (contact.ContactBehaviorProfile()?.Scores != null)
                {
                    return contact.ContactBehaviorProfile().Scores.All(y =>
                    {
                        if (!(y.Value.ProfileDefinitionId != ProfileDefinitionId) && y.Value.Values.ContainsKey(ProfileKeyDefinitionId))
                            return y.Value.Values[ProfileKeyDefinitionId] != Number;
                        return true;
                    });
                }
                return true;
            }

            if (contact.ContactBehaviorProfile()?.Scores == null)
                return false;

                return contact.ContactBehaviorProfile().Scores.Any(y =>
            {
                if (y.Value.ProfileDefinitionId == ProfileDefinitionId && y.Value.Values.ContainsKey(ProfileKeyDefinitionId))
                    return Comparison.Evaluate(y.Value.Values[ProfileKeyDefinitionId], Number);
                return false;
            });
        }

        //public Expression<Func<Contact, bool>> CreateContactSearchQuery(IContactSearchQueryContext context)
        //{
        //    return contact => false;
        //    //return (Expression<Func<Contact, bool>>)(contact => contact.InteractionsCache().InteractionCaches.Any<InteractionCacheEntry>((Func<InteractionCacheEntry, bool>)(i => i.ProfileScores.Any<ProfileScoreCacheEntry>((Func<ProfileScoreCacheEntry, bool>)(y => y.ProfileDefinitionId == this.ProfileDefinitionId && y.MatchedPatternId == (Guid?)this.MatchedPatternId)))));

        //    //return contact => contact.ContactBehaviorProfile().Scores
        //    //    .Where(y => y.Value.ProfileDefinitionId == ProfileDefinitionId)
        //    //    .Any(x => true);

        //    //if (Comparison == NumericOperationType.IsNotEqualTo)
        //    //{
        //    //    return contact => !contact.ContactBehaviorProfile().Scores
        //    //        .Where(y => y.Value.ProfileDefinitionId == ProfileDefinitionId && y.Value.Values.ContainsKey(ProfileKeyDefinitionId))
        //    //        .Any(y => Comparison.Evaluate(y.Value.Values[ProfileKeyDefinitionId], Number));
        //    //}
        //    //else
        //    //{
        //    //    return contact => contact.ContactBehaviorProfile().Scores
        //    //        .Where(y => y.Value.ProfileDefinitionId == ProfileDefinitionId && y.Value.Values.ContainsKey(ProfileKeyDefinitionId))
        //    //        .Any(y => Comparison.Evaluate(y.Value.Values[ProfileKeyDefinitionId], Number));
        //    //}
        //}
    }
}