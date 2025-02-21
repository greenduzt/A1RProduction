using A1QSystem.Model.Production;
using A1QSystem.Model.Products;
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
    public class PrintWeeklySchedulePDF
    {
        Document document;
        TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;
        private DateTime currentDate;
        private string day1;
        private string day2;
        private string day3;
        private string day4;
        private string day5;
        private List<WeeklySchedule> weeklySchedule1;
        private List<WeeklySchedule> weeklySchedule2;
        private List<WeeklySchedule> weeklySchedule3;
        private List<WeeklySchedule> weeklySchedule4;
        private List<WeeklySchedule> weeklySchedule5;

        public PrintWeeklySchedulePDF(List<WeeklySchedule> ws1, List<WeeklySchedule> ws2, List<WeeklySchedule> ws3, List<WeeklySchedule> ws4,
            List<WeeklySchedule> ws5, string d1, string d2, string d3, string d4, string d5)
        {
            currentDate = DateTime.Now;
            day1 = d1;
            day2 = d2;
            day3 = d3;
            day4 = d4;
            day5 = d5;
            weeklySchedule1 = ws1;
            weeklySchedule2 = ws2;
            weeklySchedule3 = ws3;
            weeklySchedule4 = ws4;
            weeklySchedule5 = ws5;
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

            string filename = "WS" + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";
            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/WeeklySchedule/" + filename);

                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/WeeklySchedule/" + filename);
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
            this.document.Info.Title = "Weekly Schedule";
            this.document.Info.Subject = "Weekly Schedule";
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

            paragraph = this.addressFrame.AddParagraph("Weekly Schedule");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = this.addressFrame.AddParagraph("Printed Date - " + DateTime.Now.ToString("dd/MM/yyy") + " at " + DateTime.Now.ToString("hh:mm tt"));
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 10;
            paragraph.Format.SpaceBefore = "0.6cm";
            paragraph.Format.Font.Bold = false;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();
            
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-2cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            //Footer page numbers
            TextFrame textFrame2 = section.Footers.Primary.AddTextFrame();
            textFrame2.Width = "18cm";
            textFrame2.Top = "10cm";
            Paragraph footerPara2 = textFrame2.AddParagraph();
            footerPara2.AddText("");
            footerPara2.Format.Font.Size = 7;
            footerPara2.Format.Alignment = ParagraphAlignment.Left;

            footerPara2 = textFrame2.AddParagraph();
            footerPara2.AddText("");
            footerPara2.Format.Font.Size = 7;
            footerPara2.Format.Alignment = ParagraphAlignment.Left;

            TextFrame textFrame1 = section.Footers.Primary.AddTextFrame();
            textFrame1.Width = "27.6cm";
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
            this.table.Borders.Color = MigraDoc.DocumentObjectModel.Colors.Black;
            this.table.Borders.Width = 0.5;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Borders.Bottom.Width = 0.5;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 1.0;
            this.table.BottomPadding = 1.0;


            Column column = this.table.AddColumn("5.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("5.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column = this.table.AddColumn("5.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column = this.table.AddColumn("5.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column = this.table.AddColumn("5.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row row3 = table.AddRow();
            row3.HeadingFormat = true;
            row3.Format.Alignment = ParagraphAlignment.Center;
            row3.Format.Font.Bold = true;

            row3.Cells[0].AddParagraph(day1);
            row3.Cells[0].Format.Font.Size = 9;
            row3.Cells[0].Format.Font.Bold = true;
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row3.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row3.Cells[1].AddParagraph(day2);
            row3.Cells[1].Format.Font.Size = 9;
            row3.Cells[1].Format.Font.Bold = true;
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row3.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row3.Cells[2].AddParagraph(day3);
            row3.Cells[2].Format.Font.Size = 9;
            row3.Cells[2].Format.Font.Bold = true;
            row3.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row3.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row3.Cells[3].AddParagraph(day4);
            row3.Cells[3].Format.Font.Size = 9;
            row3.Cells[3].Format.Font.Bold = true;
            row3.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row3.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row3.Cells[4].AddParagraph(day5);
            row3.Cells[4].Format.Font.Size = 9;
            row3.Cells[4].Format.Font.Bold = true;
            row3.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row3.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;            
            
            var w1 = weeklySchedule1.OrderByDescending(x => x.MixingShift);
            weeklySchedule1 = new List<WeeklySchedule>(w1);
            Construct(weeklySchedule1, 0);

            var w2 = weeklySchedule2.OrderByDescending(x => x.MixingShift);
            weeklySchedule2 = new List<WeeklySchedule>(w2);
            if(weeklySchedule2 != null)
            {
                weeklySchedule2.Insert(0, new WeeklySchedule() { RawProduct = new RawProduct() { Description = "" } });
            }            

            Construct(weeklySchedule2, 1);

            var w3 = weeklySchedule3.OrderByDescending(x => x.MixingShift);
            weeklySchedule3 = new List<WeeklySchedule>(w3);
            if (weeklySchedule3 != null)
            {
                weeklySchedule3.Insert(0, new WeeklySchedule() { RawProduct = new RawProduct() { Description = "" } });
            }
            Construct(weeklySchedule3, 2);

            var w4 = weeklySchedule4.OrderByDescending(x => x.MixingShift);
            weeklySchedule4 = new List<WeeklySchedule>(w4);
            if (weeklySchedule4 != null)
            {
                weeklySchedule4.Insert(0, new WeeklySchedule() { RawProduct = new RawProduct() { Description = "" } });
            }
            Construct(weeklySchedule4, 3);

            var w5 = weeklySchedule5.OrderByDescending(x => x.MixingShift);
            weeklySchedule5 = new List<WeeklySchedule>(w5);
            if (weeklySchedule5 != null)
            {
                weeklySchedule5.Insert(0, new WeeklySchedule() { RawProduct = new RawProduct() { Description = "" } });
            }
            Construct(weeklySchedule5, 4);
                      
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.5cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();
            
        }        

        private void Construct(List<WeeklySchedule> ws, int coll)
        {
            int r = table.Rows.Count;
            
            int firstMorning = ws.FindIndex(x => x.MixingShift == "Morning");
            if (firstMorning >= 0)
            {
                ws.Insert(firstMorning, new WeeklySchedule() { RawProduct = new RawProduct() { Description = "MORNING" } });
            }

            int firstArvo = ws.FindIndex(x => x.MixingShift == "Arvo");
            if (firstArvo >= 0)
            {
                ws.Insert(firstArvo, new WeeklySchedule() { RawProduct = new RawProduct() { Description = "ARVO" } });
            }

            int firstNight = ws.FindIndex(x => x.MixingShift == "Night");
            if (firstNight >= 0)
            {
                ws.Insert(firstArvo, new WeeklySchedule() { RawProduct = new RawProduct() { Description = "NIGHT" } });
            }

            for (int i = 0; i < ws.Count; i++)
            {                
                if (coll > 0 && r > i)
                {
                    if (!string.IsNullOrWhiteSpace(ws[i].RawProduct.Description))
                    {
                        if (ws[i].RawProduct.Description.Equals("MORNING") || ws[i].RawProduct.Description.Equals("ARVO") || ws[i].RawProduct.Description.Equals("NIGHT"))
                        {
                            table.Rows[i].HeadingFormat = true;
                            table.Rows[i].Format.Alignment = ParagraphAlignment.Center;
                            table.Rows[i].Format.Font.Bold = true;

                            table.Rows[i].Cells[coll].AddParagraph(ws[i].RawProduct.Description);
                            table.Rows[i].Cells[coll].Format.Font.Size = 9;
                            table.Rows[i].Cells[coll].Format.Font.Bold = true;
                            table.Rows[i].Cells[coll].Format.Alignment = ParagraphAlignment.Center;
                            table.Rows[i].Cells[coll].Shading.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;
                            table.Rows[i].Cells[coll].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                        }
                        else
                        {
                            table.Rows[i].HeadingFormat = true;
                            table.Rows[i].Format.Alignment = ParagraphAlignment.Center;
                            table.Rows[i].Format.Font.Bold = false;
                            table.Rows[i].BottomPadding = 1;

                            table.Rows[i].Cells[coll].AddParagraph(ws[i].RawProduct.Description + System.Environment.NewLine + ws[i].BottomRowString + System.Environment.NewLine + ws[i].Comments);
                            table.Rows[i].Cells[coll].Format.Font.Size = 8;
                            table.Rows[i].Cells[coll].Format.Font.Bold = true;
                            table.Rows[i].Cells[coll].Format.Alignment = ParagraphAlignment.Left;
                            table.Rows[i].Cells[coll].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            table.Rows[i].Cells[coll].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                        }
                    }
                }
                else
                {
                    if (ws[i].RawProduct.Description.Equals("MORNING") || ws[i].RawProduct.Description.Equals("ARVO") || ws[i].RawProduct.Description.Equals("NIGHT"))
                    {
                        Row row1 = table.AddRow();
                        row1.HeadingFormat = true;
                        row1.Format.Alignment = ParagraphAlignment.Center;
                        row1.Format.Font.Bold = true;

                        row1.Cells[coll].AddParagraph(ws[i].RawProduct.Description);
                        row1.Cells[coll].Format.Font.Size = 9;
                        row1.Cells[coll].Format.Font.Bold = true;
                        row1.Cells[coll].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[coll].Shading.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;
                        row1.Cells[coll].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    }
                    else
                    {
                        Row row2 = table.AddRow();
                        row2.HeadingFormat = true;
                        row2.Format.Alignment = ParagraphAlignment.Center;
                        row2.Format.Font.Bold = false;
                        row2.BottomPadding = 1;

                        row2.Cells[coll].AddParagraph(ws[i].RawProduct.Description + System.Environment.NewLine + ws[i].BottomRowString + System.Environment.NewLine + ws[i].Comments);
                        row2.Cells[coll].Format.Font.Size = 8;
                        row2.Cells[coll].Format.Font.Bold = true;
                        row2.Cells[coll].Format.Alignment = ParagraphAlignment.Left;
                        row2.Cells[coll].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        row2.Cells[coll].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                    }
                }                
            }
        }
    }
}
