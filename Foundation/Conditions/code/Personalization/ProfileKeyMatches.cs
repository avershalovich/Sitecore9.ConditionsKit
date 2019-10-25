using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System;
using System.Linq;

namespace Foundation.Conditions.Personalization
{
    public class ProfileKeyMatches<T> : IntegerComparisonCondition<T> where T : RuleContext
    {
        public Guid ProfileKeyDefinitionId { get; set; }

        public Guid ProfileDefinitionId { get; set; }

        //public NumericOperationType Comparison { get; set; }
        protected override bool Execute(T ruleContext)
        {
            if (!Tracker.Current.IsActive || Tracker.Current.Contact == null) return false;

            var profile = Tracker.Current.Contact.BehaviorProfiles?.Profiles?.FirstOrDefault(x => x.Id == new ID(ProfileDefinitionId));

            var op = GetOperator();
            if (op == ConditionOperator.NotEqual && profile == null)
            {
                return true;
            }

            if (profile != null)
            {
                var score = profile.GetScore(new ID(ProfileKeyDefinitionId));
                int value = (int)Math.Ceiling(score);
                var passed = Compare(value);
                return passed;
            }

            return false;

        }
    }
}