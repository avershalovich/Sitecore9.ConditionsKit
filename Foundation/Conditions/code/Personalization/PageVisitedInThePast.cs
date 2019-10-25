using Sitecore.Analytics;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Segmentation.Predicates;
using System;
using System.Linq;

namespace Foundation.Conditions.Personalization
{
    public class PageVisitedInThePast<T> : IntegerComparisonCondition<T> where T : RuleContext
    {
        public Guid ItemId { get; set; }
        public int Number { get; set; }
        public TimeUnit Unit { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (!Tracker.Current.IsActive || Tracker.Current.Contact == null) return false;

            DateTime minDate = DateTime.UtcNow.SubtractTimeUnit(Unit, Number);
            var id = Tracker.Current.Contact.Identifiers.First(x => x.Source == Foundation.Conditions.Constants.XdbTrackerSource);

            var has = HasPageVisitInPast(id.Source, id.Identifier, this.ItemId, minDate);

            return has;
        }

        public bool HasPageVisitInPast(string source, string identifier, Guid pageId, DateTime from)
        {
            using (var client = SitecoreXConnectClientConfiguration.GetClient())
            {
                try
                {
                    var reference = new IdentifiedContactReference(source, identifier);
                    var contact = client.Get(reference, new ContactExpandOptions(PersonalInformation.DefaultFacetKey)
                    {
                        Interactions = new RelatedInteractionsExpandOptions(LocaleInfo.DefaultFacetKey)
                        {
                            StartDateTime = from,
                            Limit = int.MaxValue
                        }
                    });

                    if (contact == null) return false;

                    return contact.Interactions
                        .Any(i => i.Events
                            .Any(e => e.DefinitionId == Foundation.Conditions.Constants.PageViewEventId && e.ItemId == pageId && e.Timestamp >= from));
                }
                catch (XdbExecutionException ex)
                {
                    Log.Error(ex.Message, ex, this);
                    return false;
                }
            }
        }
    }
}