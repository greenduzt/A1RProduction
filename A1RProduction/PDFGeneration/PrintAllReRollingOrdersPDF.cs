using A1QSystem.Model.Production.ReRoll;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.PDFGeneration
{
    public class PrintAllReRollingOrdersPDF
    {
        private Document document;
        private ObservableCollection<ReRollingOrder> reRollingProduction;
        private string shiftName;
        private DateTime currentDate;
        private DateTime reRollingDate;

        public PrintAllReRollingOrdersPDF(ObservableCollection<ReRollingOrder> rrp)
        {
            reRollingProduction = rrp;
            currentDate = DateTime.Now;
        }

        public Exception PrintReRollingAll(string s)
        {
            shiftName = s;
            Exception res = null;
            AssignValues();
            document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.TopMargin = "3.5cm";
            document.DefaultPageSetup.FooterDistance = "1cm";
            document.DefaultPageSetup.LeftMargin = "1cm";
            document.DefaultPageSetup.RightMargin = "1cm";

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string filename = "ReRolling_All_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";

            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/ReRollLabelsAll/" + filename);
                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/ReRollLabelsAll/" + filename);
                info.Verb = "Print";
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(info);
            }
            catch (Exception ex)
            {
                res = ex;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.Delete(Path.Combine(desktopPath, filename));

            return res;
        }

        private void AssignValues()
        {
            foreach (var item in reRollingProduction)
            {
                reRollingDate = item.ReRollingDate;
            }
        }

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "A1 Rubber Re-Rolling Orders";
            this.document.Info.Subject = "A1 Rubber Re-Rolling Orders";
            this.document.Info.Author = "A1 Rubber";
            this.document.DefaultPageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;

            DefineStyles();
            CreateBody();

            return this.document;
        }

        void DefineStyles()
        {
            MigraDoc.DocumentObjectModel.Style style = this.document.Styles["Normal"];
            style.Font.Name = "Helvatica";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("10cm", MigraDoc.DocumentObjectModel.TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", MigraDoc.DocumentObjectModel.TabAlignment.Center);

            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Helvatica";
            style.Font.Size = 9;

            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", MigraDoc.DocumentObjectModel.TabAlignment.Right);
        }

        void CreateBody()
        {
            Section section = this.document.AddSection();
            Paragraph paragraph = section.AddParagraph();

            Table reRollingTable = new Table();
            reRollingTable = section.AddTable();
            reRollingTable.Style = "Table";
            reRollingTable.Borders.Color = Color.FromCmyk(0, 0, 0, 100);
            reRollingTable.Borders.Width = 1;
            reRollingTable.Borders.Left.Width = 0.5;
            reRollingTable.Borders.Right.Width = 0.5;
            reRollingTable.Rows.LeftIndent = 0;

            Column column = reRollingTable.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = reRollingTable.AddColumn("12cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = reRollingTable.AddColumn("3.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            int coatingRow = 0;//Row number

            reRollingTable.AddRow();
            reRollingTable.Rows[coatingRow].HeadingFormat = true;
            reRollingTable.Rows[coatingRow].Format.Alignment = ParagraphAlignment.Left;
            reRollingTable.Rows[coatingRow].Borders.Left.Width = 0;
            reRollingTable.Rows[coatingRow].Borders.Right.Width = 0;
            reRollingTable.Rows[coatingRow].Borders.Top.Width = 0;
            reRollingTable.Rows[coatingRow].Borders.Bottom.Width = 0;

            reRollingTable.AddRow();
            reRollingTable[1, 0].AddParagraph();
            reRollingTable[1, 0].Format.Alignment = ParagraphAlignment.Center;
            reRollingTable[1, 0].MergeRight = 2;
            reRollingTable[1, 0].Borders.Top.Width = 0;
            reRollingTable[1, 0].Borders.Bottom.Width = 0;
            reRollingTable[1, 0].Borders.Left.Width = 0;
            reRollingTable[1, 2].Borders.Right.Width = 0;
            reRollingTable[1, 0].Format.Font.Size = 30;

            reRollingTable.AddRow();
            reRollingTable[2, 0].AddParagraph("LIST OF RE-ROLLING ORDERS");
            reRollingTable[2, 0].Format.Alignment = ParagraphAlignment.Center;
            reRollingTable[2, 0].MergeRight = 2;
            reRollingTable[2, 0].Borders.Top.Width = 0;
            reRollingTable[2, 0].Borders.Bottom.Width = 0;
            reRollingTable[2, 0].Borders.Left.Width = 0;
            reRollingTable[2, 2].Borders.Right.Width = 0;
            reRollingTable[2, 0].Format.Font.Size = 28;

            reRollingTable.AddRow();
            reRollingTable[3, 0].AddParagraph(reRollingDate.ToString("dd/MM/yyyy") + " (" + reRollingDate.DayOfWeek + " - " + shiftName + ")");
            reRollingTable[3, 0].Format.Alignment = ParagraphAlignment.Center;
            reRollingTable[3, 0].MergeRight = 2;
            reRollingTable[3, 0].Borders.Top.Width = 0;
            reRollingTable[3, 0].Borders.Bottom.Width = 0;
            reRollingTable[3, 0].Borders.Left.Width = 0;
            reRollingTable[3, 2].Borders.Right.Width = 0;
            reRollingTable[3, 0].Format.Font.Size = 22;

            reRollingTable.AddRow();
            reRollingTable[4, 0].AddParagraph();
            reRollingTable[4, 0].Format.Alignment = ParagraphAlignment.Center;
            reRollingTable[4, 0].MergeRight = 2;
            reRollingTable[4, 0].Borders.Top.Width = 0;
            reRollingTable[4, 0].Borders.Bottom.Width = 0;
            reRollingTable[4, 0].Borders.Left.Width = 0;
            reRollingTable[4, 2].Borders.Right.Width = 0;
            reRollingTable[4, 0].Format.Font.Size = 30;

            Table itemTable = new Table();
            itemTable = section.AddTable();
            itemTable.Style = "Table";
            itemTable.Borders.Color = Color.FromCmyk(0, 0, 0, 100);
            itemTable.Borders.Width = 1;
            itemTable.Borders.Left.Width = 0.5;
            itemTable.Borders.Right.Width = 0.5;
            itemTable.Rows.LeftIndent = 0;

            Column col = itemTable.AddColumn("4cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("2.3cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("2.2cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("2.7cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("2cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("6.1cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            itemTable.AddRow();
            itemTable.BottomPadding = 2;
            itemTable.TopPadding = 2;
            itemTable[0, 0].AddParagraph("PRODUCT");
            itemTable[0, 0].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 0].Format.Font.Size = 12;
            itemTable[0, 0].Format.Font.Bold = true;
            itemTable[0, 0].Borders.Bottom.Width = 1;

            itemTable[0, 1].AddParagraph("DENSITY");
            itemTable[0, 1].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 1].Format.Font.Size = 12;
            itemTable[0, 1].Format.Font.Bold = true;
            itemTable[0, 1].Borders.Bottom.Width = 1;

            itemTable[0, 2].AddParagraph("SIZE");
            itemTable[0, 2].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 2].Format.Font.Size = 12;
            itemTable[0, 2].Format.Font.Bold = true;
            itemTable[0, 2].Borders.Bottom.Width = 1;

            itemTable[0, 3].AddParagraph("THICKNESS");
            itemTable[0, 3].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 3].Format.Font.Size = 12;
            itemTable[0, 3].Borders.Bottom.Width = 1;
            itemTable[0, 3].Format.Font.Bold = true;

            itemTable[0, 4].AddParagraph("ROLLS");
            itemTable[0, 4].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 4].Format.Font.Size = 12;
            itemTable[0, 4].Format.Font.Bold = true;
            itemTable[0, 4].Borders.Bottom.Width = 1;

            itemTable[0, 5].AddParagraph("COMMENTS");
            itemTable[0, 5].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 5].Format.Font.Size = 12;
            itemTable[0, 5].Format.Font.Bold = true;
            itemTable[0, 5].Borders.Bottom.Width = 1;

            int x = 1;
            for (int i = 0; i < reRollingProduction.Count; i++)
            {

                itemTable.AddRow();
                itemTable.BottomPadding = 1;
                itemTable.TopPadding = 1;
                itemTable[x, 0].AddParagraph(reRollingProduction[i].Product.ProductName);
                itemTable[x, 0].Format.Alignment = ParagraphAlignment.Left;
                itemTable[x, 0].Format.Font.Size = 12;
                itemTable[x, 0].Borders.Bottom.Width = 1;

                itemTable[x, 1].AddParagraph(reRollingProduction[i].Product.Density);
                itemTable[x, 1].Format.Alignment = ParagraphAlignment.Center;
                itemTable[x, 1].Format.Font.Size = 12;
                itemTable[x, 1].Borders.Bottom.Width = 1;

                itemTable[x, 2].AddParagraph(reRollingProduction[i].Product.Width.ToString("G29") + " x " + reRollingProduction[i].Product.Tile.Height.ToString("G29") + "Lm");
                itemTable[x, 2].Format.Alignment = ParagraphAlignment.Center;
                itemTable[x, 2].Format.Font.Size = 12;
                itemTable[x, 2].Borders.Bottom.Width = 1;

                itemTable[x, 3].AddParagraph(reRollingProduction[i].Product.Tile.Thickness + "mm");
                itemTable[x, 3].Format.Alignment = ParagraphAlignment.Center;
                itemTable[x, 3].Format.Font.Size = 12;
                itemTable[x, 3].Borders.Bottom.Width = 1;

                itemTable[x, 4].AddParagraph(reRollingProduction[i].Rolls.ToString());
                itemTable[x, 4].Format.Alignment = ParagraphAlignment.Right;
                itemTable[x, 4].Format.Font.Size = 12;
                itemTable[x, 4].Borders.Bottom.Width = 1;

                itemTable[x, 5].AddParagraph();
                itemTable[x, 5].Format.Alignment = ParagraphAlignment.Left;
                itemTable[x, 5].Format.Font.Size = 7;
                itemTable[x, 5].Borders.Bottom.Width = 1;
                x++;
            }

            // Create footer
            MigraDoc.DocumentObjectModel.Paragraph footerPara1 = section.Footers.Primary.AddParagraph();
            footerPara1.AddText("Date Printed - " + DateTime.Now.ToString());
            footerPara1.Format.Font.Size = 8;
            footerPara1.Format.Font.Bold = false;
            footerPara1.Format.Alignment = ParagraphAlignment.Left;

            MigraDoc.DocumentObjectModel.Paragraph footerPara2 = section.Footers.Primary.AddParagraph();
            footerPara2.AddText("©Copyright A1Rubber 2019");
            footerPara2.Format.Font.Size = 8;
            footerPara2.Format.Font.Bold = false;
            footerPara2.Format.Alignment = ParagraphAlignment.Right;
        }
    }
}
