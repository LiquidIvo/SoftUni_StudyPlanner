using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.StudyTask;
using StudyPlanner.ViewModels.StudyTask;


namespace StudyPlanner.Services.Core.Services
{ 
    public class PdfService : IPdfService
    {
        public byte[] GenerateStudyTaskPdf(StudyTaskDetailsDTO task)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .Text($"Study Task: {task.Title}")
                        .FontSize(20)
                        .Bold();

                    page.Content()
                        .Column(column =>
                        {
                            
                            column.Item().Text($"Description: {task.Description}");
                            column.Item().Text($"Due Date: {task.DueDate:dd MMM yyyy}");
                            column.Item().Text($"Priority: {task.Priority}");
                            column.Item().Text($"Status: {task.Status}");
                            column.Item().Text($"Category: {task.Category}");
                            column.Item().Text($"Subject: {task.Subject}");

                            column.Item().PaddingVertical(20);

                            
                            column.Item().Text("Study Sessions").FontSize(16).Bold();

                            foreach (var session in task.StudySessions)
                            {
                                column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                column.Item().Text($"Date: {session.StartTime:dd MMM yyyy}");
                                column.Item().Text($"Time: {session.StartTime:HH:mm} - {session.EndTime:HH:mm}");
                                column.Item().Text($"Duration: {(session.EndTime - session.StartTime).TotalHours:F2} hours");
                                column.Item().Text($"Notes: {session.Notes}");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}

