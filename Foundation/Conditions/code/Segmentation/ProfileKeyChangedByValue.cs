using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;

namespace Foundation.Conditions.Segmentation
{
    public class ProfileKeyChangedByValue : ICondition
    {
        public Guid ProfileKeyDefinitionId { get; set; }

        public Guid ProfileDefinitionId { get; set; }

        public double Number { get; set; }

        public NumericOperationType Comparison { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            Contact contact = context.Fact<Contact>();

            var twoLatests = contact.Interactions.OrderByDescending(x => x.StartDateTime).Take(2).ToList();
            if (twoLatests.Count != 2) return false;

            var last = GetScore(twoLatests[0]);
            var previous = GetScore(twoLatests[1]);
            int diff = (int)Math.Ceiling(last - previous);

            return Comparison.Evaluate(diff, Number);

        }

        public double GetScore(Interaction interaction)
        {
            var profile = interaction.ProfileScores()?.Scores?.FirstOrDefault(x => x.Value.ProfileDefinitionId == this.ProfileDefinitionId);
            if (profile != null && profile.Value.Value.Values.ContainsKey(ProfileKeyDefinitionId))
            {
                var score = profile.Value.Value.Values[ProfileKeyDefinitionId];
                return score;
            }
            return 0;
        }
    }
}