using Atracker.Models;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace Atracker.Documents
{
    public class PerformanceReportDocument : IDocument
    {
        private readonly PerformanceReportViewModel _model;
        public PerformanceReportDocument(PerformanceReportViewModel model) { _model = model; }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("Performance Report").SemiBold().FontSize(24);
                page.Content().Text("...");
                // CORRECTED FOOTER:
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                });
            });
        }
    }
}