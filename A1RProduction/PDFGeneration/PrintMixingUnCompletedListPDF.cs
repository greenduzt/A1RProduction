using A1QSystem.Model.Production;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.PDFGeneration
{
    public class PrintMixingUnCompletedListPDF
    {
        private List<ProductionHistory> _printingHistory;
        private string _tableHeader1;
        private DateTime _selectedDate;
        private TextFrame addressFrame;
        private DateTime _currentDate;
        private Document document;
        MigraDoc.DocumentObjectModel.Tables.Table table;

        public PrintMixingUnCompletedListPDF(List<ProductionHistory> ph, DateTime selectedDate, string tH)
        {
            _printingHistory = ph;
            _selectedDate = selectedDate;
            _tableHeader1 = tH;
        }

        public Exception CreateProductionPDF()
        {
            Exception res = null;
            _currentDate = DateTime.Now;

            //System.IO.FileInfo file = new System.IO.FileInfo(_workStationType);
            //string filename = file.Name.Remove((file.Name.Length - file.Extension.Length));
            string filename = "mixing_uncompleted_" + _currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";

            //string filename = "Production"+_workStationType + _currentDate.ToString("ddMMyyyy") + ".pdf";
            //string filename = "test.pdf";
            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.FooterDistance = "-2.0cm";
            document.DefaultPageSetup.LeftMargin = "1.2cm";
            document.DefaultPageSetup.RightMargin = "1.2cm";
            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfSharp.Pdf.PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            try
            {

                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/MixingUnCompleted/" + filename);
                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/MixingUnCompleted/" + filename);
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

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "A1 Rubber Mixing UnCompleted List";
            this.document.Info.Subject = "A1 Rubber Mixing UnCompleted List";
            this.document.Info.Author = "A1 Rubber";

            DefineStyles();
            CreatePage();
            //FillContent();

            return this.document;
        }

        void DefineStyles()
        {
            // Get the predefined style Normal.
            MigraDoc.DocumentObjectModel.Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Helvatica";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Helvatica";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            MigraDoc.DocumentObjectModel.Section section = this.document.AddSection();

            // Put a logo in the header
            MigraDoc.DocumentObjectModel.Shapes.Image image = section.Headers.Primary.AddImage("I:/PRODUCTION/QImg/a1rubber_logo.png");
            image.Height = "2.0cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;


            // Create footer
            MigraDoc.DocumentObjectModel.Paragraph paragraph = section.Footers.Primary.AddParagraph();

            this.addressFrame = section.AddTextFrame();
            this.addressFrame.MarginRight = "0";
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "10.2cm";
            this.addressFrame.Left = ShapePosition.Right;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "-0.1cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;


            paragraph = this.addressFrame.AddParagraph("Mixing UnCompleted List");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.1cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.1cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.1cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            paragraph = section.AddParagraph(_tableHeader1.ToUpper());
            paragraph.Format.SpaceBefore = "-0.1cm";
            paragraph.Style = "Reference";
            paragraph.Format.Font.Bold = true;
            
            paragraph.AddTab();

            // Create the item table
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Format.Font.Name = "Helvatica";
            this.table.Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;
            this.table.Borders.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 4.0;
            this.table.BottomPadding = 1.0;


            Column column2 = this.table.AddColumn("2.5cm");//Order No
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("5cm");//Product Code
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("5.5cm");//description
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("1.5cm");//Qty
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("1.5cm");//Type
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("2.5cm");//Mixing date
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;


            

            Row row1 = table.AddRow();
            row1.HeadingFormat = true;
            row1.Format.Alignment = ParagraphAlignment.Center;
            row1.Format.Font.Bold = true;
            row1.BottomPadding = 1;

            row1.Cells[0].AddParagraph("Order No");
            row1.Cells[0].Format.Font.Size = 10;
            row1.Cells[0].Format.Font.Bold = true;
            row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[1].AddParagraph("Product Code");
            row1.Cells[1].Format.Font.Size = 10;
            row1.Cells[1].Format.Font.Bold = true;
            row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row1.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[2].AddParagraph("Description");
            row1.Cells[2].Format.Font.Size = 10;
            row1.Cells[2].Format.Font.Bold = true;
            row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row1.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[3].AddParagraph("Qty");
            row1.Cells[3].Format.Font.Size = 10;
            row1.Cells[3].Format.Font.Bold = true;
            row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[4].AddParagraph("Type");
            row1.Cells[4].Format.Font.Size = 10;
            row1.Cells[4].Format.Font.Bold = true;
            row1.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[5].AddParagraph("Intended Mixing Date");
            row1.Cells[5].Format.Font.Size = 10;
            row1.Cells[5].Format.Font.Bold = true;
            row1.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

           
            foreach (var item in _printingHistory)
            {
                Row row = table.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.BottomPadding = 1;

                row.Cells[0].AddParagraph(item.SalesOrder.ID.ToString());
                row.Cells[0].Format.Font.Size = 10;
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row.Cells[1].AddParagraph(item.RawProduct.RawProductCode);
                row.Cells[1].Format.Font.Size = 10;
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row.Cells[2].AddParagraph(item.RawProduct.Description);
                row.Cells[2].Format.Font.Size = 10;
                row.Cells[2].Format.Font.Bold = false;
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row.Cells[3].AddParagraph(item.Qty.ToString());
                row.Cells[3].Format.Font.Size = 10;
                row.Cells[3].Format.Font.Bold = false;
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row.Cells[4].AddParagraph(item.RawProduct.RawProductType);
                row.Cells[4].Format.Font.Size = 10;
                row.Cells[4].Format.Font.Bold = false;
                row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row.Cells[5].AddParagraph(item.SalesOrder.MixingDate.ToString("dd/MM/yyyy"));
                row.Cells[5].Format.Font.Size = 10;
                row.Cells[5].Format.Font.Bold = false;
                row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                              
            }

            //Footer page numbers
            TextFrame textFrame2 = section.Footers.Primary.AddTextFrame();
            textFrame2.Width = "18cm";
            textFrame2.Top = "10cm";
            Paragraph footerPara2 = textFrame2.AddParagraph();
            footerPara2.AddText("Printed Date - " + DateTime.Now.ToString("dd/MM/yyy"));
            footerPara2.Format.Font.Size = 7;
            footerPara2.Format.Alignment = ParagraphAlignment.Left;

            footerPara2 = textFrame2.AddParagraph();
            footerPara2.AddText("Printed Time - " + DateTime.Now.ToString("hh:mm tt"));
            footerPara2.Format.Font.Size = 7;
            footerPara2.Format.Alignment = ParagraphAlignment.Left;

            TextFrame textFrame1 = section.Footers.Primary.AddTextFrame();
            textFrame1.Width = "18.2cm";
            Paragraph footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("www.a1rubber.com");
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Right;

            footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("©Copyright A1Rubber " + DateTime.Now.Year.ToString());
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Right;

            footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("Page");
            footerPara1.AddPageField();
            footerPara1.AddText(" of ");
            footerPara1.AddNumPagesField();
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Center;
        }
    }
}
