﻿namespace ServiceControl.EventLog.Definitions
{
    using Contracts.EndpointControl;

    public class NewEndpointDetectedDefinition : EventLogMappingDefinition<NewEndpointDetected>
    {
        public NewEndpointDetectedDefinition()
        {
            Description(m => string.Format("New  '{0}' endpoint detected. In order for this endpoint to be monitored the plugin needs to be installed.", m.Endpoint));

            RelatesToEndpoint(m => m.Endpoint);
            RelatesToMachine(m => m.Machine);

            RaisedAt(m => m.DetectedAt);
        }
    }
}