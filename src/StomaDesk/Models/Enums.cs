namespace StomaDesk.Models
{
    public enum ToothState
    {
        Healthy,
        Caries,
        Filling,
        RootCanal,
        Crown,
        Implant,
        Extracted
    }

    public enum TreatmentStatus
    {
        Proposed,
        Accepted,
        InProgress,
        Done
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        Arrived,
        Done,
        NoShow,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        Transfer
    }
}
