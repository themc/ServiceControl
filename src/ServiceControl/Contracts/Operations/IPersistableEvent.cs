namespace ServiceControl.Contracts.Operations
{
    using System;

    public interface IPersistableEvent
    {
        string Id { get; set; }
        DateTime RaisedAt { get; set; }
        string Endpoint { get; set; }
        string Machine { get; set; }
        string Type { get; set; } // actual type name -- EndpointFailedToHearbeat or failedmessages
        string AssociatedMessageId { get; set; } // Id of the document either failed message in errq or hearbeat failed. 
    }
}
