using System.Diagnostics;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.Extensions.Html;
using MigraDocCore.Extensions.Markdown;
using MigraDocCore.Rendering;

namespace MigraDocCore.Extensions.Examples {
    internal class Program {
        static void Main(string[] args) {
            new Program().Run();
        }

        private string outputName = "output.pdf";

        internal void Run() {
            if (File.Exists(outputName)) {
                File.Delete(outputName);
            }

            var doc = new Document();
            StyleDocument(doc);
            var section = doc.AddSection();

            section.Headers.Primary.AddMarkdown("**This** is a *header* rendered by Markdown.");

            var html = File.ReadAllText("example.html");
            section.AddHtml(html);

            section.AddPageBreak();

            var markdown = File.ReadAllText("example.md");
            section.AddMarkdown(markdown);

            section.AddPageBreak();

            section.AddParagraph().AddMarkdown("A **Markdown** formatted *paragraph*.");

            var table = section.AddTable();
            table.AddColumn("10cm");
            table.AddColumn("8cm");

            var row = table.AddRow();
            row.Cells[0].AddMarkdown(markdown);
            row.Cells[1].AddParagraph("Second cell");

            section.Footers.Primary.AddMarkdown("This is a **formatted** footer with *fancy* text.");

            var renderer = new PdfDocumentRenderer();
            renderer.Document = doc;
            renderer.RenderDocument();

            renderer.Save(outputName);

            var psi = new ProcessStartInfo {
                FileName = outputName,
                UseShellExecute = true,
            };
            Process.Start(psi);
        }

        private void StyleDocument(Document doc) {
            Color green = new Color(108, 179, 63),
                  brown = new Color(88, 71, 76),
                  lightbrown = new Color(150, 132, 126);

            var body = doc.Styles["Normal"];

            body.Font.Size = Unit.FromPoint(10);
            body.Font.Color = new Color(51, 51, 51);

            body.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
            body.ParagraphFormat.LineSpacing = 1.25;
            body.ParagraphFormat.SpaceAfter = 10;

            var footer = doc.Styles["Footer"];
            footer.Font.Size = Unit.FromPoint(9);
            footer.Font.Color = lightbrown;

            var h1 = doc.Styles["Heading1"];
            h1.Font.Color = brown;
            h1.Font.Bold = true;
            h1.Font.Size = Unit.FromPoint(15);

            var h2 = doc.Styles["Heading2"];
            h2.Font.Color = green;
            h2.Font.Bold = true;
            h2.Font.Size = Unit.FromPoint(13);

            var h3 = doc.Styles["Heading3"];
            h3.Font.Bold = true;
            h3.Font.Color = Colors.Black;
            h3.Font.Size = Unit.FromPoint(11);

            var links = doc.Styles["Hyperlink"];
            links.Font.Color = green;

            var unorderedlist = doc.AddStyle("UnorderedList", "Normal");
            var listInfo = new ListInfo();
            listInfo.ListType = ListType.BulletList1;
            unorderedlist.ParagraphFormat.ListInfo = listInfo;
            unorderedlist.ParagraphFormat.LeftIndent = "1cm";
            unorderedlist.ParagraphFormat.FirstLineIndent = "-0.5cm";
            unorderedlist.ParagraphFormat.SpaceAfter = 0;

            var orderedlist = doc.AddStyle("OrderedList", "UnorderedList");
            orderedlist.ParagraphFormat.ListInfo.ListType = ListType.NumberList1;

            // for list spacing (since MigraDoc doesn't provide a list object that we can target)
            var listStart = doc.AddStyle("ListStart", "Normal");
            listStart.ParagraphFormat.SpaceAfter = 0;
            listStart.ParagraphFormat.LineSpacing = 0.5;
            var listEnd = doc.AddStyle("ListEnd", "ListStart");
            listEnd.ParagraphFormat.LineSpacing = 1;

            var hr = doc.AddStyle("HorizontalRule", "Normal");
            var hrBorder = new Border();
            hrBorder.Width = "1pt";
            hrBorder.Color = Colors.DarkGray;
            hr.ParagraphFormat.Borders.Bottom = hrBorder;
            hr.ParagraphFormat.LineSpacing = 0;
            hr.ParagraphFormat.SpaceBefore = 15;
        }
    }
}
