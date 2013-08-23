﻿namespace ServiceControl.EndpointPlugin.Infrastructure.PerformanceCounters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using NServiceBus;
    using Operations.Heartbeats;

    public class PerformanceCounterCapturer : IWantToRunWhenBusStartsAndStops, INeedInitialization
    {
        public void Init()
        {
            Configure.Component<PerformanceCounterCapturer>(DependencyLifecycle.SingleInstance);
        }

        public void Start()
        {
            captureTimer = new Timer(CaptureCounters, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }


        public void Stop()
        {
            captureTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
        }

        public List<DataPoint> GetCollectedData(string counter)
        {
            List<DataPoint> data;

            if (collectedData.TryRemove(counter, out data))
            {
                return data;
            }

            return new List<DataPoint>();
        }

        void CaptureCounters(object state)
        {
            foreach (var counter in monitoredCounters)
            {
                var counterName = counter.Key;
                var capturedValue = new DataPoint
                {
                    Time = DateTime.UtcNow,
                    Value = counter.Value.RawValue
                };


                collectedData.AddOrUpdate(counterName, new List<DataPoint> {capturedValue}, (k, existing) =>
                {
                    existing.Add(capturedValue);
                    return existing;
                });
            }
        }

        public void EnableCapturing(string counterCategory, string counterName, string instanceName, string counterKey)

        {
            if (monitoredCounters.ContainsKey(counterKey))
            {
                return;
            }

            var counter = new PerformanceCounter(counterCategory, counterName, instanceName, true);

            monitoredCounters.Add(counterKey, counter);
        }


        static readonly Dictionary<string, PerformanceCounter> monitoredCounters =
            new Dictionary<string, PerformanceCounter>();

        static readonly ConcurrentDictionary<string, List<DataPoint>> collectedData =
            new ConcurrentDictionary<string, List<DataPoint>>();

        Timer captureTimer;
    }
}