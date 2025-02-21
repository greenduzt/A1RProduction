using A1QSystem.Core.Enumerations;
using A1QSystem.Model;
using A1QSystem.Model.Vehicles;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace A1QSystem
{
    public class VehicleWorkOrderPDF
    {
        Document document;
        TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;
        private string odometer;
        private string frequency;
        private string unit;
        public string Date;
        public string Comments;
        

        private DateTime currentDate;
        private VehicleWorkOrder vehicleWorkOrder;
        private ObservableCollection<VehicleMaintenanceInfo> vehicleWorkOrderDescription;
        private ObservableCollection<VehicleMaintenanceInfo> safetyInspectionInfo;

        public VehicleWorkOrderPDF(VehicleWorkOrder vwo, ObservableCollection<VehicleMaintenanceInfo> vwod, ObservableCollection<VehicleMaintenanceInfo> sii,string f, string u,long o)
        {
            vehicleWorkOrder = vwo;
            vehicleWorkOrderDescription = vwod;
            safetyInspectionInfo = sii;
            Date = Convert.ToString(vwo.NextServiceDate);
            currentDate = DateTime.Now;
            frequency = f;
            unit = u;
            odometer = o.ToString();
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

            string filename = "VWO" + vehicleWorkOrder.VehicleWorkOrderID + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";
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
                //int result = DBAccess.InsertErrorLog(ex.Message);
                //Msg.Show("The file is currently opened. Please close the file and try again.", "File is in use", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.Delete(Path.Combine(desktopPath, filename));

            return res;
        }

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "Vehicle Work Order";
            this.document.Info.Subject = "Vehicle Work Order";
            this.document.Info.Author = "Chamara Walaliyadde";

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

            paragraph = this.addressFrame.AddParagraph("Vehicle Work Order");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            paragraph = this.addressFrame.AddParagraph("Work Order Type - " + vehicleWorkOrder.WorkOrderType);
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            //paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;


            paragraph = this.addressFrame.AddParagraph("Maintenance Frequency - " + (String.IsNullOrWhiteSpace(frequency) || frequency == "0" ? "Monthly" : frequency == "8" ? "Weekly" : frequency + unit));
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            //paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;         

            paragraph = this.addressFrame.AddParagraph("Work Order No - " + vehicleWorkOrder.VehicleWorkOrderID);
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = this.addressFrame.AddParagraph("Assigned Date - " + Convert.ToDateTime(vehicleWorkOrder.NextServiceDate).ToString("dd/MM/yyyy"));
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


            // Create the Top table
            //section.PageSetup.TopMargin = "3cm";
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Format.Font.Name = "Helvatica";
            this.table.Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;
            this.table.Borders.Color = MigraDoc.DocumentObjectModel.Colors.White;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 1.0;
            this.table.BottomPadding = 1.0;        
           

            Column column2 = this.table.AddColumn("4.35cm");
            column2.Format.Alignment = ParagraphAlignment.Center;

            column2 = this.table.AddColumn("2.35cm");
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("3.92cm");
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("7.92cm");
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row row3 = table.AddRow();
            row3.HeadingFormat = true;
            row3.Format.Alignment = ParagraphAlignment.Center;
            row3.Format.Font.Bold = true;

            row3.Cells[0].AddParagraph("Vehicle Serial Number");
            row3.Cells[0].Format.Font.Size = 9;
            row3.Cells[0].Format.Font.Bold = true;
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row3.Cells[1].AddParagraph("Vehicle Code");
            row3.Cells[1].Format.Font.Size = 9;
            row3.Cells[1].Format.Font.Bold = true;
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row3.Cells[2].AddParagraph("Vehicle Brand");
            row3.Cells[2].Format.Font.Size = 9;
            row3.Cells[2].Format.Font.Bold = true;
            row3.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row3.Cells[3].AddParagraph("Vehicle Description");
            row3.Cells[3].Format.Font.Size = 9;
            row3.Cells[3].Format.Font.Bold = true;
            row3.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row row4 = table.AddRow();
            row4.HeadingFormat = true;
            row4.Format.Alignment = ParagraphAlignment.Center;
            row4.Format.Font.Bold = true;

            row4.Cells[0].AddParagraph(vehicleWorkOrder.Vehicle.SerialNumber);
            row4.Cells[0].Format.Font.Size = 11;
            row4.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[0].Format.Font.Bold = true;
            row4.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[1].AddParagraph(vehicleWorkOrder.Vehicle.VehicleCode);
            row4.Cells[1].Format.Font.Size = 8;
            row4.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[1].Format.Font.Bold = false;
            row4.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[2].AddParagraph(vehicleWorkOrder.Vehicle.VehicleBrand);
            row4.Cells[2].Format.Font.Size = 8;
            row4.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[2].Format.Font.Bold = false;
            row4.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[3].AddParagraph(vehicleWorkOrder.Vehicle.VehicleDescription);
            row4.Cells[3].Format.Font.Size = 8;
            row4.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[3].Format.Font.Bold = false;
            row4.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();       
                        
            // Create the Second table
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Format.Font.Name = "Helvatica";
            this.table.Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;
            this.table.Borders.Color = MigraDoc.DocumentObjectModel.Colors.White;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 1.0;
            this.table.BottomPadding = 1.0;        

            Column mCol = this.table.AddColumn("6.70cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;

            mCol = this.table.AddColumn("3.92cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;
            mCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mCol = this.table.AddColumn("3.92cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;
            mCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row mRow = table.AddRow();
            mRow.HeadingFormat = true;
            mRow.Format.Alignment = ParagraphAlignment.Center;
            row3.Format.Font.Bold = true;

            mRow.Cells[0].AddParagraph("Mechanic Name");
            mRow.Cells[0].Format.Font.Size = 9;
            mRow.Cells[0].Format.Font.Bold = true;
            mRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            mRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            mRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mRow.Cells[1].AddParagraph("Last Odometre");
            mRow.Cells[1].Format.Font.Size = 9;
            mRow.Cells[1].Format.Font.Bold = true;
            mRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            mRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            mRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mRow.Cells[2].AddParagraph("Current Odometre");
            mRow.Cells[2].Format.Font.Size = 9;
            mRow.Cells[2].Format.Font.Bold = true;
            mRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            mRow.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            mRow.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            
            Row mRow2 = table.AddRow();
            mRow2.HeadingFormat = true;
            mRow2.Format.Alignment = ParagraphAlignment.Center;
            mRow2.Format.Font.Bold = true;

            //mRow2.Cells[0].AddParagraph(vehicleWorkOrder.User.FullName);
            mRow2.Cells[0].AddParagraph("");
            mRow2.Cells[0].Format.Font.Size = 8;
            mRow2.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            mRow2.Cells[0].Format.Font.Bold = false;
            mRow2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            mRow2.Cells[1].AddParagraph(vehicleWorkOrder.LastOdometerReading.ToString() == "0" ? "" : vehicleWorkOrder.LastOdometerReading + unit);
            mRow2.Cells[1].Format.Font.Size = 8;
            mRow2.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            mRow2.Cells[1].Format.Font.Bold = false;
            mRow2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            mRow2.Cells[2].AddParagraph(odometer == "0" ? "" : odometer+unit);
            mRow2.Cells[2].Format.Font.Size = 8;
            mRow2.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            mRow2.Cells[2].Format.Font.Bold = false;
            mRow2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

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

            Column itemsCol = this.table.AddColumn("1.50cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;

            itemsCol = this.table.AddColumn("1.50cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.50cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            
            itemsCol = this.table.AddColumn("12.2cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            itemsCol = this.table.AddColumn("1.8cm");
            itemsCol.Format.Alignment = ParagraphAlignment.Center;
            itemsCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;                

            Row itemRow = table.AddRow();
            itemRow.HeadingFormat = true;
            itemRow.Format.Alignment = ParagraphAlignment.Center;
            itemRow.Format.Font.Bold = true;
            
            itemRow.Cells[0].AddParagraph("Item Code");
            itemRow.Cells[0].Format.Font.Size = 9;
            itemRow.Cells[0].Format.Font.Bold = true;
            itemRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            itemRow.Cells[1].AddParagraph("Item Description");
            itemRow.Cells[1].Format.Font.Size = 9;
            itemRow.Cells[1].MergeRight = 2;
            itemRow.Cells[1].Format.Font.Bold = true;
            itemRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;   

            itemRow.Cells[4].AddParagraph("Completed (Tick)");
            itemRow.Cells[4].Format.Font.Size = 9;
            itemRow.Cells[4].Format.Font.Bold = true;
            itemRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
            
            foreach (var item in vehicleWorkOrderDescription)
            {
                Row row2 = table.AddRow();
                row2.HeadingFormat = true;
                row2.Format.Alignment = ParagraphAlignment.Center;
                row2.Format.Font.Bold = false;
                row2.BottomPadding = 1;
                
                row2.Cells[0].AddParagraph(item.Code);
                row2.Cells[0].Format.Font.Size = 9;
                row2.Cells[0].Format.Font.Bold = true;
                row2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                row2.Cells[1].AddParagraph(item.Description);
                row2.Cells[1].Format.Font.Size = 9;
                row2.Cells[1].MergeRight = 2;
                row2.Cells[1].Format.Font.Bold = true;
                row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                row2.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;   
                
            }


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
            this.table.Borders.Width = 0;
            this.table.Borders.Left.Width = 0;
            this.table.Borders.Right.Width = 0;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 1.0;
            this.table.BottomPadding = 1.0;

            Column finalCol = this.table.AddColumn("12.2cm");
            finalCol.Format.Alignment = ParagraphAlignment.Left;

            finalCol = this.table.AddColumn("6.2cm");
            finalCol.Format.Alignment = ParagraphAlignment.Right;
            finalCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row finalRow = table.AddRow();
            finalRow.HeadingFormat = true;
            finalRow.Format.Alignment = ParagraphAlignment.Center;
            finalRow.Format.Font.Bold = true;

            finalRow.Cells[0].AddParagraph("Sign........................................................");
            finalRow.Cells[0].Format.Font.Size = 9;
            finalRow.Cells[0].Format.Font.Bold = true;
            finalRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            finalRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            finalRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            finalRow.Cells[1].AddParagraph("Date..........................................");
            finalRow.Cells[1].Format.Font.Size = 9;
            finalRow.Cells[1].Format.Font.Bold = true;
            finalRow.Cells[1].Format.Alignment = ParagraphAlignment.Right;
            finalRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            finalRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;


        }        
    }
}
