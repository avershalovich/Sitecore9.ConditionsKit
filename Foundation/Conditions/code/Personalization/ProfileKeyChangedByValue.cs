using Sitecore.Analytics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Linq;

namespace Foundation.Conditions.Personalization
{
    public class ProfileKeyChangedByValue<T> : IntegerComparisonCondition<T> where T : RuleContext
    {
        public Guid ProfileKeyDefinitionId { get; set; }
        public Guid ProfileDefinitionId { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (!Tracker.Current.IsActive || Tracker.Current.Contact == null || Tracker.Current.Contact.Identifiers==null || Tracker.Current.Contact.Identifiers.Count<1) return false;
            var id = Tracker.Current.Contact.Identifiers.First(x => x.Source == Foundation.Conditions.Constants.XdbTrackerSource);
            return EvaluateProfileKey(id.Source, id.Identifier);
        }

        public bool EvaluateProfileKey(string source, string identifier)
        {
          
            using (var client = SitecoreXConnectClientConfiguration.GetClient())
            {

                    var reference = new IdentifiedContactReference(source, identifier);
                    var contact = client.Get(reference, new ContactExpandOptions(PersonalInformation.DefaultFacetKey)
                    {
                        Interactions = new RelatedInteractionsExpandOptions(LocaleInfo.DefaultFacetKey, ProfileScores.DefaultFacetKey)
                        {
                            EndDateTime = DateTime.MaxValue,
                            Limit = 3
                        }
                    });

                var twoLatests = contact?.Interactions.OrderByDescending(x => x.EndDateTime).Take(2).ToList();

                if (twoLatests?.Count != 2) return false;

                var last = GetScore(twoLatests[0]);
                var previous = GetScore(twoLatests[1]);
                int diff = (int)Math.Ceiling(last - previous);

                return Compare(diff);
            }
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