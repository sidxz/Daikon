using System;
using System.Collections.Generic;
using CQRS.Core.Domain;

namespace Aggregators.Application.Disclosure.Dashboard
{
    /*
     * View model for the disclosure dashboard.
     * Acts as a container for the dashboard's tabular data,
     * inheriting metadata context from VMMeta.
     */
    public class DisclosureDashView : VMMeta
    {
        /*
         * A collection of table rows, each representing a molecule along with its
         * horizon relationships and deeply fetched results.
         */
        public List<DisclosureDashTableElemView> TableElements { get; set; } = new();
    }
}
