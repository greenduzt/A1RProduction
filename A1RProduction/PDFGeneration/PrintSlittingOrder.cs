using A1QSystem.Core;
using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.ViewModel.Productions.Slitting;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MsgBox;
using PdfSharp.Pdf.Printing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A1QSystem.PDFGeneration
{
    public class PrintSlittingOrder
    {
        Document document;
        private SlittingConfirmationViewModel slittingProductionDetails;
        private DateTime currentDate;
        private bool isOffSpec;
        private int currShift;
        public PrintSlittingOrder(SlittingConfirmationViewModel ppd, bool os)
        {
            string folder = "SlittingYieldCut";
            string prefix = "SlitYield_Shift";
            isOffSpec = os;
            slittingProductionDetails = ppd;
            ShiftManager sm = new ShiftManager();
            currShift = sm.GetCurrentShift();
            currentDate = DateTime.Now;
            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.TopMargin = "5.5cm";
            document.DefaultPageSetup.FooterDistance = "1cm";
            document.DefaultPageSetup.LeftMargin = "1cm";
            document.DefaultPageSetup.RightMargin = "1cm";

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            if (isOffSpec)
            {
                folder = "SlittingOffSpec";
                prefix = "SlitOffSpec_";
            }

            string filename = prefix + currShift + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";

            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/" + folder + "/" + filename);
                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/" + folder + "/" + filename);
                info.Verb = "Print";
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(info);
            }
            catch (Exception ex)
            {
                Msg.Show("The file is currently opened. Please close the file and try again. " + ex.ToString(), "File is in use", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.Delete(Path.Combine(desktopPath, filename));
        }


        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "A1 Rubber Slitting Yield Cut";
            this.document.Info.Subject = "A1 Rubber Slitting Yield Cut";
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
            style.ParagraphFormat.AddTabStop("16cm", MigraDoc.DocumentObjectModel.TabAlignment.Right);

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

            string val = slittingProductionDetails.TotalYieldCutStr;
            if (isOffSpec)
            {
                val = slittingProductionDetails.OffSpecTilesStr;
            }

            Table coatingTable = new Table();
            coatingTable = section.AddTable();
            coatingTable.Style = "Table";
            coatingTable.Borders.Color = Color.FromCmyk(0, 0, 0, 100);
            coatingTable.Borders.Width = 1;
            coatingTable.Borders.Left.Width = 0.5;
            coatingTable.Borders.Right.Width = 0.5;
            coatingTable.Rows.LeftIndent = 0;


            Column column = coatingTable.AddColumn("6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("3.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            int r = 0;//Row number

            coatingTable.AddRow();
            coatingTable.Rows[r].HeadingFormat = true;
            coatingTable.Rows[r].Format.Alignment = ParagraphAlignment.Left;
            coatingTable.Rows[r].Borders.Left.Width = 0;
            coatingTable.Rows[r].Borders.Right.Width = 0;
            coatingTable.Rows[r].Borders.Top.Width = 0;
            coatingTable.Rows[r].Borders.Bottom.Width = 0;

            if (isOffSpec)
            {
                r += 1;
                coatingTable.AddRow();
                coatingTable[r, 0].AddParagraph("OFF- SPEC ONLY");
                coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
                coatingTable[r, 0].Format.Font.Bold = true;
                coatingTable[r, 0].MergeRight = 2;
                coatingTable[r, 0].Borders.Top.Width = 0;
                coatingTable[r, 0].Borders.Bottom.Width = 0;
                coatingTable[r, 0].Borders.Left.Width = 0;
                coatingTable[r, 2].Borders.Right.Width = 0;
                coatingTable[r, 0].Format.Font.Size = 34;

            }
            r += 1;
            coatingTable.AddRow();
            coatingTable[r, 0].AddParagraph();
            coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[r, 0].MergeRight = 2;
            coatingTable[r, 0].Borders.Top.Width = 0;
            coatingTable[r, 0].Borders.Bottom.Width = 0;
            coatingTable[r, 0].Borders.Left.Width = 0;
            coatingTable[r, 2].Borders.Right.Width = 0;
            coatingTable[r, 0].Format.Font.Size = 30;

            r += 1;
            coatingTable.AddRow();
            coatingTable[r, 0].AddParagraph(slittingProductionDetails.slittingProductionDetails.Product.ProductDescription);
            coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[r, 0].MergeRight = 2;
            coatingTable[r, 0].Borders.Top.Width = 0;
            coatingTable[r, 0].Borders.Bottom.Width = 0;
            coatingTable[r, 0].Borders.Left.Width = 0;
            coatingTable[r, 2].Borders.Right.Width = 0;
            coatingTable[r, 0].Format.Font.Size = 66;
            coatingTable[r, 0].Format.Font.Bold = true;

            r += 1;
            coatingTable.AddRow();
            coatingTable[r, 0].AddParagraph(slittingProductionDetails.slittingProductionDetails.Product.Tile.Thickness + "mm ");
            coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[r, 0].MergeRight = 2;
            coatingTable[r, 0].Borders.Top.Width = 0;
            coatingTable[r, 0].Borders.Bottom.Width = 0;
            coatingTable[r, 0].Borders.Left.Width = 0;
            coatingTable[r, 2].Borders.Right.Width = 0;
            coatingTable[r, 0].Format.Font.Size = 66;
            coatingTable[r, 0].Format.Font.Bold = true;

            r += 1;
            coatingTable.AddRow();
            coatingTable[r, 0].AddParagraph();
            coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[r, 0].MergeRight = 2;
            coatingTable[r, 0].Borders.Top.Width = 0;
            coatingTable[r, 0].Borders.Bottom.Width = 0;
            coatingTable[r, 0].Borders.Left.Width = 0;
            coatingTable[r, 2].Borders.Right.Width = 0;
            coatingTable[r, 0].Format.Font.Size = 30;

            r += 1;
            coatingTable.AddRow();
            coatingTable[r, 0].AddParagraph();
            coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[r, 0].MergeRight = 2;
            coatingTable[r, 0].Borders.Top.Width = 0;
            coatingTable[r, 0].Borders.Bottom.Width = 0;
            coatingTable[r, 0].Borders.Left.Width = 0;
            coatingTable[r, 2].Borders.Right.Width = 0;
            coatingTable[r, 0].Format.Font.Size = 15;

            r += 1;
            coatingTable.AddRow();
            coatingTable[r, 0].AddParagraph();
            coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[r, 0].MergeRight = 2;
            coatingTable[r, 0].Borders.Top.Width = 0;
            coatingTable[r, 0].Borders.Bottom.Width = 0;
            coatingTable[r, 0].Borders.Left.Width = 0;
            coatingTable[r, 2].Borders.Right.Width = 0;
            coatingTable[r, 0].Format.Font.Size = 15;

            string s = Convert.ToDecimal(val) > 1 ? " Tiles" : "Tile";
            r += 1;
            coatingTable.AddRow();
            //coatingTable[r, 0].AddParagraph(val.ToString() + " " + slittingProductionDetails.slittingProductionDetails.Product.ProductUnit + "S");
            coatingTable[r, 0].AddParagraph(val.ToString() + s);
            coatingTable[r, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[r, 0].Format.Font.Name = "Calibri";
            coatingTable[r, 0].MergeRight = 2;
            coatingTable[r, 0].Format.Font.Size = 66;
            coatingTable[r, 0].Format.Font.Bold = true;
            coatingTable[r, 0].Borders.Top.Width = 0;
            coatingTable[r, 0].Borders.Bottom.Width = 0;
            coatingTable[r, 0].Borders.Left.Width = 0;
            coatingTable[r, 2].Borders.Right.Width = 0;

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

