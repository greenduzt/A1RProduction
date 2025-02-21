using A1QSystem.Model;
using A1QSystem.Model.Vehicles;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace A1QSystem
{
    public class WorkOrderPDF
    {
        Document document;
        TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;
        public int WorkOrderId;
        public string SerialNo;
        public string Date;
        public List<Vehicle> CheklstItems;
        
        public string Comments;
        public string AllotedTime;
        public int SafetyItem;
        public int BreakDown;
        public int RoutineMain;
        public int Inspection;
        public string VehicleName;

        public WorkOrderPDF(List<Vehicle> _fItems, int _WOID, string _serialNo, string _date, string _comments, string _extTime, int _safetyItem, int _breakdown, int _routMain, int _inspection, string VName)
        {
            WorkOrderId = _WOID;
            SerialNo = _serialNo;
            Date = _date;
            CheklstItems = _fItems;
            
            Comments = _comments;
            AllotedTime = _extTime;
            SafetyItem = _safetyItem;
            BreakDown = _breakdown;
            RoutineMain = _routMain;
            Inspection = _inspection;
            VehicleName = VName;
        }
        
        public void createWorkOrderPDF(){

            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.FooterDistance = "0.2cm";
            document.DefaultPageSetup.LeftMargin = "1.2cm";
            document.DefaultPageSetup.RightMargin = "1.2cm";
            document.DefaultPageSetup.TopMargin = "3.3cm"; 
     
            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string str = SerialNo;

            string filename = str + WorkOrderId + ".pdf";
            pdfRenderer.PdfDocument.Save("I:/MAINTENANCE/Work Orders/" + filename);
            Process.Start("I:/MAINTENANCE/Work Orders/" + filename);

        }

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "Forklift Work Order";
            this.document.Info.Subject = "Forklift Work Order";
            this.document.Info.Author = "A1 Rubber";

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
            string Safety = ""; string Break = ""; string Routine = ""; string Insp = "";

                     
            MigraDoc.DocumentObjectModel.Section section = this.document.AddSection();
            MigraDoc.DocumentObjectModel.Shapes.Image image = section.Headers.Primary.AddImage("I:/PRODUCTION/QImg/a1rubber_logo.png");
            
            image.Height = "1.7cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;

            MigraDoc.DocumentObjectModel.Paragraph paragraph = section.AddParagraph();

            this.addressFrame = section.AddTextFrame();
            this.addressFrame.MarginLeft = "2.1cm";
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "7.0cm";
            this.addressFrame.Left = ShapePosition.Right;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "-1cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;

            paragraph = this.addressFrame.AddParagraph("Forklift Work Order");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            //Footer page numbers
            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddPageField();
            paragraph.AddText(" of ");
            paragraph.AddNumPagesField();
            paragraph.Format.Font.Size = 8;
            paragraph.Format.Font.Bold = false;
            paragraph.Format.Alignment = ParagraphAlignment.Center;        


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


            Column column2 = this.table.AddColumn("6.35cm");
            column2.Format.Alignment = ParagraphAlignment.Center;

            column2 = this.table.AddColumn("6.35cm");
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("2.92cm");
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column2 = this.table.AddColumn("2.92cm");
            column2.Format.Alignment = ParagraphAlignment.Center;
            column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row row3 = table.AddRow();
            row3.HeadingFormat = true;
            row3.Format.Alignment = ParagraphAlignment.Center;
            row3.Format.Font.Bold = true;

            row3.Cells[0].AddParagraph("Forklift Serial Number");
            row3.Cells[0].Format.Font.Size = 9;
            row3.Cells[0].Format.Font.Bold = true;
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row3.Cells[1].AddParagraph("Vehicle Name");
            row3.Cells[1].Format.Font.Size = 9;
            row3.Cells[1].Format.Font.Bold = true;
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row3.Cells[2].AddParagraph("Assigned Date");
            row3.Cells[2].Format.Font.Size = 9;
            row3.Cells[2].Format.Font.Bold = true;
            row3.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row3.Cells[3].AddParagraph("Work Order No");
            row3.Cells[3].Format.Font.Size = 9;
            row3.Cells[3].Format.Font.Bold = true;
            row3.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
            row3.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row row4 = table.AddRow();
            row4.HeadingFormat = true;
            row4.Format.Alignment = ParagraphAlignment.Center;
            row4.Format.Font.Bold = true;

            row4.Cells[0].AddParagraph(SerialNo);
            row4.Cells[0].Format.Font.Size = 11;
            row4.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[0].Format.Font.Bold = true;
            row4.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[1].AddParagraph(VehicleName);
            row4.Cells[1].Format.Font.Size = 8;
            row4.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[1].Format.Font.Bold = false;
            row4.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[2].AddParagraph(Date);
            row4.Cells[2].Format.Font.Size = 8;
            row4.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[2].Format.Font.Bold = false;
            row4.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[3].AddParagraph(WorkOrderId.ToString());
            row4.Cells[3].Format.Font.Size = 8;
            row4.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[3].Format.Font.Bold = false;
            row4.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

           
            if (SafetyItem == 1)
            {
                Safety = "Yes";
            }
            if (BreakDown == 1)
            {
                Break = "Yes";
            }
            if (RoutineMain == 1)
            {
                Routine = "Yes";
            }
            if (Inspection == 1)
            {
                Insp = "Yes";
            }

            // Create the item table
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;
            this.table.Borders.Visible = false;
            this.table.Borders.Left.Visible = false;
            this.table.Borders.Right.Visible = false;

            // Before you can add a row, you must define the columns
            Column col = this.table.AddColumn("5cm");
            col.Format.Alignment = ParagraphAlignment.Left;

            col = this.table.AddColumn("1cm");
            col.Format.Alignment = ParagraphAlignment.Left;

            col = this.table.AddColumn("2.5cm");
            col.Format.Alignment = ParagraphAlignment.Left;

            col = this.table.AddColumn("5.5cm");
            col.Format.Alignment = ParagraphAlignment.Left;

            col = this.table.AddColumn("1cm");
            col.Format.Alignment = ParagraphAlignment.Left;

            col = this.table.AddColumn("3.6cm");
            col.Format.Alignment = ParagraphAlignment.Left;


            // Create the header of the table
            Row r = table.AddRow();
            r.HeadingFormat = true;
            r.Format.Alignment = ParagraphAlignment.Center;
            r.Format.Font.Bold = true;

            r.Cells[0].AddParagraph("Breakdown maintenance");
            r.Cells[0].Format.Font.Bold = false;
            r.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            r.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;

            r.Cells[1].AddParagraph(Break);
            r.Cells[1].Borders.Visible = true;
            r.Cells[1].Borders.Left.Visible = true;
            r.Cells[1].Borders.Right.Visible = true;
            r.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            r.Cells[2].AddParagraph("");
            r.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            r.Cells[3].AddParagraph("Scheduled/ routine maintenance");
            r.Cells[3].Format.Font.Bold = false;
            r.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            r.Cells[3].VerticalAlignment = VerticalAlignment.Bottom;

            r.Cells[4].AddParagraph(Routine);
            r.Cells[4].Borders.Visible = true;
            r.Cells[4].Borders.Left.Visible = true;
            r.Cells[4].Borders.Right.Visible = true;
            r.Cells[4].Format.Alignment = ParagraphAlignment.Left;

            r.Cells[5].AddParagraph("");
            r.Cells[5].Borders.Visible = false;
            r.Cells[5].Borders.Left.Visible = true;
            r.Cells[5].Borders.Right.Visible = false;
            r.Cells[5].Format.Alignment = ParagraphAlignment.Left;

            Row r2 = table.AddRow();
            r2.HeadingFormat = true;
            r2.Format.Alignment = ParagraphAlignment.Center;
            r2.Format.Font.Bold = true;

            Row r1 = table.AddRow();
            r1.HeadingFormat = true;
            r1.Format.Alignment = ParagraphAlignment.Center;
            r1.Format.Font.Bold = true;



            r1.Cells[0].AddParagraph("Safety");
            r1.Cells[0].Format.Font.Bold = false;
            r1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            r1.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;

            r1.Cells[1].AddParagraph(Safety);
            r1.Cells[1].Borders.Visible = true;
            r1.Cells[1].Borders.Left.Visible = true;
            r1.Cells[1].Borders.Right.Visible = true;
            r1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

            r1.Cells[2].AddParagraph("");
            r1.Cells[2].Format.Alignment = ParagraphAlignment.Left;

            r1.Cells[3].AddParagraph("Specific work/ inspection");
            r1.Cells[3].Format.Font.Bold = false;
            r1.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            r1.Cells[3].VerticalAlignment = VerticalAlignment.Bottom;

            r1.Cells[4].AddParagraph(Insp);
            r1.Cells[4].Borders.Visible = true;
            r1.Cells[4].Borders.Left.Visible = true;
            r1.Cells[4].Borders.Right.Visible = true;
            r1.Cells[4].Format.Alignment = ParagraphAlignment.Left;

            r1.Cells[5].AddParagraph();
            r1.Cells[5].Borders.Visible = false;
            r1.Cells[5].Borders.Left.Visible = true;
            r1.Cells[5].Borders.Right.Visible = false;
            r1.Cells[5].Format.Alignment = ParagraphAlignment.Left;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            // Create the Second table
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

            Column mCol = this.table.AddColumn("6.35cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;

            mCol = this.table.AddColumn("4.17cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;
            mCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mCol = this.table.AddColumn("1.95cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;
            mCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mCol = this.table.AddColumn("2.15cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;
            mCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mCol = this.table.AddColumn("1.98cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;
            mCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mCol = this.table.AddColumn("1.98cm");
            mCol.Format.Alignment = ParagraphAlignment.Center;
            mCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row mRow = table.AddRow();
            mRow.HeadingFormat = true;
            mRow.Format.Alignment = ParagraphAlignment.Center;
            row3.Format.Font.Bold = true;

            mRow.Cells[0].AddParagraph("Mechanic Name");
            mRow.Cells[0].Format.Font.Size = 9;
            mRow.Cells[0].Format.Font.Bold = true;
            mRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            mRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            mRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            mRow.Cells[1].AddParagraph("Hourly Meter Reading");
            mRow.Cells[1].Format.Font.Size = 9;
            mRow.Cells[1].Format.Font.Bold = true;
            mRow.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            mRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            mRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            mRow.Cells[2].AddParagraph("Date");
            mRow.Cells[2].Format.Font.Size = 9;
            mRow.Cells[2].Format.Font.Bold = true;
            mRow.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            mRow.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            mRow.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            mRow.Cells[3].AddParagraph("Alloted Time");
            mRow.Cells[3].Format.Font.Size = 9;
            mRow.Cells[3].Format.Font.Bold = true;
            mRow.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            mRow.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            mRow.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            mRow.Cells[4].AddParagraph("Start Time");
            mRow.Cells[4].Format.Font.Size = 9;
            mRow.Cells[4].Format.Font.Bold = true;
            mRow.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            mRow.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            mRow.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            mRow.Cells[5].AddParagraph("Finish Time");
            mRow.Cells[5].Format.Font.Size = 9;
            mRow.Cells[5].Format.Font.Bold = true;
            mRow.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            mRow.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            mRow.Cells[5].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            Row mRow2 = table.AddRow();
            mRow2.HeadingFormat = true;
            mRow2.Format.Alignment = ParagraphAlignment.Center;
            mRow2.Format.Font.Bold = true;

            mRow2.Cells[0].AddParagraph("");
            mRow2.Cells[0].Format.Font.Size = 8;
            mRow2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            mRow2.Cells[0].Format.Font.Bold = false;
            mRow2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mRow2.Cells[1].AddParagraph("");
            mRow2.Cells[1].Format.Font.Size = 8;
            mRow2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            mRow2.Cells[1].Format.Font.Bold = false;
            mRow2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mRow2.Cells[2].AddParagraph("");
            mRow2.Cells[2].Format.Font.Size = 8;
            mRow2.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            mRow2.Cells[2].Format.Font.Bold = false;
            mRow2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mRow2.Cells[3].AddParagraph(AllotedTime + "Hrs");
            mRow2.Cells[3].Format.Font.Size = 8;
            mRow2.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            mRow2.Cells[3].Format.Font.Bold = false;
            mRow2.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mRow2.Cells[4].AddParagraph("");
            mRow2.Cells[4].Format.Font.Size = 8;
            mRow2.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            mRow2.Cells[4].Format.Font.Bold = false;
            mRow2.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            mRow2.Cells[5].AddParagraph("");
            mRow2.Cells[5].Format.Font.Size = 8;
            mRow2.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            mRow2.Cells[5].Format.Font.Bold = false;
            mRow2.Cells[5].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();
                                 
           

          // Create the item table
          this.table = section.AddTable();
          this.table.Style = "Table"; 
          this.table.Borders.Width = 0.25;
          this.table.Borders.Left.Width = 0.5;
          this.table.Borders.Right.Width = 0.5;
          this.table.Rows.LeftIndent = 0;

          // Before you can add a row, you must define the columns
          Column column = this.table.AddColumn("3.5cm");
          column.Format.Alignment = ParagraphAlignment.Left;

          column = this.table.AddColumn("12.5cm");
          column.Format.Alignment = ParagraphAlignment.Left;

          column = this.table.AddColumn("2.0cm");
          column.Format.Alignment = ParagraphAlignment.Center;          
          
          column = this.table.AddColumn("0.6cm");
          column.Format.Alignment = ParagraphAlignment.Center;

          // Create the header of the table
          Row rows = table.AddRow();
          rows.HeadingFormat = true;
          rows.Format.Alignment = ParagraphAlignment.Center;
          rows.Format.Font.Bold = true;

          rows.Cells[0].AddParagraph("Part Name");
          rows.Cells[0].Format.Font.Bold = false;
          rows.Cells[0].Format.Alignment = ParagraphAlignment.Left;
          rows.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
          rows.Cells[0].MergeDown = 1;

          rows.Cells[1].AddParagraph("Comments");
          rows.Cells[1].Format.Alignment = ParagraphAlignment.Left;
          rows.Cells[1].MergeDown = 1;

          rows.Cells[2].AddParagraph("JSA");
          rows.Cells[2].Format.Alignment = ParagraphAlignment.Center;
          rows.Cells[2].MergeDown = 1;

          rows.Cells[3].AddParagraph("Tick");
          rows.Cells[3].Format.Alignment = ParagraphAlignment.Center;
          rows.Cells[3].VerticalAlignment = VerticalAlignment.Bottom;
          rows.Cells[3].MergeDown = 1;
          rows.Cells[3].Format.Font.Size = "7";

          rows = table.AddRow();
          rows.HeadingFormat = true;
          rows.Format.Alignment = ParagraphAlignment.Center;
          rows.Format.Font.Bold = true;

                            
          for (int i = 0; i < CheklstItems.Count; i++)
          {
              //if (CheklstItems[i].PartAdded == true)
              //{
              //      Row row1 = this.table.AddRow();
              //      Row row2 = this.table.AddRow();

              //      row1.TopPadding = 1.5;
              //      row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
              //      row1.Cells[0].MergeDown = 1;
              //      row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;

              //      row1.Cells[2].Format.Font.Size = "7";
              //      row2.Cells[2].Format.Font.Size = "7";

              //      for (int x = 0; x < RiskMatrix.Count; x++)
              //      {
              //          if (RiskMatrix[x].PartNumber == CheklstItems[i].PartNumber)
              //          {
              //              row1.Cells[2].AddParagraph(getSeverity(RiskMatrix[x].Code));                            
                            
              //              break;
              //          }
              //      }

              //      for (int y = 0; y < RiskMatrix.Count; y++)
              //      {
              //          if (RiskMatrix[y].PartNumber.ToString() == "11" + CheklstItems[i].PartNumber)
              //          {
              //              row2.Cells[2].AddParagraph(getSeverity(RiskMatrix[y].Code));
              //              break;
              //          }
              //      }  
                                           

              //      row1.Cells[0].AddParagraph(CheklstItems[i].PartName);
              //      paragraph = row1.Cells[1].AddParagraph();
              //      paragraph.AddFormattedText(CheklstItems[i].PartDescription, TextFormat.Bold);

              //      row2.Cells[1].AddParagraph(CheklstItems[i].PartComment);                    
              //      row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;
             
              //}
          }

         

          if (string.IsNullOrEmpty(Comments))
          {
              Comments = "\n\n\n";
          }
          else
          {
              Comments = Comments + "\n\n";
          }

          Row rowB = this.table.AddRow();
          rowB.Borders.Visible = false;

          rowB = this.table.AddRow();
          rowB.Cells[0].Borders.Visible = true;
          rowB.Cells[0].Borders.Left.Visible = true;
          rowB.Cells[0].Borders.Right.Visible = true;
          rowB.Cells[0].AddParagraph("Comments:");
          rowB.Cells[0].Format.Font.Bold = true;
          rowB.Cells[0].Format.Alignment = ParagraphAlignment.Left;
          rowB.Cells[1].AddParagraph(Comments);
          rowB.Cells[1].MergeRight = 2;
          rowB.Cells[1].Borders.Visible = true;
          rowB.Cells[1].Borders.Left.Visible = true;
          rowB.Cells[1].Borders.Right.Visible = true;

          /*Row rowC = this.table.AddRow();
          rowB.Borders.Visible = false;

          rowC = this.table.AddRow();
          rowC.Cells[0].Borders.Visible = true;
          rowC.Cells[0].Borders.Left.Visible = true;
          rowC.Cells[0].Borders.Right.Visible = true;
          rowC.Cells[0].AddParagraph("");
          rowC.Cells[0].Format.Font.Bold = true;
          rowC.Cells[0].Format.Alignment = ParagraphAlignment.Left;
          rowC.Cells[1].AddParagraph(Comments);
          rowC.Cells[1].MergeRight = 2;
          rowC.Cells[1].Borders.Visible = true;
          rowC.Cells[1].Borders.Left.Visible = true;
          rowC.Cells[1].Borders.Right.Visible = true;
*/
          Row rowD = this.table.AddRow();
          rowD.Borders.Visible = false;

          rowD = this.table.AddRow();
          rowD.Cells[0].Borders.Visible = true;
          rowD.Cells[0].Borders.Left.Visible = true;
          rowD.Cells[0].Borders.Right.Visible = true;
          rowD.Cells[0].AddParagraph("Mechanic's Comments:");
          rowD.Cells[0].Format.Font.Bold = true;
          rowD.Cells[0].Format.Alignment = ParagraphAlignment.Left;
          rowD.Cells[1].AddParagraph("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
          rowD.Cells[1].MergeRight = 2;
          rowD.Cells[1].Borders.Visible = true;
          rowD.Cells[1].Borders.Left.Visible = true;
          rowD.Cells[1].Borders.Right.Visible = true;

          Row rowE = this.table.AddRow();
          rowE.Borders.Visible = false;
          Row rowF = this.table.AddRow();
          rowF.Borders.Visible = false;
      
          Row rowG = this.table.AddRow();
          rowG.Borders.Visible = false;
          rowG.Cells[0].AddParagraph("Signature:");
          rowG.Cells[0].Format.Font.Bold = true;
          rowG.Cells[0].Format.Alignment = ParagraphAlignment.Left;
          rowG.Cells[1].AddParagraph("Date:");
          rowG.Cells[1].Format.Font.Bold = true;
          rowG.Cells[1].Format.Alignment = ParagraphAlignment.Right;
          
         
        }

        private string getSeverity(string code){
            string level = "";
            string Stringpart = "";
            string likelihood = "";
                switch (code)
                {
                    case "1A":
                        level= "High";
                        break;
                    case "1B":
                         level= "Moderate";
                        break;
                    case "1C":
                        level= "Low";
                        break;
                    case "1D":
                        level= "Low";
                        break;
                    case "1E":
                        level= "Low";
                        break;
                    case "2A":
                       level= "High";
                        break;
                    case "2B":
                        level= "High";
                        break;
                    case "2C":
                        level= "Moderate";
                        break;
                    case "2D":
                        level= "Low";
                        break;
                    case "2E":
                         level= "Low";
                        break;
                    case "3A":
                        level= "Extreme";
                        break;
                    case "3B":
                        level= "High";
                        break;
                    case "3C":
                         level= "High";
                        break;
                    case "3D":
                        level= "Moderate";
                        break;
                    case "3E":
                        level= "Moderate";
                        break;
                    case "4A":
                        level= "Extreme";
                        break;
                    case "4B":
                        level= "Extreme";
                        break;
                    case "4C":
                        level= "Extreme";
                        break;
                    case "4D":
                        level= "High";
                        break;
                    case "4E":
                        level= "High";
                        break;
                    case "5A":
                        level= "Extreme";
                        break;
                    case "5B":
                        level= "Extreme";
                        break;
                    case "5C":
                        level= "Extreme";
                        break;
                    case "5D":
                        level= "Extreme";
                        break;
                    case "5E":
                        level= "High";
                        break;
                }

                foreach (char c in code)
                {
                    if (Char.IsLetter(c))
                    {
                        Stringpart += c;
                    }
                  
                }

                if (Stringpart == "A")
                {
                    likelihood = "Alomost Certain";
                }
                else if (Stringpart == "B")
                {
                    likelihood = "Likely";
                }
                else if (Stringpart == "C")
                {
                    likelihood = "Possible";
                }
                else if (Stringpart == "D")
                {
                    likelihood = "Unlikely";
                }
                else if (Stringpart == "E")
                {
                    likelihood = "Rare";
                }
                else
                {
                    likelihood = "";
                }

                return likelihood + " " + level;
        }
        
    }
}
