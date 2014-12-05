using System;
using System.Collections.Generic;

namespace Penates.Services
{
    public class ReportService
    {
        //public bool Add(Subscription subscription)
        //{
        //    this.ValidateSubscription(subscription);

        //    var subscriptionRepository = new SubscriptionRepository();

        //    if (subscriptionRepository.Get(subscription.Email) != null)
        //    {
        //        return false;
        //    }

        //    subscriptionRepository.Add(subscription);

        //    return true;
        //}

        //public List<Subscription> GetAllSubscriptions()
        //{
        //    var subscriptionRepository = new SubscriptionRepository();

        //    return subscriptionRepository.GetAllSubscriptions();
        //}

        //private void ValidateSubscription(Subscription subscription)
        //{
        //    if (subscription == null)
        //    {
        //        throw new ArgumentNullException("subscription");
        //    }

        //    if (string.IsNullOrEmpty(subscription.Email)
        //        || string.IsNullOrEmpty(subscription.Name)
        //        || string.IsNullOrEmpty(subscription.Telephone)
        //        || (!subscription.Promo3Instalments && !subscription.Promo6Instalments && !subscription.Promo9Instalments && !subscription.Promo12Instalments))
        //    {
        //        throw new Exception("Empty mandatory subscription fields");
        //    }
        //}
    }
}