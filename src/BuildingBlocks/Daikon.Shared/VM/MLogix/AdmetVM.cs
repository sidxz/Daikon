using System;
using System.Collections.Generic;
using CQRS.Core.Domain;

namespace Daikon.Shared.VM.MLogix
{
    public class AdmetVM : VMMeta
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "";
        public Dictionary<string, object> Predictions { get; set; }
        public string ModelVersion { get; set; }
        public string Error { get; set; }
    }
}
