using A1QSystem.Core.Enumerations;
using A1QSystem.Model.Vehicles;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MsgBox;
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
    public class VehicleWorkOrderHistoryPDF
    {
        Document document;
        TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;
        public int WorkOrderId;
        public string Date;
        public List<Vehicle> CheklstItems;
        public string Comments;
        public string AllotedTime;
        public int SafetyItem;
        public int BreakDown;
        public int RoutineMain;
        public int Inspection;

        private DateTime currentDate;
        private VehicleWorkOrderHistory vehicleWorkOrderHistory;

        public VehicleWorkOrderHistoryPDF(VehicleWorkOrderHistory v)
        {
            vehicleWorkOrderHistory = v;
            Date = Convert.ToString(Convert.ToDateTime(vehicleWorkOrderHistory.CompletedDate).ToString("dd/MM/yyyy"));
            currentDate = DateTime.Now;
        }

        public Exception createWorkOrderPDF()
        {
            Exception res = null;
            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.FooterDistance = "-3.3cm";
            document.DefaultPageSetup.LeftMargin = "1.2cm";
            document.DefaultPageSetup.RightMargin = "1.2cm";
            document.DefaultPageSetup.TopMargin = "3.2cm";

            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string filename = "VWOH" + vehicleWorkOrderHistory.VehicleWorkOrderID + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";
            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/VehicleWorkOrderHistory/" + filename);
                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/VehicleWorkOrderHistory/" + filename);
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
            this.document.Info.Title = "Vehicle Work Order History";
            this.document.Info.Subject = "Vehicle Work Order History";
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
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "7cm";
            this.addressFrame.Left = ShapePosition.Right;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.RelativeVertical = RelativeVertical.Page;
            //this.addressFrame.MarginTop = "-1cm";

            paragraph = this.addressFrame.AddParagraph("Vehicle Work Order History");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            paragraph = this.addressFrame.AddParagraph("Work Order Type - " + vehicleWorkOrderHistory.WorkOrderType);
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            //paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = this.addressFrame.AddParagraph("Maintenance Frequency - " + (String.IsNullOrWhiteSpace(vehicleWorkOrderHistory.MaintenanceFrequency) ? "One Off" : vehicleWorkOrderHistory.MaintenanceFrequency));
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            //paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = this.addressFrame.AddParagraph("Work Order No - " + vehicleWorkOrderHistory.VehicleWorkOrderID);
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = this.addressFrame.AddParagraph("Completed Date - " + Convert.ToDateTime(vehicleWorkOrderHistory.CompletedDate).ToString("dd/MM/yyyy"));
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.3cm";
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
            footerPara1.AddText("©Copyright A1Rubber" + DateTime.Now.Year);
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

            row4.Cells[0].AddParagraph(vehicleWorkOrderHistory.Vehicle.SerialNumber);
            row4.Cells[0].Format.Font.Size = 11;
            row4.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[0].Format.Font.Bold = true;
            row4.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[1].AddParagraph(vehicleWorkOrderHistory.Vehicle.VehicleCode);
            row4.Cells[1].Format.Font.Size = 8;
            row4.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[1].Format.Font.Bold = false;
            row4.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[2].AddParagraph(vehicleWorkOrderHistory.Vehicle.VehicleBrand);
            row4.Cells[2].Format.Font.Size = 8;
            row4.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[2].Format.Font.Bold = false;
            row4.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[3].AddParagraph(vehicleWorkOrderHistory.Vehicle.VehicleDescription);
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

            Row mRow2 = table.AddRow();
            mRow2.HeadingFormat = true;
            mRow2.Format.Alignment = ParagraphAlignment.Center;
            mRow2.Format.Font.Bold = true;

            mRow2.Cells[0].AddParagraph(vehicleWorkOrderHistory.User.FullName);
            mRow2.Cells[0].Format.Font.Size = 8;
            mRow2.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            mRow2.Cells[0].Format.Font.Bold = false;
            mRow2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            mRow2.Cells[1].AddParagraph(vehicleWorkOrderHistory.OdometerReading == 0 ? "" : vehicleWorkOrderHistory.OdometerReading.ToString());
            mRow2.Cells[1].Format.Font.Size = 8;
            mRow2.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            mRow2.Cells[1].Format.Font.Bold = false;
            mRow2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

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

            if (vehicleWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
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
            }
            else
            {
                itemRow.Cells[0].AddParagraph("Item Description");
                itemRow.Cells[0].Format.Font.Size = 9;
                itemRow.Cells[0].Format.Font.Bold = true;
                itemRow.Cells[0].MergeRight = 3;
                itemRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                itemRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                itemRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;               
            }

            itemRow.Cells[4].AddParagraph("Completed");
            itemRow.Cells[4].Format.Font.Size = 9;
            itemRow.Cells[4].Format.Font.Bold = true;
            itemRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            itemRow.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            itemRow.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            foreach (var item in vehicleWorkOrderHistory.VehicleWorkDescription)
            {
                Row row2 = table.AddRow();
                row2.HeadingFormat = true;
                row2.Format.Alignment = ParagraphAlignment.Center;
                row2.Format.Font.Bold = false;
                row2.BottomPadding = 1;

                if (vehicleWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                {
                    row2.Cells[0].AddParagraph(item.VehicleMaintenanceInfo.Code);
                    row2.Cells[0].Format.Font.Size = 9;
                    row2.Cells[0].Format.Font.Bold = true;
                    row2.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                    row2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    row2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    row2.Cells[1].AddParagraph(item.Description);
                    row2.Cells[1].Format.Font.Size = 9;
                    row2.Cells[1].MergeRight = 2;
                    row2.Cells[1].Format.Font.Bold = true;
                    row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                    row2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    row2.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    if (vehicleWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                    {
                        if (item.PartsOrdered == true)
                        {
                            row2.Cells[2].AddParagraph(item.IsCompleted == true ? "Yes" : "No");
                            row2.Cells[2].Format.Font.Size = 9;
                            row2.Cells[2].Format.Font.Bold = true;
                            row2.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                            row2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            row2.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                        }
                        else
                        {
                            row2.Cells[2].AddParagraph(item.IsCompleted == true ? "Yes" : "No");
                            row2.Cells[2].Format.Font.Size = 9;
                            row2.Cells[2].Format.Font.Bold = true;
                            row2.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                            row2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            row2.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                        }
                        row2.Cells[3].AddParagraph(item.IsCompleted == true ? "Yes" : "No");
                        row2.Cells[3].Format.Font.Size = 9;
                        row2.Cells[3].Format.Font.Bold = true;
                        row2.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                        row2.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        row2.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    }
                    else
                    {
                        row2.Cells[4].AddParagraph(item.IsCompleted == true ? "Yes" : "No");
                        row2.Cells[4].Format.Font.Size = 9;
                        row2.Cells[4].Format.Font.Bold = true;
                        row2.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                        row2.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        row2.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    }

                    if (vehicleWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                    {
                        foreach (var items in item.VehicleRepairDescription)
                        {
                            Row vrdRow = table.AddRow();
                            vrdRow.HeadingFormat = true;
                            vrdRow.Format.Alignment = ParagraphAlignment.Center;
                            vrdRow.Format.Font.Bold = false;
                            vrdRow.BottomPadding = 1;

                            vrdRow.Cells[0].AddParagraph();
                            vrdRow.Cells[0].Format.Font.Bold = true;
                            vrdRow.Cells[0].Format.Borders.Width = 0;
                            vrdRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            vrdRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            vrdRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                            vrdRow.Cells[1].AddParagraph(items.StrSequenceNumber + "  |  " + items.RepairDescription);
                            vrdRow.Cells[1].MergeRight = 2;
                            vrdRow.Cells[1].Borders.Bottom.Visible = true;
                            vrdRow.Cells[1].Format.Font.Size = 9;
                            vrdRow.Cells[1].Format.Font.Bold = true;
                            vrdRow.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                            vrdRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            vrdRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                            vrdRow.Cells[4].AddParagraph(items.IsCompleted == true ? "Yes" : "No");
                            vrdRow.Cells[4].Format.Font.Size = 9;
                            vrdRow.Cells[4].Format.Font.Bold = true;
                            vrdRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                            vrdRow.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            vrdRow.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                            foreach (var itemz in items.Vehicleparts)
                            {
                                Row vpRow = table.AddRow();
                                vpRow.HeadingFormat = true;
                                vpRow.Format.Alignment = ParagraphAlignment.Center;
                                vpRow.Format.Font.Bold = false;
                                vpRow.BottomPadding = 1;

                                vpRow.Cells[0].AddParagraph();
                                vpRow.Cells[0].MergeRight = 1;
                                vpRow.Cells[0].Format.Borders.Width = 0;
                                vpRow.Cells[0].Format.Font.Size = 9;
                                vpRow.Cells[0].Format.Font.Bold = true;
                                vpRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                                vpRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                vpRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                vpRow.Cells[2].AddParagraph(itemz.StrSequenceNumber + "  |  " + itemz.PartCode);
                                vpRow.Cells[2].MergeRight = 2;
                                vpRow.Cells[2].Borders.Bottom.Visible = true;
                                vpRow.Cells[2].Format.Font.Size = 9;
                                vpRow.Cells[2].Format.Font.Bold = true;
                                vpRow.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                                vpRow.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                vpRow.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                            }
                        }    
                    }
                    else
                    {
                        Row row5 = table.AddRow();
                        row5.HeadingFormat = true;
                        row5.Format.Alignment = ParagraphAlignment.Center;
                        row5.Format.Font.Bold = false;
                        row5.BottomPadding = 1;

                        row5.Cells[0].AddParagraph("");
                        row5.Cells[0].MergeRight = 4;
                        row5.Cells[0].Borders.Bottom.Visible = false;
                        row5.Cells[0].Format.Font.Size = 9;
                        row5.Cells[0].Format.Font.Bold = true;
                        row5.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                        row5.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        row5.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                        Row row6 = table.AddRow();
                        row6.HeadingFormat = true;
                        row6.Format.Alignment = ParagraphAlignment.Center;
                        row6.Format.Font.Bold = false;
                        row6.BottomPadding = 1;

                        row6.Cells[0].AddParagraph("");
                        row6.Cells[0].MergeRight = 4;
                        row6.Cells[0].Borders.Top.Visible = false;
                        row6.Cells[0].Format.Font.Size = 9;
                        row6.Cells[0].Format.Font.Bold = true;
                        row6.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                        row6.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        row6.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    }
                }
                else if (vehicleWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                {

                    row2.Cells[0].AddParagraph(item.Description == null ? "Repair order for " + vehicleWorkOrderHistory.Vehicle.SerialNumber : item.Description);
                    row2.Cells[0].Format.Font.Size = 9;
                    row2.Cells[0].MergeRight = 4;
                    row2.Cells[0].Format.Font.Bold = true;
                    row2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    row2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    foreach (var items in item.VehicleRepairDescription)
                    {
                        Row vrdRow = table.AddRow();
                        vrdRow.HeadingFormat = true;
                        vrdRow.Format.Alignment = ParagraphAlignment.Center;
                        vrdRow.Format.Font.Bold = false;
                        vrdRow.BottomPadding = 1;

                        vrdRow.Cells[0].AddParagraph();
                        vrdRow.Cells[0].Format.Font.Bold = true;
                        vrdRow.Cells[0].Format.Borders.Width = 0;
                        vrdRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                        vrdRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        vrdRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                        vrdRow.Cells[1].AddParagraph(items.StrSequenceNumber + "  |  " + items.RepairDescription);
                        vrdRow.Cells[1].MergeRight = 2;
                        vrdRow.Cells[1].Borders.Bottom.Visible = true;
                        vrdRow.Cells[1].Format.Font.Size = 9;
                        vrdRow.Cells[1].Format.Font.Bold = true;
                        vrdRow.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                        vrdRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        vrdRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                        vrdRow.Cells[4].AddParagraph(items.IsCompleted == true ? "Yes" : "No");
                        vrdRow.Cells[4].Format.Font.Size = 9;
                        vrdRow.Cells[4].Format.Font.Bold = true;
                        vrdRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                        vrdRow.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        vrdRow.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                        foreach (var itemz in items.Vehicleparts)
                        {
                            Row vpRow = table.AddRow();
                            vpRow.HeadingFormat = true;
                            vpRow.Format.Alignment = ParagraphAlignment.Center;
                            vpRow.Format.Font.Bold = false;
                            vpRow.BottomPadding = 1;

                            vpRow.Cells[0].AddParagraph();
                            vpRow.Cells[0].MergeRight = 1;
                            vpRow.Cells[0].Format.Borders.Width = 0;
                            vpRow.Cells[0].Format.Font.Size = 9;
                            vpRow.Cells[0].Format.Font.Bold = true;
                            vpRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            vpRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            vpRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                            vpRow.Cells[2].AddParagraph(itemz.StrSequenceNumber + "  |  " + itemz.PartCode);
                            vpRow.Cells[2].MergeRight = 2;
                            vpRow.Cells[2].Borders.Bottom.Visible = true;
                            vpRow.Cells[2].Format.Font.Size = 9;
                            vpRow.Cells[2].Format.Font.Bold = true;
                            vpRow.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                            vpRow.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            vpRow.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                        }
                    }                    
                }
            }            

            if (!string.IsNullOrWhiteSpace(vehicleWorkOrderHistory.ExtraNotes))
            {
                paragraph = section.AddParagraph();
                paragraph.Format.SpaceBefore = "-0.10cm";
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

                Column ExtraNoteCol = this.table.AddColumn("18.50cm");
                ExtraNoteCol.Format.Alignment = ParagraphAlignment.Center;

                Row ExtraNoteRow1 = table.AddRow();
                ExtraNoteRow1.HeadingFormat = true;
                ExtraNoteRow1.Format.Alignment = ParagraphAlignment.Center;
                ExtraNoteRow1.Format.Font.Bold = true;

                ExtraNoteRow1.Cells[0].AddParagraph("Extra Notes");
                ExtraNoteRow1.Cells[0].Format.Font.Size = 9;
                ExtraNoteRow1.Cells[0].Format.Font.Bold = true;
                ExtraNoteRow1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                ExtraNoteRow1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                ExtraNoteRow1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                Row ExtraNoteRow2 = table.AddRow();
                ExtraNoteRow2.HeadingFormat = true;
                ExtraNoteRow2.Format.Alignment = ParagraphAlignment.Center;
                ExtraNoteRow2.Format.Font.Bold = true;

                ExtraNoteRow2.Cells[0].AddParagraph(vehicleWorkOrderHistory.ExtraNotes);
                ExtraNoteRow2.Cells[0].Format.Font.Size = 9;
                ExtraNoteRow2.Cells[0].Format.Font.Bold = true;
                ExtraNoteRow2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                ExtraNoteRow2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                ExtraNoteRow2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
            }

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.10cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

        }        
    }
}
