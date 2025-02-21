using A1QSystem.Model.Production;
using A1QSystem.Model.Stock;
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
using A1QSystem.DB;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Shreding;

namespace A1QSystem.PDFGeneration
{
    public class PrintProductionPDF
    {
        private string _workStationType;
        private string _shift;
        private DateTime _currentDate;
        private Document document;
        private TextFrame addressFrame;
        private string _tableHeader1;
        private DateTime _selectedDate;
        private List<ProductionHistory> _printingHistory;
        private List<GradingCompleted> _tmpGradingGradedCompleted;
        private List<GradedStockHistory> _gradedStockHistory;
        private List<GradingCompleted> _gradingGradedCompleted;
        private List<GradedStock> _gradedStock;
        private List<ShredStock> _shredStock;
        private List<ShreddingHistory> ShreddingHistory;
        MigraDoc.DocumentObjectModel.Tables.Table table;

        public PrintProductionPDF(string type, List<ProductionHistory> printingHistory, string shift, List<GradingCompleted> ggCom, List<GradedStockHistory> gsh, string tH, DateTime selectedDate, List<ShreddingHistory> sh)
        {
            _workStationType = type;
            _printingHistory = printingHistory;
            _shift = shift;
            _tmpGradingGradedCompleted = ggCom;
            _gradedStockHistory = gsh;
            _gradingGradedCompleted = new List<GradingCompleted>();
            _tableHeader1 = tH;
            _selectedDate = selectedDate;
            ShreddingHistory = sh;
            
        }

