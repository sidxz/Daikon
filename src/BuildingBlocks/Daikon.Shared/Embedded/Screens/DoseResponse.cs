
using Daikon.Shared.Constants.Units;

namespace Daikon.Shared.Embedded.Screens
{
    public class DoseResponse
    {
        /* Concentration of the compound at which the response was measured.
         * Example: 10.0 (meaning 10 μM or 10 nM depending on ConcentrationUnit).
         */
        public double Concentration { get; set; } = 0.0;

        /* Unit of the concentration value.
         * Possible values (suggested constants from Units class):
         * - Units.MicroMolar ("μM")
         * - Units.NanoMolar ("nM")
         * - Units.MilliMolar ("mM")
         * - Units.MicrogramPerMilliliter ("μg/mL")
         * - Units.Picomolar ("pM")
         * Default is μM (MicroMolar).
         */
        public string ConcentrationUnit { get; set; } = ConcentrationUnits.MicroMolar;

        /* Observed biological response at the given concentration.
         * Example: 80.0 (meaning 80% inhibition or 80% viability depending on ResponseType).
         */
        public double Response { get; set; } = 0.0;

        /* Type of response measured.
         * Possible values:
         * - "PercentInhibition" — % reduction of activity compared to control (most common)
         * - "PercentActivation" — % increase of activity compared to control
         * - "SignalIntensity" — Raw instrument signal value (e.g., fluorescence, luminescence)
         * - "RelativeFluorescenceUnits" (RFU)
         * - "RelativeLuminescenceUnits" (RLU)
         * - "Absorbance" (e.g., OD600)
         * - "ViabilityPercent" — % viable cells relative to control
         * - "CellDeathPercent" — % dead cells relative to control
         * - "GrowthPercent" — % cell or bacterial growth
         * - "MigrationInhibitionPercent" — % inhibition of cell migration
         * - "DifferentiationPercent" — % cells differentiated
         * - "BindingPercent" — % target binding
         * Default is "PercentInhibition".
         */
        public string ResponseType { get; set; } = "PercentInhibition";

        /* Unit of the response value.
         * Possible values (suggested constants from Units class):
         * - Units.Percent ("%")
         * - Units.RelativeFluorescenceUnits ("RFU")
         * - Units.RelativeLuminescenceUnits ("RLU")
         * - Units.Absorbance ("Absorbance Units")
         * - Units.SignalIntensity ("Raw Signal")
         * Default is "%".
         */
        public string ResponseUnit { get; set; } = ResponseUnits.Percent;

        /* Optional: Time point at which the response was measured (in hours).
         * Example: 24.0 (24 hours), 48.0 (48 hours).
         * Useful for time-course experiments like measuring viability over time.
         * Nullable — leave empty if not applicable.
         */
        public double? TimePoint { get; set; }

        /* Optional: Free-text comment for this data point.
         * Example: "Outlier", "Control sample", "High background observed".
         * Nullable — leave empty if not applicable.
         */
        public string? Comment { get; set; }

        /* Optional: Whether this data point represents a control measurement.
         * Example: True if DMSO-treated well or positive control compound.
         * Nullable — leave empty if not specified.
         */
        public bool? IsControl { get; set; }
    }

}
