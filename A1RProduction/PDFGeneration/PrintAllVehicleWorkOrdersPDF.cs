using A1QSystem.Model.Vehicles;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
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
    public class PrintAllVehicleWorkOrdersPDF
    {
        Document document;
        MigraDoc.DocumentObjectModel.Shapes.TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;
        private ObservableCollection<VehicleWorkOrder> vehicleWorkOrders;
        private DateTime currentDate;

        public PrintAllVehicleWorkOrdersPDF(ObservableCollection<VehicleWorkOrder> vwo)
        {
            vehicleWorkOrders = new ObservableCollection<VehicleWorkOrder>();
            vehicleWorkOrders = vwo;
            currentDate = DateTime.Now;
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
            document.DefaultPageSetup.Orientation = Orientation.Landscape;

            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string filename = "AVWO" + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";
            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/VehicleWorkOrders/" + filename);

                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/VehicleWorkOrders/" + filename);
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
            this.document.Info.Title = "Vehicle Work Orders";
            this.document.Info.Subject = "Vehicle Work Orders";
            this.document.Info.Author = "Chamara Walaliyadde";

            DefineStyles();
            CreatePage();

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

            paragraph = this.addressFrame.AddParagraph("Vehicle Work Orders");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;
                        
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-1.1cm";
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
            textFrame1.Width = "18.2cm";
            Paragraph footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("www.a1rubber.com");
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Right;

            footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("©Copyright A1Rubber " + currentDate.Year.ToString());
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Right;

            footerPara1 = textFrame1.AddParagraph();
            footerPara1.AddText("Page");
            footerPara1.AddPageField();
            footerPara1.AddText(" of ");
            footerPara1.AddNumPagesField();
            footerPara1.Format.Font.Size = 7;
            footerPara1.Format.Alignment = ParagraphAlignment.Center;
                                    
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
            this.table.Format.Font.Size = 9;

            Column column2 = this.table.AddColumn("1.7cm");//WO No
            column2.Format.Alignment = ParagraphAlignment.Center;

            column2 = this.table.AddColumn("2.2cm");//WO Type
            column2.Format.Alignment = ParagraphAlignment.Center;

            column2 = this.table.AddColumn("1.5cm");//Urgency
            column2.Format.Alignment = ParagraphAlignment.Center;

            column2 = this.table.AddColumn("2cm");//Code
            column2.Format.Alignment = ParagraphAlignment.Center;

            column2 = this.table.AddColumn("3.5cm");//Serial Number
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("2.35cm");//Vehicle Name
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("7.5cm");//Description
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("2cm");//Service Date
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("3cm");//Days left
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row row1 = table.AddRow();
            row1.HeadingFormat = true;
            row1.Format.Alignment = ParagraphAlignment.Center;
            row1.Format.Font.Bold = true;

            row1.Cells[0].AddParagraph("WO No");
            row1.Cells[0].Format.Font.Size = 9;
            row1.Cells[0].Format.Font.Bold = true;
            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[1].AddParagraph("WO Type");
            row1.Cells[1].Format.Font.Size = 9;
            row1.Cells[1].Format.Font.Bold = true;
            row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[2].AddParagraph("Urgency");
            row1.Cells[2].Format.Font.Size = 9;
            row1.Cells[2].Format.Font.Bold = true;
            row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[3].AddParagraph("Code");
            row1.Cells[3].Format.Font.Size = 9;
            row1.Cells[3].Format.Font.Bold = true;
            row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[4].AddParagraph("Serial Number");
            row1.Cells[4].Format.Font.Size = 9;
            row1.Cells[4].Format.Font.Bold = true;
            row1.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[5].AddParagraph("Vehicle Name");
            row1.Cells[5].Format.Font.Size = 9;
            row1.Cells[5].Format.Font.Bold = true;
            row1.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[6].AddParagraph("Description");
            row1.Cells[6].Format.Font.Size = 9;
            row1.Cells[6].Format.Font.Bold = true;
            row1.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[7].AddParagraph("Service Date");
            row1.Cells[7].Format.Font.Size = 9;
            row1.Cells[7].Format.Font.Bold = true;
            row1.Cells[7].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row1.Cells[8].AddParagraph("Days Left");
            row1.Cells[8].Format.Font.Size = 9;
            row1.Cells[8].Format.Font.Bold = true;
            row1.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;              

            foreach (var item in vehicleWorkOrders)
            {
                Row row2 = table.AddRow();
                row2.HeadingFormat = true;
                row2.Format.Alignment = ParagraphAlignment.Center;
                row2.Format.Font.Bold = false;
                row2.BottomPadding = 1;

                row2.Cells[0].AddParagraph(item.VehicleWorkOrderID.ToString());
                row2.Cells[0].Format.Font.Size = 9;
                row2.Cells[0].Format.Font.Bold = true;
                row2.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                row2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[1].AddParagraph(item.WorkOrderType);
                row2.Cells[1].Format.Font.Size = 9;
                row2.Cells[1].Format.Font.Bold = true;
                row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[2].AddParagraph(item.Urgency == 1 ? "Urgent" : "Normal");
                row2.Cells[2].Format.Font.Size = 9;
                row2.Cells[2].Format.Font.Bold = true;
                row2.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[3].AddParagraph(item.Vehicle.VehicleCode);
                row2.Cells[3].Format.Font.Size = 9;
                row2.Cells[3].Format.Font.Bold = true;
                row2.Cells[3].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[4].AddParagraph(item.Vehicle.SerialNumber);
                row2.Cells[4].Format.Font.Size = 9;
                row2.Cells[4].Format.Font.Bold = true;
                row2.Cells[4].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[5].AddParagraph(item.Vehicle.VehicleBrand);
                row2.Cells[5].Format.Font.Size = 9;
                row2.Cells[5].Format.Font.Bold = true;
                row2.Cells[5].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[6].AddParagraph(item.Vehicle.VehicleDescription);
                row2.Cells[6].Format.Font.Size = 9;
                row2.Cells[6].Format.Font.Bold = true;
                row2.Cells[6].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[6].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[6].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[7].AddParagraph(Convert.ToDateTime(item.NextServiceDate).ToString("dd/MM/yyyy"));
                row2.Cells[7].Format.Font.Size = 9;
                row2.Cells[7].Format.Font.Bold = true;
                row2.Cells[7].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[7].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[7].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[8].AddParagraph(item.DaysToComplete);
                row2.Cells[8].Format.Font.Size = 9;
                row2.Cells[8].Format.Font.Bold = true;
                row2.Cells[8].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[8].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[8].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
            }
        } 
    }
}
