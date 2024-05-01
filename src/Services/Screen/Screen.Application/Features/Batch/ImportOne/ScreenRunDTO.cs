using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace Screen.Application.Features.Batch.ImportOne
{
    public class ScreenRunDTO : BaseEntity
    {
        public Guid ScreenId { get; set; }
        public string Library { get; set; }
        public string? Protocol { get; set; }
        public string? LibrarySize { get; set; }
        public string? Scientist { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UnverifiedHitCount { get; set; }
        public string? HitRate { get; set; }
        public string? PrimaryHitCount { get; set; }
        public string? ConfirmedHitCount { get; set; }
        public string? NoOfCompoundsScreened { get; set; }
        public string? Concentration { get; set; }
        public string? ConcentrationUnit { get; set; }
        public string? Notes { get; set; }
    }
}