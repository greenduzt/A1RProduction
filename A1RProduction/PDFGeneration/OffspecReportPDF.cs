using A1QSystem.Model.Production.Slitting;
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
    public class OffspecReportPDF
    {
        Document document;
        TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;
        private DateTime fromDate;
        private DateTime toDate;
        private DateTime currentDate;
        private List<OffSpecDetails> offSpecDetails;

        public OffspecReportPDF(List<OffSpecDetails> osd, DateTime f,DateTime t)
        {
            currentDate = DateTime.Now;
            offSpecDetails = new List<OffSpecDetails>();
            offSpecDetails = osd;
            fromDate = f;
            toDate = t;
        }

        public Exception createWorkOrderPDF()
        {
            Exception res = null;
            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.FooterDistance = "-4cm";
            document.DefaultPageSetup.LeftMargin = "1.2cm";
            document.DefaultPageSetup.RightMargin = "1.2cm";
            document.DefaultPageSetup.TopMargin = "3.2cm";

            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string filename = "OS" + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";
            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/OffSpecReport/" + filename);

                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/OffSpecReport/" + filename);
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
            this.document.Info.Title = "Off-Spec Report";
            this.document.Info.Subject = "Off-Spec Report";
            this.document.Info.Author = "Chamara Walaliyadde";
            this.document.DefaultPageSetup.Orientation = Orientation.Landscape;

            DefineStyles();
            CreatePage();
            
            // FillContent();

            return this.document;
        }

        void DefineStyles()
        {
            // Get the predefined style Normal.
            MigraDoc.DocumentObjectModel.Style style = this.document.Styles["Normal"];

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
            MigraDoc.DocumentObjectModel.Section section = this.document.AddSection();
            MigraDoc.DocumentObjectModel.Shapes.Image image = section.Headers.Primary.AddImage("I:/PRODUCTION/QImg/a1rubber_logo.png");
            image.Height = "1.6cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;

            MigraDoc.DocumentObjectModel.Paragraph paragraph = section.AddParagraph();

            this.addressFrame = section.AddTextFrame();
            this.addressFrame.MarginRight = "0";
            this.addressFrame.Height = "3.5cm";
            this.addressFrame.Width = "7cm";
            this.addressFrame.Left = ShapePosition.Right;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.RelativeVertical = RelativeVertical.Page;
            this.addressFrame.MarginTop = "-1.5cm";

            paragraph = this.addressFrame.AddParagraph("Off-Spec Report");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            paragraph = this.addressFrame.AddParagraph("From - " + fromDate.ToString("dd/MM/yyyy") + "  " + " To - " + toDate.ToString("dd/MM/yyyy"));
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-2cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

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
            textFrame1.Width = "27.2cm";
            Paragraph footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("www.a1rubber.com");
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Right;

            footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("©Copyright A1Rubber 2019");
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Right;

            footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("Page");
            footerPara1.AddPageField();
            footerPara1.AddText(" of ");
            footerPara1.AddNumPagesField();
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Center;

            

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            //Table for items
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Format.Font.Name = "Helvatica";
            this.table.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            this.table.Borders.Color = MigraDoc.DocumentObjectModel.Colors.Black;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 1.0;
            this.table.BottomPadding = 1.0;

            Column itemsCol = this.table.AddColumn("1.1cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;

            itemsCol = this.table.AddColumn("2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("5cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            
            itemsCol = this.table.AddColumn("1.2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("0.9cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("0.9cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.6cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("2.2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.4cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("5cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.8cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        

            Row itemRow = table.AddRow();
            itemRow.HeadingFormat = true;
            itemRow.Format.Alignment = ParagraphAlignment.Center;
            itemRow.Format.Font.Bold = true;
            
            itemRow.Cells[0].AddParagraph("Order No");
            itemRow.Cells[0].Format.Font.Size = 9;
            itemRow.Cells[0].Format.Font.Bold = true;
            itemRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[1].AddParagraph("Product Code");
            itemRow.Cells[1].Format.Font.Size = 9;            
            itemRow.Cells[1].Format.Font.Bold = true;
            itemRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;   

            itemRow.Cells[2].AddParagraph("Description");
            itemRow.Cells[2].Format.Font.Size = 9;
            itemRow.Cells[2].Format.Font.Bold = true;
            itemRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[3].AddParagraph("Blocks Cut");
            itemRow.Cells[3].Format.Font.Size = 9;
            itemRow.Cells[3].Format.Font.Bold = true;
            itemRow.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[4].AddParagraph("OffSpec");
            itemRow.Cells[4].Format.Font.Size = 9;
            itemRow.Cells[4].Format.Font.Bold = true;
            itemRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[5].AddParagraph("Lifted Off Board");
            itemRow.Cells[5].Format.Font.Size = 9;
            itemRow.Cells[5].Format.Font.Bold = true;
            itemRow.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[6].AddParagraph("Uneven Thickness");
            itemRow.Cells[6].Format.Font.Size = 9;
            itemRow.Cells[6].Format.Font.Bold = true;
            itemRow.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[7].AddParagraph("Stone Lines");
            itemRow.Cells[7].Format.Font.Size = 9;
            itemRow.Cells[7].Format.Font.Bold = true;
            itemRow.Cells[7].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[8].AddParagraph("Too Thick");
            itemRow.Cells[8].Format.Font.Size = 9;
            itemRow.Cells[8].Format.Font.Bold = true;
            itemRow.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[9].AddParagraph("Too Thin");
            itemRow.Cells[9].Format.Font.Size = 9;
            itemRow.Cells[9].Format.Font.Bold = true;
            itemRow.Cells[9].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[9].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[9].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[10].AddParagraph("Damaged");
            itemRow.Cells[10].Format.Font.Size = 9;
            itemRow.Cells[10].Format.Font.Bold = true;
            itemRow.Cells[10].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[10].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[10].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[11].AddParagraph("Contaminated");
            itemRow.Cells[11].Format.Font.Size = 9;
            itemRow.Cells[11].Format.Font.Bold = true;
            itemRow.Cells[11].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[11].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[11].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[12].AddParagraph("Operator Error");
            itemRow.Cells[12].Format.Font.Size = 9;
            itemRow.Cells[12].Format.Font.Bold = true;
            itemRow.Cells[12].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[12].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[12].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[13].AddParagraph("Other Reason");
            itemRow.Cells[13].Format.Font.Size = 9;
            itemRow.Cells[13].Format.Font.Bold = true;
            itemRow.Cells[13].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[13].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[13].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[14].AddParagraph("Completed Date");
            itemRow.Cells[14].Format.Font.Size = 9;
            itemRow.Cells[14].Format.Font.Bold = true;
            itemRow.Cells[14].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[14].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[14].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;


            decimal totBlocksCut = 0,totOffSpec=0, totLiftedOffBoard=0,totUnevenThickness=0,totStoneLines=0,totTooThick=0,totTooThin=0,totDamaged=0,totContaminated=0,totOperatorError=0;

            foreach (var item in offSpecDetails)
            {
                Row row2 = table.AddRow();
                row2.HeadingFormat = true;
                row2.Format.Alignment = ParagraphAlignment.Center;
                row2.Format.Font.Bold = false;
                row2.BottomPadding = 1;

                row2.Cells[0].AddParagraph(item.OrderNo.ToString());
                row2.Cells[0].Format.Font.Size = 9;
                row2.Cells[0].Format.Font.Bold = true;
                row2.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                row2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[1].AddParagraph(item.Product.ProductCode);
                row2.Cells[1].Format.Font.Size = 9;
                row2.Cells[1].Format.Font.Bold = true;
                row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[2].AddParagraph(item.Product.ProductDescription);
                row2.Cells[2].Format.Font.Size = 9;
                row2.Cells[2].Format.Font.Bold = true;
                row2.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[3].AddParagraph(item.Blocks.ToString());
                row2.Cells[3].Format.Font.Size = 9;
                row2.Cells[3].Format.Font.Bold = true;
                row2.Cells[3].Format.Alignment = ParagraphAlignment.Right;
                row2.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[4].AddParagraph(item.OffSpecTiles.ToString());
                row2.Cells[4].Format.Font.Size = 9;
                row2.Cells[4].Format.Font.Bold = true;
                row2.Cells[4].Format.Alignment = ParagraphAlignment.Right;
                row2.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
               
                row2.Cells[5].Format.Font.Size = 9;
                row2.Cells[5].Format.Font.Bold = true;
                row2.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[5].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.LiftedOffBoard == true)
                {
                    row2.Cells[5].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[5].AddParagraph("");
                }

                row2.Cells[6].Format.Font.Size = 9;
                row2.Cells[6].Format.Font.Bold = true;
                row2.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[6].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.UnevenThickness == true)
                {
                    row2.Cells[6].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[6].AddParagraph("");
                }

                row2.Cells[7].Format.Font.Size = 9;
                row2.Cells[7].Format.Font.Bold = true;
                row2.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[7].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.StoneLines == true)
                {
                    row2.Cells[7].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[7].AddParagraph("");
                }

                row2.Cells[8].Format.Font.Size = 9;
                row2.Cells[8].Format.Font.Bold = true;
                row2.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[8].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.TooThick == true)
                {
                    row2.Cells[8].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[8].AddParagraph("");
                }

                row2.Cells[9].Format.Font.Size = 9;
                row2.Cells[9].Format.Font.Bold = true;
                row2.Cells[9].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[9].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[9].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[9].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.TooThin == true)
                {
                    row2.Cells[9].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[9].AddParagraph("");
                }

                row2.Cells[10].Format.Font.Size = 9;
                row2.Cells[10].Format.Font.Bold = true;
                row2.Cells[10].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[10].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[10].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[10].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.DamagedBlockLog == true)
                {
                    row2.Cells[10].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[10].AddParagraph("");
                }

                row2.Cells[11].Format.Font.Size = 9;
                row2.Cells[11].Format.Font.Bold = true;
                row2.Cells[11].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[11].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[11].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[11].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.Contaminated == true)
                {
                    row2.Cells[11].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[11].AddParagraph("");
                }


                row2.Cells[12].Format.Font.Size = 9;
                row2.Cells[12].Format.Font.Bold = true;
                row2.Cells[12].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[12].VerticalAlignment = VerticalAlignment.Center;
                row2.Cells[12].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[12].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                if (item.OperatorError == true)
                {
                    row2.Cells[12].AddParagraph().AddImage("I:/PRODUCTION/QImg/right.png");
                }
                else
                {
                    row2.Cells[12].AddParagraph("");
                }


                row2.Cells[13].AddParagraph(item.OtherComment);
                row2.Cells[13].Format.Font.Size = 9;
                row2.Cells[13].Format.Font.Bold = true;
                row2.Cells[13].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[13].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[13].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[14].AddParagraph(item.CompletedDate.ToString("dd/MM/yyyy"));
                row2.Cells[14].Format.Font.Size = 9;
                row2.Cells[14].Format.Font.Bold = true;
                row2.Cells[14].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[14].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[14].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                totBlocksCut += item.Blocks;
                totOffSpec += item.OffSpecTiles;
                totLiftedOffBoard += item.LiftedOffBoard == true ? 1 : 0;
                totUnevenThickness += item.UnevenThickness == true ? 1 : 0;
                totStoneLines += item.StoneLines == true ? 1 : 0;
                totTooThick += item.TooThick == true ? 1 : 0;
                totTooThin += item.TooThin == true ? 1 : 0;
                totDamaged += item.DamagedBlockLog == true ? 1 : 0;
                totContaminated += item.Contaminated == true ? 1 : 0;
                totOperatorError += item.OperatorError == true ? 1 : 0;
            }

            Row rowLast = table.AddRow();
            rowLast.HeadingFormat = true;
            rowLast.Format.Alignment = ParagraphAlignment.Center;
            rowLast.Format.Font.Bold = false;
            rowLast.BottomPadding = 1;

            rowLast.Cells[0].AddParagraph("TOTAL");
            rowLast.Cells[0].Format.Font.Size = 12;
            rowLast.Cells[0].Format.Font.Bold = true;
            rowLast.Cells[0].MergeRight = 2;
            rowLast.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            rowLast.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[3].AddParagraph(totBlocksCut.ToString());
            rowLast.Cells[3].Format.Font.Size = 9;
            rowLast.Cells[3].Format.Font.Bold = true;
            rowLast.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[4].AddParagraph(totOffSpec.ToString());
            rowLast.Cells[4].Format.Font.Size = 9;
            rowLast.Cells[4].Format.Font.Bold = true;
            rowLast.Cells[4].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[5].AddParagraph(totLiftedOffBoard.ToString());
            rowLast.Cells[5].Format.Font.Size = 9;
            rowLast.Cells[5].Format.Font.Bold = true;
            rowLast.Cells[5].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[6].AddParagraph(totUnevenThickness.ToString());
            rowLast.Cells[6].Format.Font.Size = 9;
            rowLast.Cells[6].Format.Font.Bold = true;
            rowLast.Cells[6].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[7].AddParagraph(totStoneLines.ToString());
            rowLast.Cells[7].Format.Font.Size = 9;
            rowLast.Cells[7].Format.Font.Bold = true;
            rowLast.Cells[7].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[8].AddParagraph(totTooThick.ToString());
            rowLast.Cells[8].Format.Font.Size = 9;
            rowLast.Cells[8].Format.Font.Bold = true;
            rowLast.Cells[8].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[9].AddParagraph(totTooThin.ToString());
            rowLast.Cells[9].Format.Font.Size = 9;
            rowLast.Cells[9].Format.Font.Bold = true;
            rowLast.Cells[9].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[9].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[9].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[10].AddParagraph(totDamaged.ToString());
            rowLast.Cells[10].Format.Font.Size = 9;
            rowLast.Cells[10].Format.Font.Bold = true;
            rowLast.Cells[10].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[10].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[10].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[11].AddParagraph(totContaminated.ToString());
            rowLast.Cells[11].Format.Font.Size = 9;
            rowLast.Cells[11].Format.Font.Bold = true;
            rowLast.Cells[11].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[11].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[11].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowLast.Cells[12].AddParagraph(totOperatorError.ToString());
            rowLast.Cells[12].Format.Font.Size = 9;
            rowLast.Cells[12].Format.Font.Bold = true;
            rowLast.Cells[12].Format.Alignment = ParagraphAlignment.Right;
            rowLast.Cells[12].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowLast.Cells[12].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

        }   
    }
}
