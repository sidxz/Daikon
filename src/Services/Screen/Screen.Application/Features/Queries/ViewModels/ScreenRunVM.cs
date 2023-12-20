
namespace Screen.Application.Features.Queries.ViewModels
{
    public class ScreenRunVM
    {
        public Guid ScreenId { get; set; }
        public Guid ScreenRunId { get; set; }

        public object Library { get; set; }
        public object Protocol { get; set; }
        public object LibrarySize { get; set; }
        public object Scientist { get; set; }
        public object StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public object UnverifiedHitCount { get; set; }
        public object HitRate { get; set; }
        public object PrimaryHitCount { get; set; }
        public object ConfirmedHitCount { get; set; }
        public object NoOfCompoundsScreened { get; set; }
        public object Concentration { get; set; }
        public object ConcentrationUnit { get; set; }
        public object Notes { get; set; }
    }
}