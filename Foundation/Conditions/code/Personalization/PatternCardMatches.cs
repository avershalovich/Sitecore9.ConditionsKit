using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System;

namespace Foundation.Conditions.Personalization
{
    public class PatternCardMatches<T> : IntegerComparisonCondition<T> where T : RuleContext
    {
        public Guid MatchedPatternId { get; set; }

        public Guid ProfileDefinitionId { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (!Tracker.Current.IsActive || Tracker.Current.Contact == null) return false;

            var matched = Tracker.Current.Contact.BehaviorProfiles.IsMatchingPattern(new ID(ProfileDefinitionId), new ID(MatchedPatternId));
          
            return matched;
        }
    }
}