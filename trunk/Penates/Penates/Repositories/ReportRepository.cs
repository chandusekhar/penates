namespace Penates.Repositories
{
    using System.Collections.Generic;

    public class ReportRepository
    {
        //public PromoSubscription Get(string email)
        //{
            //var db = new PenatesDataContext();

            //var query = from s in db.GetTable<PromoSubscription>() where s.Email == email select s;

            //return query.FirstOrDefault();
        //}

        //public void Add(Models.Subscription subscription)
        //{
        //    var db = new PenatesDataContext();

        //    db.PromoSubscriptions.InsertOnSubmit(GetSubscriptionDbEntity(subscription));

        //    db.SubmitChanges();
        //}

        //public List<Models.Subscription> GetAllSubscriptions()
        //{
        //    var subscriptionsResult = new List<Models.Subscription>();

        //    var db = new PenatesDataContext();

        //    List<GetSubscriptionsResult> result = db.GetSubscriptions().ToList();

        //    SubscriptionsMapper(result, subscriptionsResult);

        //    return subscriptionsResult;
        //}

        //private static void SubscriptionsMapper(IEnumerable<GetSubscriptionsResult> result, List<Models.Subscription> subscriptionsResult)
        //{
        //    subscriptionsResult.AddRange(result.Select(t => new Models.Subscription()
        //    {
        //        Email = t.Email,
        //        Name = t.Name,
        //        Telephone = t.Telephone,
        //        WebSite = t.Website,
        //        DisplayName = t.DisplayName,
        //        DisplayWebSite = t.DisplayWebsite,
        //        Promo3Instalments = t.Promo3Instalments,
        //        Promo6Instalments = t.Promo6Instalments,
        //        Promo9Instalments = t.Promo9Instalments,
        //        Promo12Instalments = t.Promo12Instalments
        //    }));
        //}

        //private static PromoSubscription GetSubscriptionDbEntity(Models.Subscription subscription)
        //{
        //    var subscriptionDbEntity = new PromoSubscription
        //        {
        //            Email = subscription.Email,
        //            Name = subscription.Name,
        //            Telephone = subscription.Telephone,
        //            Website = subscription.WebSite,
        //            Promo3Instalments = subscription.Promo3Instalments,
        //            Promo6Instalments = subscription.Promo6Instalments,
        //            Promo9Instalments = subscription.Promo9Instalments,
        //            Promo12Instalments = subscription.Promo12Instalments,
        //            PromoDescription = subscription.PromoDescription,
        //            Timestamp = DateTime.Now
        //        };

        //    return subscriptionDbEntity;
        //}
    }
}