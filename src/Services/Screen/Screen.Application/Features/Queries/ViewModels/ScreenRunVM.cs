
using CQRS.Core.Domain;

namespace Screen.Application.Features.Queries.ViewModels
{
    public class ScreenRunVM : DocMetadata
    {
        public Guid Id { get; set; }
        public Guid ScreenId { get; set; }

        public object Library { get; set; }
        public object Protocol { get; set; }
        public object LibrarySize { get; set; }
        public object Scientist { get; set; }
        public object StartDate { get; set; }
        public object EndDate { get; set; }
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