        public Exception CreateProductionPDF()
        {
            Exception res = null;
            _currentDate = DateTime.Now;

            //System.IO.FileInfo file = new System.IO.FileInfo(_workStationType);
            //string filename = file.Name.Remove((file.Name.Length - file.Extension.Length));
            string filename = _workStationType + "_"+_shift + _currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";

            //string filename = "Production"+_workStationType + _currentDate.ToString("ddMMyyyy") + ".pdf";
            //string filename = "test.pdf";
            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.FooterDistance = "-2.0cm";
            document.DefaultPageSetup.LeftMargin = "1.2cm";
            document.DefaultPageSetup.RightMargin = "1.2cm";
            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            try
            {
              
                    pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/Production/" + filename);
                    ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/Production/" + filename);
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
            this.document.Info.Title = "A1 Rubber " +_workStationType + " Completion";
            this.document.Info.Subject = "A1 Rubber " + _workStationType + " Completion";
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

            paragraph = this.addressFrame.AddParagraph("Completion Report - " + _workStationType);
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

            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Format.Font.Name = "Helvatica";
            this.table.Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;
            this.table.Borders.Color = MigraDoc.DocumentObjectModel.Colors.LightGray;
            this.table.Borders.Width = 0;
            this.table.Borders.Left.Width = 0;
            this.table.Borders.Right.Width = 0;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 4.0;
            this.table.BottomPadding = 1.0;

            Column columnTop1 = this.table.AddColumn("10cm");
            columnTop1.Format.Alignment = ParagraphAlignment.Center;
            columnTop1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            columnTop1 = this.table.AddColumn("2.5cm");
            columnTop1.Format.Alignment = ParagraphAlignment.Center;
            columnTop1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            columnTop1 = this.table.AddColumn("6cm");
            columnTop1.Format.Alignment = ParagraphAlignment.Center;
            columnTop1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

           
            Row rowTop1 = table.AddRow();
            rowTop1.HeadingFormat = true;
            rowTop1.Format.Alignment = ParagraphAlignment.Center;
            rowTop1.Format.Font.Bold = true;
            rowTop1.BottomPadding = 0.5;

            rowTop1.Cells[0].AddParagraph("Printed Date Time - " + DateTime.Now);
            rowTop1.Cells[0].Format.Font.Size = 12;
            rowTop1.Cells[0].Format.Font.Bold = true;
            rowTop1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowTop1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowTop1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowTop1.Cells[1].AddParagraph();
            rowTop1.Cells[1].Format.Font.Size = 12;
            rowTop1.Cells[1].Format.Font.Bold = true;
            rowTop1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            rowTop1.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowTop1.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            rowTop1.Cells[2].AddParagraph(_shift + "(" + _selectedDate.ToString("dd/MM/yyyy") + ")");
            rowTop1.Cells[2].Format.Font.Size = 12;
            rowTop1.Cells[2].Format.Font.Bold = true;
            rowTop1.Cells[2].Format.Alignment = ParagraphAlignment.Right;
            rowTop1.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            rowTop1.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            //Row rowTop2 = table.AddRow();
            //rowTop2.HeadingFormat = true;
            //rowTop2.Format.Alignment = ParagraphAlignment.Center;
            //rowTop2.Format.Font.Bold = true;
            //rowTop2.BottomPadding = 1;

            //rowTop2.Cells[0].AddParagraph("Printed Time - " + DateTime.Now.ToString("hh:mm tt"));
            //rowTop2.Cells[0].Format.Font.Size = 12;
            //rowTop2.Cells[0].Format.Font.Bold = true;
            //rowTop2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            //rowTop2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            //rowTop2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            //rowTop2.Cells[1].AddParagraph();
            //rowTop2.Cells[1].Format.Font.Size = 12;
            //rowTop2.Cells[1].Format.Font.Bold = true;
            //rowTop2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            //rowTop2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            //rowTop2.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            //rowTop2.Cells[2].AddParagraph("");
            //rowTop2.Cells[2].Format.Font.Size = 12;
            //rowTop2.Cells[2].Format.Font.Bold = true;
            //rowTop2.Cells[2].Format.Alignment = ParagraphAlignment.Right;
            //rowTop2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            //rowTop2.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.1cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            
                paragraph = section.AddParagraph(_tableHeader1.ToUpper());
                paragraph.Format.SpaceBefore = "-0.1cm";
                paragraph.Style = "Reference";
                paragraph.Format.Font.Bold = true;
                paragraph.Format.Font.Underline = Underline.Words;
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


                Column column2 = this.table.AddColumn("6cm");
                column2.Format.Alignment = ParagraphAlignment.Center;
                column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                column2 = this.table.AddColumn("9.5cm");
                column2.Format.Alignment = ParagraphAlignment.Center;
                column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                column2 = this.table.AddColumn("1.5cm");
                column2.Format.Alignment = ParagraphAlignment.Center;
                column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                column2 = this.table.AddColumn("1.5cm");
                column2.Format.Alignment = ParagraphAlignment.Center;
                column2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                Row row1 = table.AddRow();
                row1.HeadingFormat = true;
                row1.Format.Alignment = ParagraphAlignment.Center;
                row1.Format.Font.Bold = true;
                row1.BottomPadding = 1;

                row1.Cells[0].AddParagraph("Product Code");
                row1.Cells[0].Format.Font.Size = 12;
                row1.Cells[0].Format.Font.Bold = true;
                row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                row1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                row1.Cells[1].AddParagraph("Description");
                row1.Cells[1].Format.Font.Size = 12;
                row1.Cells[1].Format.Font.Bold = true;
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                row1.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                row1.Cells[2].AddParagraph("Qty");
                row1.Cells[2].Format.Font.Size = 12;
                row1.Cells[2].Format.Font.Bold = true;
                row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                row1.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                row1.Cells[3].AddParagraph("Type");
                row1.Cells[3].Format.Font.Size = 12;
                row1.Cells[3].Format.Font.Bold = true;
                row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                row1.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;


                foreach (var item in _printingHistory)
                {
                    Row row = table.AddRow();
                    row.HeadingFormat = true;
                    row.Format.Alignment = ParagraphAlignment.Center;
                    row.Format.Font.Bold = true;
                    row.BottomPadding = 1;

                    row.Cells[0].AddParagraph(item.RawProduct.RawProductCode);
                    row.Cells[0].Format.Font.Size = 10;
                    row.Cells[0].Format.Font.Bold = false;
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    row.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    row.Cells[1].AddParagraph(item.RawProduct.Description);
                    row.Cells[1].Format.Font.Size = 10;
                    row.Cells[1].Format.Font.Bold = false;
                    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    row.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    row.Cells[2].AddParagraph(item.Qty.ToString());
                    row.Cells[2].Format.Font.Size = 10;
                    row.Cells[2].Format.Font.Bold = false;
                    row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    row.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                    row.Cells[3].AddParagraph(item.RawProduct.RawProductType);
                    row.Cells[3].Format.Font.Size = 10;
                    row.Cells[3].Format.Font.Bold = false;
                    row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[3].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    row.Cells[3].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                }
                
                    paragraph = section.AddParagraph();
                    paragraph.Format.SpaceBefore = "-0.1cm";
                    paragraph.Style = "Reference";
                    paragraph.AddTab();

                    paragraph = section.AddParagraph("GRADES USED FROM MACHINE");
                    paragraph.Format.SpaceBefore = "-0.1cm";
                    paragraph.Style = "Reference";
                    paragraph.Format.Font.Underline = Underline.Words;
                    paragraph.Format.Font.Bold = true;
                    paragraph.AddTab();

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

                    Column gRdaingCol = this.table.AddColumn("4cm");
                    gRdaingCol.Format.Alignment = ParagraphAlignment.Center;
                    gRdaingCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    gRdaingCol = this.table.AddColumn("4cm");
                    gRdaingCol.Format.Alignment = ParagraphAlignment.Center;
                    gRdaingCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    gRdaingCol = this.table.AddColumn("3cm");
                    gRdaingCol.Format.Alignment = ParagraphAlignment.Center;
                    gRdaingCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;


                    Row gradingRow = table.AddRow();
                    gradingRow.HeadingFormat = true;
                    gradingRow.Format.Alignment = ParagraphAlignment.Center;
                    gradingRow.Format.Font.Bold = true;
                    gradingRow.BottomPadding = 1;

                    gradingRow.Cells[0].AddParagraph("Grade Name");
                    gradingRow.Cells[0].Format.Font.Size = 12;
                    gradingRow.Cells[0].Format.Font.Bold = true;
                    gradingRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    gradingRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                    gradingRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    gradingRow.Cells[1].AddParagraph("Kg Completed");
                    gradingRow.Cells[1].Format.Font.Size = 12;
                    gradingRow.Cells[1].Format.Font.Bold = true;
                    gradingRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                    gradingRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                    gradingRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    gradingRow.Cells[2].AddParagraph("Percentage");
                    gradingRow.Cells[2].Format.Font.Size = 12;
                    gradingRow.Cells[2].Format.Font.Bold = true;
                    gradingRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                    gradingRow.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                    gradingRow.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    GradingCompleted ph = new GradingCompleted();
                    _gradedStock = DBAccess.GetGradedStock();
                    _shredStock = DBAccess.GetShredStock();
                    if (_gradedStock.Count > 0)
                    {
                        var itemTGGC = _tmpGradingGradedCompleted.Where(i => i.OrderType == 1 || i.OrderType == 3).ToList();
                        
                        //var gradingCompletedNo1and3 =
                        //    from gradingCompleted in _tmpGradingGradedCompleted
                        //    group gradingCompleted by gradingCompleted.GradingID into gradingCompletedGroup
                        //    select new
                        //    {
                        //        GradingID = gradingCompletedGroup.Key,
                        //        GradingKg = gradingCompletedGroup.Where(i => i.OrderType == 1 || i.OrderType == 3).Sum(x => x.KGCompleted)
                        //    };


                        foreach (var item in itemTGGC)
                        {
                            if (_gradingGradedCompleted.Count == 0)
                            {
                                int ot = item.OrderType;
                                if (item.OrderType == 1)
                                {
                                    ph.OrderType = 3;
                                }

                                _gradingGradedCompleted.Add(new GradingCompleted() { ID = item.ID, ProdTimeTableID = item.ProdTimeTableID, GradingID = item.GradingID, GradingName = item.GradingName, KGCompleted = item.KGCompleted, Shift = item.Shift, CreatedDate = item.CreatedDate, OrderType = ot });
                            }
                            else
                            {
                                var yes = _gradingGradedCompleted.SingleOrDefault(x => x.GradingID == item.GradingID);
                                if (yes != null)
                                {
                                    for (int i = 0; i < _gradingGradedCompleted.Count; i++)
                                    {
                                        if (_gradingGradedCompleted[i].GradingID == item.GradingID)
                                        {
                                            _gradingGradedCompleted[i].KGCompleted += item.KGCompleted;
                                        }
                                    }
                                }
                                else
                                {
                                    int ot = item.OrderType;
                                    if (item.OrderType == 1)
                                    {
                                        ph.OrderType = 3;
                                    }
                                    _gradingGradedCompleted.Add(new GradingCompleted() { ID = item.ID, ProdTimeTableID = item.ProdTimeTableID, GradingID = item.GradingID, GradingName = item.GradingName, KGCompleted = item.KGCompleted, Shift = item.Shift, CreatedDate = item.CreatedDate, OrderType = ot });
                                }
                            }
                        } 
                    }

                    decimal sumLineTotal = (from od in _gradingGradedCompleted where od.OrderType == 1 || od.OrderType == 3 select od.KGCompleted).Sum();
                    decimal pTot = 0;
                    foreach (var item in _gradingGradedCompleted)
                    {
                        if (item.OrderType == 1 || item.OrderType == 3)
                        {
                            decimal p = Math.Round((item.KGCompleted / sumLineTotal) * 100, 2);
                            Row row = table.AddRow();
                            row.HeadingFormat = true;
                            row.Format.Alignment = ParagraphAlignment.Center;
                            row.Format.Font.Bold = true;
                            row.BottomPadding = 1;

                            row.Cells[0].AddParagraph(item.GradingName);
                            row.Cells[0].Format.Font.Size = 10;
                            row.Cells[0].Format.Font.Bold = false;
                            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            row.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            row.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                            row.Cells[1].AddParagraph(Convert.ToDecimal(item.KGCompleted).ToString("0.#####"));
                            row.Cells[1].Format.Font.Size = 10;
                            row.Cells[1].Format.Font.Bold = false;
                            row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                            row.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            row.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                            row.Cells[2].AddParagraph(p + "%");
                            row.Cells[2].Format.Font.Size = 10;
                            row.Cells[2].Format.Font.Bold = false;
                            row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                            row.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                            row.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                            pTot += p;
                        }
                    }

                    Row gradingRow1 = table.AddRow();
                    gradingRow1.HeadingFormat = true;
                    gradingRow1.Format.Alignment = ParagraphAlignment.Center;
                    gradingRow1.Format.Font.Bold = true;
                    gradingRow1.BottomPadding = 1;

                    gradingRow1.Cells[0].AddParagraph("TOTAL");
                    gradingRow1.Cells[0].Format.Font.Size = 12;
                    gradingRow1.Cells[0].Format.Font.Bold = true;
                    gradingRow1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    gradingRow1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                    gradingRow1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    gradingRow1.Cells[1].AddParagraph(Convert.ToDecimal(sumLineTotal).ToString("0.#####"));
                    gradingRow1.Cells[1].Format.Font.Size = 12;
                    gradingRow1.Cells[1].Format.Font.Bold = true;
                    gradingRow1.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                    gradingRow1.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                    gradingRow1.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    gradingRow1.Cells[2].AddParagraph(Math.Round(pTot, 0) + "%");
                    gradingRow1.Cells[2].Format.Font.Size = 12;
                    gradingRow1.Cells[2].Format.Font.Bold = true;
                    gradingRow1.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                    gradingRow1.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                    gradingRow1.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                    /**GRADES USED FROM GRADED STOCK / SHREDDED STOCK**/

                    _gradingGradedCompleted = new List<GradingCompleted>();
                    ph = new GradingCompleted();
                    if (_gradedStock.Count > 0)
                    {
                        //foreach (var itemGS in _gradedStock)
                        //{
                        //    ph = new GradingCompleted();
                        //    var itemTGGC = _tmpGradingGradedCompleted.Where(i => i.GradingID == itemGS.ID && i.OrderType == 4).ToList();
                        //    if (itemTGGC != null && itemTGGC.Count > 0)
                        //    {
                        //        foreach (var item in itemTGGC)
                        //        {
                        //            if (item.GradingID == itemGS.ID)
                        //            {
                        //                ph.ID = item.ID;
                        //                ph.ProdTimeTableID = item.ProdTimeTableID;
                        //                ph.GradingID = item.GradingID;
                        //                ph.GradingName = item.GradingName;
                        //                ph.KGCompleted += item.KGCompleted;
                        //                ph.Shift = item.Shift;
                        //                ph.CreatedDate = item.CreatedDate;
                        //                if (item.OrderType == 2)
                        //                {
                        //                    ph.OrderType = 4;
                        //                }
                        //                else
                        //                {
                        //                    ph.OrderType = item.OrderType;
                        //                }
                        //                _gradingGradedCompleted.Add(ph);
                        //            }
                        //        }
                        //    }
                        //}

                        var itemTGGC = _tmpGradingGradedCompleted.Where(i => i.OrderType == 4).ToList();

                        foreach (var item in itemTGGC)
                        {
                            if (_gradingGradedCompleted.Count == 0)
                            {
                                int ot = item.OrderType;
                                if (item.OrderType == 2)
                                {
                                    ph.OrderType = 4;
                                }
                                _gradingGradedCompleted.Add(new GradingCompleted() { ID = item.ID, ProdTimeTableID = item.ProdTimeTableID, GradingID = item.GradingID, GradingName = item.GradingName, KGCompleted = item.KGCompleted, Shift = item.Shift, CreatedDate = item.CreatedDate, OrderType = ot });
                            }
                            else
                            {
                                var yes = _gradingGradedCompleted.SingleOrDefault(x => x.GradingID == item.GradingID);
                                if (yes != null)
                                {
                                    for (int i = 0; i < _gradingGradedCompleted.Count; i++)
                                    {
                                        if (_gradingGradedCompleted[i].GradingID == item.GradingID)
                                        {
                                            _gradingGradedCompleted[i].KGCompleted += item.KGCompleted;
                                        }
                                    }
                                }
                                else
                                {
                                    int ot = item.OrderType;
                                    if (item.OrderType == 1)
                                    {
                                        ph.OrderType = 3;
                                    }
                                    _gradingGradedCompleted.Add(new GradingCompleted() { ID = item.ID, ProdTimeTableID = item.ProdTimeTableID, GradingID = item.GradingID, GradingName = item.GradingName, KGCompleted = item.KGCompleted, Shift = item.Shift, CreatedDate = item.CreatedDate, OrderType = ot });
                                }
                            }
                        }  


                    }

                    if (_shredStock.Count > 0)
                    {
                        foreach (var itemSS in _shredStock)
                        {
                            ph = new GradingCompleted();
                            var itemTGGC = _tmpGradingGradedCompleted.Where(i => i.GradingID == itemSS.Shred.ID).ToList();
                            if (itemTGGC != null && itemTGGC.Count > 0)
                            {
                                foreach (var item in itemTGGC)
                                {
                                    if (item.GradingID == itemSS.Shred.ID)
                                    {
                                        ph.ID = item.ID;
                                        ph.ProdTimeTableID = item.ProdTimeTableID;
                                        ph.GradingID = item.GradingID;
                                        ph.GradingName = itemSS.Shred.Name;
                                        ph.KGCompleted += item.KGCompleted;
                                        ph.Shift = item.Shift;
                                        ph.CreatedDate = item.CreatedDate;
                                        if (item.OrderType == 2)
                                        {
                                            ph.OrderType = 4;
                                        }
                                        else
                                        {
                                            ph.OrderType = item.OrderType;
                                        }
                                        _gradingGradedCompleted.Add(ph);
                                    }
                                }
                            }
                            //var itemGGC = _tmpGradingGradedCompleted.Where(k => k.GradingID == itemSS.Shred.ID).ToList();
                            //if (itemGGC != null)
                            //{
                            //    _gradingGradedCompleted.Add(ph);
                            //}
                        }
                    }

                    decimal gradedStockTotal = (from od in _gradingGradedCompleted where od.GradingID == 1 || od.GradingID == 2 || od.GradingID == 3 || od.GradingID == 4 || od.GradingID == 5 || od.GradingID == 6 || od.GradingID == 7 || od.GradingID == 8 select od.KGCompleted).Sum();
                    decimal gTot = 0;
                    if (gradedStockTotal > 0)
                    {
                        paragraph = section.AddParagraph();
                        paragraph.Format.SpaceBefore = "-0.1cm";
                        paragraph.Style = "Reference";
                        paragraph.AddTab();

                        paragraph = section.AddParagraph("GRADES USED FROM GRADED/SHREDDED STOCK");
                        paragraph.Format.SpaceBefore = "-0.1cm";
                        paragraph.Style = "Reference";
                        paragraph.Format.Font.Underline = Underline.Words;
                        paragraph.Format.Font.Bold = true;
                        paragraph.AddTab();

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

                        Column gStockCol = this.table.AddColumn("4cm");
                        gStockCol.Format.Alignment = ParagraphAlignment.Center;
                        gStockCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        gStockCol = this.table.AddColumn("4cm");
                        gStockCol.Format.Alignment = ParagraphAlignment.Center;
                        gStockCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        gStockCol = this.table.AddColumn("3cm");
                        gStockCol.Format.Alignment = ParagraphAlignment.Center;
                        gStockCol.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        Row gStockRow = table.AddRow();
                        gStockRow.HeadingFormat = true;
                        gStockRow.Format.Alignment = ParagraphAlignment.Center;
                        gStockRow.Format.Font.Bold = true;
                        gStockRow.BottomPadding = 1;

                        gStockRow.Cells[0].AddParagraph("Grade Name");
                        gStockRow.Cells[0].Format.Font.Size = 12;
                        gStockRow.Cells[0].Format.Font.Bold = true;
                        gStockRow.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                        gStockRow.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                        gStockRow.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        gStockRow.Cells[1].AddParagraph("Kg Completed");
                        gStockRow.Cells[1].Format.Font.Size = 12;
                        gStockRow.Cells[1].Format.Font.Bold = true;
                        gStockRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        gStockRow.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                        gStockRow.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        gStockRow.Cells[2].AddParagraph("Percentage");
                        gStockRow.Cells[2].Format.Font.Size = 12;
                        gStockRow.Cells[2].Format.Font.Bold = true;
                        gStockRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        gStockRow.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                        gStockRow.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        foreach (var item in _gradingGradedCompleted)
                        {
                            if (item.GradingID == 1 || item.GradingID == 2 || item.GradingID == 3 || item.GradingID == 4 || item.GradingID == 5 || item.GradingID == 6 || item.GradingID == 7 || item.GradingID == 8)
                            {
                                decimal p = Math.Round((item.KGCompleted / gradedStockTotal) * 100, 2);
                                Row row = table.AddRow();
                                row.HeadingFormat = true;
                                row.Format.Alignment = ParagraphAlignment.Center;
                                row.Format.Font.Bold = true;
                                row.BottomPadding = 1;

                                row.Cells[0].AddParagraph(item.GradingName);
                                row.Cells[0].Format.Font.Size = 10;
                                row.Cells[0].Format.Font.Bold = false;
                                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                                row.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                row.Cells[1].AddParagraph(Convert.ToDecimal(item.KGCompleted).ToString("0.#####"));
                                row.Cells[1].Format.Font.Size = 10;
                                row.Cells[1].Format.Font.Bold = false;
                                row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                                row.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                row.Cells[2].AddParagraph(p + "%");
                                row.Cells[2].Format.Font.Size = 10;
                                row.Cells[2].Format.Font.Bold = false;
                                row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                                row.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                gTot += p;
                            }
                        }
                        Row gStockRow1 = table.AddRow();
                        gStockRow1.HeadingFormat = true;
                        gStockRow1.Format.Alignment = ParagraphAlignment.Center;
                        gStockRow1.Format.Font.Bold = true;
                        gStockRow1.BottomPadding = 1;

                        gStockRow1.Cells[0].AddParagraph("TOTAL");
                        gStockRow1.Cells[0].Format.Font.Size = 12;
                        gStockRow1.Cells[0].Format.Font.Bold = true;
                        gStockRow1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                        gStockRow1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                        gStockRow1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        gStockRow1.Cells[1].AddParagraph(Convert.ToDecimal(gradedStockTotal).ToString("0.#####"));
                        gStockRow1.Cells[1].Format.Font.Size = 12;
                        gStockRow1.Cells[1].Format.Font.Bold = true;
                        gStockRow1.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                        gStockRow1.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                        gStockRow1.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        gStockRow1.Cells[2].AddParagraph(Math.Round(gTot, 0) + "%");
                        gStockRow1.Cells[2].Format.Font.Size = 12;
                        gStockRow1.Cells[2].Format.Font.Bold = true;
                        gStockRow1.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                        gStockRow1.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                        gStockRow1.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
                    }

                    /**END OF GRADES USED FROM GRADED STOCK / SHREDDED STOCK**/
                    if (_workStationType == "Grading")
                    {
                        if (_gradedStockHistory.Count > 0)
                        {

                            //GRADES ADDED TO GRADED STOCK
                            paragraph = section.AddParagraph();
                            paragraph.Format.SpaceBefore = "-0.1cm";
                            paragraph.Style = "Reference";
                            paragraph.AddTab();

                            paragraph = section.AddParagraph("QUANTITIES ADDED TO GRADED STOCK");
                            paragraph.Format.SpaceBefore = "-0.1cm";
                            paragraph.Style = "Reference";
                            paragraph.Format.Font.Underline = Underline.Words;
                            paragraph.Format.Font.Bold = true;
                            paragraph.AddTab();


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

                            Column gStockCol1 = this.table.AddColumn("4cm");
                            gStockCol1.Format.Alignment = ParagraphAlignment.Center;
                            gStockCol1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockCol1 = this.table.AddColumn("4cm");
                            gStockCol1.Format.Alignment = ParagraphAlignment.Center;
                            gStockCol1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockCol1 = this.table.AddColumn("3cm");
                            gStockCol1.Format.Alignment = ParagraphAlignment.Center;
                            gStockCol1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;


                            Row gStockRow2 = table.AddRow();
                            gStockRow2.HeadingFormat = true;
                            gStockRow2.Format.Alignment = ParagraphAlignment.Center;
                            gStockRow2.Format.Font.Bold = true;
                            gStockRow2.BottomPadding = 1;

                            gStockRow2.Cells[0].AddParagraph("Grade Name");
                            gStockRow2.Cells[0].Format.Font.Size = 12;
                            gStockRow2.Cells[0].Format.Font.Bold = true;
                            gStockRow2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            gStockRow2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                            gStockRow2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockRow2.Cells[1].AddParagraph("Kg Completed");
                            gStockRow2.Cells[1].Format.Font.Size = 12;
                            gStockRow2.Cells[1].Format.Font.Bold = true;
                            gStockRow2.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                            gStockRow2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                            gStockRow2.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockRow2.Cells[2].AddParagraph("Percentage");
                            gStockRow2.Cells[2].Format.Font.Size = 12;
                            gStockRow2.Cells[2].Format.Font.Bold = true;
                            gStockRow2.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                            gStockRow2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                            gStockRow2.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            decimal addedGradedStockTotal = _gradedStockHistory.Sum(x => x.Qty);
                            decimal gsTot = 0;
                            foreach (var item in _gradedStockHistory)
                            {
                                decimal p = Math.Round((item.Qty / addedGradedStockTotal) * 100, 2);
                                Row row = table.AddRow();
                                row.HeadingFormat = true;
                                row.Format.Alignment = ParagraphAlignment.Center;
                                row.Format.Font.Bold = true;
                                row.BottomPadding = 1;

                                row.Cells[0].AddParagraph(item.RubberGrades.GradeName);
                                row.Cells[0].Format.Font.Size = 10;
                                row.Cells[0].Format.Font.Bold = false;
                                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                                row.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                row.Cells[1].AddParagraph(Convert.ToDecimal(item.Qty).ToString("0.#####"));
                                row.Cells[1].Format.Font.Size = 10;
                                row.Cells[1].Format.Font.Bold = false;
                                row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                                row.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                row.Cells[2].AddParagraph(p + "%");
                                row.Cells[2].Format.Font.Size = 10;
                                row.Cells[2].Format.Font.Bold = false;
                                row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                                row.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                                gsTot += p;
                            }

                            Row gradingRow2 = table.AddRow();
                            gradingRow2.HeadingFormat = true;
                            gradingRow2.Format.Alignment = ParagraphAlignment.Center;
                            gradingRow2.Format.Font.Bold = true;
                            gradingRow2.BottomPadding = 1;

                            gradingRow2.Cells[0].AddParagraph("TOTAL");
                            gradingRow2.Cells[0].Format.Font.Size = 12;
                            gradingRow2.Cells[0].Format.Font.Bold = true;
                            gradingRow2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            gradingRow2.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                            gradingRow2.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gradingRow2.Cells[1].AddParagraph(Convert.ToDecimal(addedGradedStockTotal).ToString("0.#####"));
                            gradingRow2.Cells[1].Format.Font.Size = 12;
                            gradingRow2.Cells[1].Format.Font.Bold = true;
                            gradingRow2.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                            gradingRow2.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                            gradingRow2.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gradingRow2.Cells[2].AddParagraph(Math.Round(gsTot, 0) + "%");
                            gradingRow2.Cells[2].Format.Font.Size = 12;
                            gradingRow2.Cells[2].Format.Font.Bold = true;
                            gradingRow2.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                            gradingRow2.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                            gradingRow2.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                        }

                        if (ShreddingHistory.Count > 0)
                        {

                            //SHRED STOCK
                            paragraph = section.AddParagraph();
                            paragraph.Format.SpaceBefore = "-0.1cm";
                            paragraph.Style = "Reference";
                            paragraph.AddTab();

                            paragraph = section.AddParagraph("QUANTITIES ADDED TO SHREDDED STOCK");
                            paragraph.Format.SpaceBefore = "-0.1cm";
                            paragraph.Style = "Reference";
                            paragraph.Format.Font.Underline = Underline.Words;
                            paragraph.Format.Font.Bold = true;
                            paragraph.AddTab();

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

                            Column gStockCol2 = this.table.AddColumn("4cm");
                            gStockCol2.Format.Alignment = ParagraphAlignment.Center;
                            gStockCol2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockCol2 = this.table.AddColumn("4cm");
                            gStockCol2.Format.Alignment = ParagraphAlignment.Center;
                            gStockCol2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockCol2 = this.table.AddColumn("3cm");
                            gStockCol2.Format.Alignment = ParagraphAlignment.Center;
                            gStockCol2.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            Row gStockRow3 = table.AddRow();
                            gStockRow3.HeadingFormat = true;
                            gStockRow3.Format.Alignment = ParagraphAlignment.Center;
                            gStockRow3.Format.Font.Bold = true;
                            gStockRow3.BottomPadding = 1;

                            gStockRow3.Cells[0].AddParagraph("Shred Name");
                            gStockRow3.Cells[0].Format.Font.Size = 12;
                            gStockRow3.Cells[0].Format.Font.Bold = true;
                            gStockRow3.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            gStockRow3.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                            gStockRow3.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockRow3.Cells[1].AddParagraph("Kg Completed");
                            gStockRow3.Cells[1].Format.Font.Size = 12;
                            gStockRow3.Cells[1].Format.Font.Bold = true;
                            gStockRow3.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                            gStockRow3.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                            gStockRow3.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gStockRow3.Cells[2].AddParagraph("Percentage");
                            gStockRow3.Cells[2].Format.Font.Size = 12;
                            gStockRow3.Cells[2].Format.Font.Bold = true;
                            gStockRow3.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                            gStockRow3.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
                            gStockRow3.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            decimal addedShredStockTotal = ShreddingHistory.Sum(x => x.Total);
                            decimal ssTot = 0;
                            foreach (var item in ShreddingHistory)
                            {
                                decimal p = Math.Round((item.Total / addedShredStockTotal) * 100, 2);
                                Row row = table.AddRow();
                                row.HeadingFormat = true;
                                row.Format.Alignment = ParagraphAlignment.Center;
                                row.Format.Font.Bold = true;
                                row.BottomPadding = 1;

                                row.Cells[0].AddParagraph(item.ShredStock.Shred.Name);
                                row.Cells[0].Format.Font.Size = 10;
                                row.Cells[0].Format.Font.Bold = false;
                                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                                row.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                row.Cells[1].AddParagraph(item.Total.ToString("0.#####"));
                                row.Cells[1].Format.Font.Size = 10;
                                row.Cells[1].Format.Font.Bold = false;
                                row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                                row.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

                                row.Cells[2].AddParagraph(p + "%");
                                row.Cells[2].Format.Font.Size = 10;
                                row.Cells[2].Format.Font.Bold = false;
                                row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                                row.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
                                row.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;
                                ssTot += p;
                            }

                            Row gradingRow3 = table.AddRow();
                            gradingRow3.HeadingFormat = true;
                            gradingRow3.Format.Alignment = ParagraphAlignment.Center;
                            gradingRow3.Format.Font.Bold = true;
                            gradingRow3.BottomPadding = 1;

                            gradingRow3.Cells[0].AddParagraph("TOTAL");
                            gradingRow3.Cells[0].Format.Font.Size = 12;
                            gradingRow3.Cells[0].Format.Font.Bold = true;
                            gradingRow3.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            gradingRow3.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                            gradingRow3.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gradingRow3.Cells[1].AddParagraph(Convert.ToDecimal(addedShredStockTotal).ToString("0.#####"));
                            gradingRow3.Cells[1].Format.Font.Size = 12;
                            gradingRow3.Cells[1].Format.Font.Bold = true;
                            gradingRow3.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                            gradingRow3.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                            gradingRow3.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

                            gradingRow3.Cells[2].AddParagraph(Math.Round(ssTot, 0) + "%");
                            gradingRow3.Cells[2].Format.Font.Size = 12;
                            gradingRow3.Cells[2].Format.Font.Bold = true;
                            gradingRow3.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                            gradingRow3.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Gray;
                            gradingRow3.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
                        }
                    }
                
        }
    }
}
