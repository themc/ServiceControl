﻿namespace ServiceControl.Plugin.CustomChecks
{
    using System;
    using System.Collections.Generic;
    using EndpointPlugin.Operations.ServiceControlBackend;
    using Internal;
    using NServiceBus;
    using Messages;

    public abstract class CustomCheck : ICustomCheck
    {
        static string hostId;

        static CustomCheck()
        {
            string hostId;
            Dictionary<string, string> hostProperties;

            HostInformationRetriever.TryToRetrieveHostInfo(out hostId, out hostProperties);

            CustomCheck.hostId = hostId;
        }

        protected CustomCheck(string id, string category)
        {
            this.category = category;
            this.id = id;
        }

        public string Category
        {
            get { return category; }
        }

        public void ReportPass()
        {
            ReportToBackend(CheckResult.Pass);
        }


        public void ReportFailed(string failureReason)
        {
            ReportToBackend(CheckResult.Failed(failureReason));
        }

        public string Id
        {
            get { return id; }
        }

        void ReportToBackend(CheckResult result)
        {
            Configure.Instance.Builder.Build<ServiceControlBackend>().Send(new ReportCustomCheckResult
            {
                HostId = hostId,
                CustomCheckId = id,
                Category = category,
                Result = result,
                ReportedAt = DateTime.UtcNow
            });
        }

        readonly string category;
        readonly string id;
    }
}