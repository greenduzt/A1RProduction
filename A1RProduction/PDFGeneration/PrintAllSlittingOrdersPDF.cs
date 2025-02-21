using A1QSystem.Model.Production.Slitting;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MsgBox;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace A1QSystem.PDFGeneration
{
    public class PrintAllSlittingOrdersPDF
    {
        Document document;
        private string header;
        private string shiftName;
        private DateTime currentDate;
        private DateTime slittingDate;
        private ObservableCollection<SlittingOrder> slittingProduction;

        public PrintAllSlittingOrdersPDF(ObservableCollection<SlittingOrder> sp, string sn, string hdr)
        {
            header = hdr;
            slittingProduction = sp;
            shiftName = sn;
            currentDate = DateTime.Now;
            AssignValues();
            //document = CreateDocument();
            //document.UseCmykColor = true;
            //document.DefaultPageSetup.TopMargin = "3.5cm";
            //document.DefaultPageSetup.FooterDistance = "1cm";
            //document.DefaultPageSetup.LeftMargin = "1cm";
            //document.DefaultPageSetup.RightMargin = "1cm";

            //PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            //pdfRenderer.Document = document;
            //pdfRenderer.RenderDocument();

            //string filename = "SlittingOrder_All_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";

            //try
            //{
            //    pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/SlittingOrdersAll/" + filename);
            //    ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/SlittingOrdersAll/" + filename);
            //    info.Verb = "Print";
            //    info.CreateNoWindow = true;
            //    info.WindowStyle = ProcessWindowStyle.Hidden;
            //    Process.Start(info);
            //}
            //catch (Exception ex)
            //{
            //    //int result = DBAccess.InsertErrorLog(ex.Message);
            //    Msg.Show("The file is currently opened. Please close the file and try again. " + ex.ToString(), "File is in use", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            //}

            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //File.Delete(System.IO.Path.Combine(desktopPath, filename));

        }

        public Exception CreatePDF()
        {
            Exception res = null;
            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.TopMargin = "3.5cm";
            document.DefaultPageSetup.FooterDistance = "1cm";
            document.DefaultPageSetup.LeftMargin = "1cm";
            document.DefaultPageSetup.RightMargin = "1cm";

            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string filename = "SlittingOrder_All_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";
            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/SlittingOrdersAll/" + filename);
                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/SlittingOrdersAll/" + filename);
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
            File.Delete(System.IO.Path.Combine(desktopPath, filename));

            return res;
        }

        private void AssignValues()
        {
            foreach (var item in slittingProduction)
            {
                slittingDate = item.SlittingDate;
            }
        }

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "A1 Rubber Slitting Orders";
            this.document.Info.Subject = "A1 Rubber Slitting Orders";
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

            Table coatingTable = new Table();
            coatingTable = section.AddTable();
            coatingTable.Style = "Table";
            coatingTable.Borders.Color = Color.FromCmyk(0, 0, 0, 100);
            coatingTable.Borders.Width = 1;
            coatingTable.Borders.Left.Width = 0.5;
            coatingTable.Borders.Right.Width = 0.5;
            coatingTable.Rows.LeftIndent = 0;


            Column column = coatingTable.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("12cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("3.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            int coatingRow = 0;//Row number

            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Alignment = ParagraphAlignment.Left;
            coatingTable.Rows[coatingRow].Borders.Left.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Right.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Top.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Bottom.Width = 0;

            coatingTable.AddRow();
            coatingTable[1, 0].AddParagraph();
            coatingTable[1, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[1, 0].MergeRight = 2;
            coatingTable[1, 0].Borders.Top.Width = 0;
            coatingTable[1, 0].Borders.Bottom.Width = 0;
            coatingTable[1, 0].Borders.Left.Width = 0;
            coatingTable[1, 2].Borders.Right.Width = 0;
            coatingTable[1, 0].Format.Font.Size = 30;

            coatingTable.AddRow();
            coatingTable[2, 0].AddParagraph("LIST OF " + header + " SLITTING ORDERS");
            coatingTable[2, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[2, 0].MergeRight = 2;
            coatingTable[2, 0].Borders.Top.Width = 0;
            coatingTable[2, 0].Borders.Bottom.Width = 0;
            coatingTable[2, 0].Borders.Left.Width = 0;
            coatingTable[2, 2].Borders.Right.Width = 0;
            coatingTable[2, 0].Format.Font.Size = 28;

            coatingTable.AddRow();
            coatingTable[3, 0].AddParagraph(slittingDate.ToString("dd/MM/yyyy") + " (" + slittingDate.DayOfWeek + " - " + shiftName + ")");
            coatingTable[3, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[3, 0].MergeRight = 2;
            coatingTable[3, 0].Borders.Top.Width = 0;
            coatingTable[3, 0].Borders.Bottom.Width = 0;
            coatingTable[3, 0].Borders.Left.Width = 0;
            coatingTable[3, 2].Borders.Right.Width = 0;
            coatingTable[3, 0].Format.Font.Size = 22;

            coatingTable.AddRow();
            coatingTable[4, 0].AddParagraph();
            coatingTable[4, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[4, 0].MergeRight = 2;
            coatingTable[4, 0].Borders.Top.Width = 0;
            coatingTable[4, 0].Borders.Bottom.Width = 0;
            coatingTable[4, 0].Borders.Left.Width = 0;
            coatingTable[4, 2].Borders.Right.Width = 0;
            coatingTable[4, 0].Format.Font.Size = 30;


            Table itemTable = new Table();
            itemTable = section.AddTable();
            itemTable.Style = "Table";
            itemTable.Borders.Color = Color.FromCmyk(0, 0, 0, 100);
            itemTable.Borders.Width = 1;
            itemTable.Borders.Left.Width = 0.5;
            itemTable.Borders.Right.Width = 0.5;
            itemTable.Rows.LeftIndent = 0;

            Column col = itemTable.AddColumn("7.9cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("3.7cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("2.7cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("2.7cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            col = itemTable.AddColumn("2cm");
            col.Format.Alignment = ParagraphAlignment.Center;

            itemTable.AddRow();
            itemTable[0, 0].AddParagraph("PRODUCT");
            itemTable[0, 0].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 0].Format.Font.Size = 16;
            itemTable[0, 0].Format.Font.Bold = true;
            itemTable[0, 0].Borders.Bottom.Width = 1;

            itemTable[0, 1].AddParagraph("THICKNESS");
            itemTable[0, 1].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 1].Format.Font.Size = 16;
            itemTable[0, 1].Borders.Bottom.Width = 1;

            itemTable[0, 2].AddParagraph("BLOCKS");
            itemTable[0, 2].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 2].Format.Font.Size = 16;
            itemTable[0, 2].Format.Font.Bold = true;
            itemTable[0, 2].Borders.Bottom.Width = 1;

            itemTable[0, 3].AddParagraph("BLOCKS CUT");
            itemTable[0, 3].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 3].Format.Font.Size = 16;
            itemTable[0, 3].Format.Font.Bold = true;
            itemTable[0, 3].Borders.Bottom.Width = 1;

            itemTable[0, 4].AddParagraph("YIELD");
            itemTable[0, 4].Format.Alignment = ParagraphAlignment.Center;
            itemTable[0, 4].Format.Font.Size = 16;
            itemTable[0, 4].Format.Font.Bold = true;
            itemTable[0, 4].Borders.Bottom.Width = 1;

            int x = 1;
            for (int i = 0; i < slittingProduction.Count; i++)
            {
                itemTable.AddRow();
                itemTable[x, 0].AddParagraph(slittingProduction[i].Product.ProductDescription);
                itemTable[x, 0].Format.Alignment = ParagraphAlignment.Left;
                itemTable[x, 0].Format.Font.Size = 16;
                itemTable[x, 0].Borders.Bottom.Width = 1;

                itemTable[x, 1].AddParagraph(slittingProduction[i].Product.Tile.Thickness + "mm");
                itemTable[x, 1].Format.Alignment = ParagraphAlignment.Right;
                itemTable[x, 1].Format.Font.Size = 16;
                itemTable[x, 1].Borders.Bottom.Width = 1;

                itemTable[x, 2].AddParagraph(slittingProduction[i].Blocks.ToString());
                itemTable[x, 2].Format.Alignment = ParagraphAlignment.Right;
                itemTable[x, 2].Format.Font.Size = 16;
                itemTable[x, 2].Borders.Bottom.Width = 1;

                itemTable[x, 3].AddParagraph(" /" + slittingProduction[i].Blocks.ToString());
                itemTable[x, 3].Format.Alignment = ParagraphAlignment.Right;
                itemTable[x, 3].Format.Font.Size = 16;
                itemTable[x, 3].Borders.Bottom.Width = 1;

                itemTable[x, 4].AddParagraph();
                itemTable[x, 4].Format.Alignment = ParagraphAlignment.Right;
                itemTable[x, 4].Format.Font.Size = 16;
                itemTable[x, 4].Borders.Bottom.Width = 1;

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