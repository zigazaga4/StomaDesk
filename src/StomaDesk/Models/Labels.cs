using System;
using System.Collections.Generic;

namespace StomaDesk.Models
{
    /// <summary>
    /// Romanian display text for every enum the UI shows. Kept in one place so a label
    /// changes once and every grid, combo box and printout follows.
    /// </summary>
    public static class Labels
    {
        public static string For(ToothState value)
        {
            switch (value)
            {
                case ToothState.Healthy: return "Sănătos";
                case ToothState.Caries: return "Carie";
                case ToothState.Filling: return "Obturație";
                case ToothState.RootCanal: return "Tratament de canal";
                case ToothState.Crown: return "Coroană";
                case ToothState.Implant: return "Implant";
                case ToothState.Extracted: return "Extras";
                default: return value.ToString();
            }
        }

        public static string For(TreatmentStatus value)
        {
            switch (value)
            {
                case TreatmentStatus.Proposed: return "Propus";
                case TreatmentStatus.Accepted: return "Acceptat";
                case TreatmentStatus.InProgress: return "În lucru";
                case TreatmentStatus.Done: return "Finalizat";
                default: return value.ToString();
            }
        }

        public static string For(AppointmentStatus value)
        {
            switch (value)
            {
                case AppointmentStatus.Scheduled: return "Programat";
                case AppointmentStatus.Confirmed: return "Confirmat";
                case AppointmentStatus.Arrived: return "Prezent";
                case AppointmentStatus.Done: return "Finalizat";
                case AppointmentStatus.NoShow: return "Neprezentat";
                case AppointmentStatus.Cancelled: return "Anulat";
                default: return value.ToString();
            }
        }

        public static string For(PaymentMethod value)
        {
            switch (value)
            {
                case PaymentMethod.Cash: return "Numerar";
                case PaymentMethod.Card: return "Card";
                case PaymentMethod.Transfer: return "Transfer bancar";
                default: return value.ToString();
            }
        }
    }

    /// <summary>A value plus the text shown for it. Combo boxes hold these instead of raw enums.</summary>
    public sealed class Choice<T>
    {
        private readonly T _value;
        private readonly string _text;

        public Choice(T value, string text)
        {
            _value = value;
            _text = text;
        }

        public T Value
        {
            get { return _value; }
        }

        public string Text
        {
            get { return _text; }
        }

        public override string ToString()
        {
            return _text;
        }
    }

    public static class Choices
    {
        public static List<Choice<T>> FromEnum<T>(Func<T, string> label) where T : struct
        {
            var list = new List<Choice<T>>();
            foreach (T value in Enum.GetValues(typeof(T)))
                list.Add(new Choice<T>(value, label(value)));
            return list;
        }
    }
}
