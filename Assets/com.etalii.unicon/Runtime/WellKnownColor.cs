namespace EtAlii.UniCon
{
    using UnityEngine;

    internal static class WellKnownColor
    {
        public const string MarkerTagHexColor = "#11ff1122";
        public const string UnityMarkerTagColor = "#008A17";
        public const string UnityHeaderHexColor = "#DDDDDD";
        
        public static readonly Color Action = Color.green * 0.85f; // ColorUtility.TryParseHtmlString("#11ff1122", out var propertyGridHeaderColor

        // ReSharper disable InconsistentNaming
        public const string LogIconVerboseHexColor = "#5A5A5A";
        public const string LogIconInformationHexColor = "white";
        public const string LogIconDebugHexColor = "#808080";
        public const string LogIconWarningHexColor = "yellow";
        public const string LogIconErrorHexColor = "red";
        public const string LogIconFatalHexColor = "red";
        // ReSharper restore InconsistentNaming
    }
}