using A1QSystem.Model.Production.ReRoll;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.PDFGeneration
{
    public class PrintLabelsPDF
    {
        private DateTime currentDate;
        private Document document;
        private ReRollingOrder reRollingProduction;
        private int noOfPrints;

        public PrintLabelsPDF(ReRollingOrder rrp, int p)
        {
            reRollingProduction = rrp;
            noOfPrints = p;
            currentDate = DateTime.Now;            
        }

        public Exception CreateLabel()
        {
            Exception res = null;
            string folder = "ReRollLabels";
            string prefix = "RRL";

            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.TopMargin = "5.5cm";
            document.DefaultPageSetup.FooterDistance = "1.5cm";
            document.DefaultPageSetup.LeftMargin = "1cm";
            document.DefaultPageSetup.RightMargin = "1cm";

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string filename = prefix + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";

            try
            {
                pdfRenderer.PdfDocument.Save("I:/PRODUCTION/DONOTDELETE/" + folder + "/" + filename);

                if (noOfPrints > 1)
                {
                    prefix = "C_RRL";
                    string cFileName = prefix + "_" + currentDate.ToString("_ddMMyyyy_HHmmss") + ".pdf";
                    string[] items = new string[noOfPrints];

                    for (int i = 0; i < noOfPrints; i++)
                    {
                        items[i] = "I:/PRODUCTION/DONOTDELETE/" + folder + "/" + filename;
                    }

                    PdfDocument outputDocument = new PdfDocument();
                    outputDocument.PageLayout = PdfPageLayout.SinglePage;
                    foreach (string file in items)
                    {
                        PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        int count = inputDocument.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfPage page = inputDocument.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }

                    outputDocument.Save("I:/PRODUCTION/DONOTDELETE/" + folder + "/" + cFileName);
                    filename = cFileName;
                }

                ProcessStartInfo info = new ProcessStartInfo("I:/PRODUCTION/DONOTDELETE/" + folder + "/" + filename);
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
            this.document.Info.Title = "A1 Rubber Roll";
            this.document.Info.Subject = "A1 Rubber roll";
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
            // Put a logo in the header
            Image image = section.Headers.Primary.AddImage(reRollingProduction.Product.LogoPath);
            image.Height = "4cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Left;
            image.WrapFormat.Style = WrapStyle.Through;

            Paragraph paragraph1 = document.LastSection.AddParagraph();
            paragraph1.AddLineBreak();

            Paragraph paragraph2 = document.LastSection.AddParagraph();
            paragraph2.Format.Borders.Color = Colors.Red;
            paragraph2.Format.Borders.Width = 1.5;
            paragraph2.Format.Shading.Color = Colors.Red;
            paragraph2.Format.Borders.Distance = 3;
            paragraph2.Format.Font.Color = Colors.White;
            paragraph2.Format.Font.Bold = true;
            paragraph2.Format.Font.Size = 24;
            paragraph2.Format.Alignment = ParagraphAlignment.Left;
            paragraph2.AddText("DENSITY");

            Paragraph paragraph3 = document.LastSection.AddParagraph();
            paragraph3.AddFormattedText(reRollingProduction.Product.Density, new Font("Verdana"));
            paragraph3.Format.Borders.Width = 1.5;
            paragraph3.Format.Borders.Color = Colors.Black;
            paragraph3.Format.Borders.Distance = 3;
            paragraph3.Format.Font.Size = 40;
            paragraph3.Format.Font.Bold = true;
            paragraph3.Format.Alignment = ParagraphAlignment.Center;

            Paragraph paragraph4 = document.LastSection.AddParagraph();
            paragraph4.AddLineBreak();

            Paragraph paragraph5 = document.LastSection.AddParagraph();
            paragraph5.Format.Borders.Color = Colors.Red;
            paragraph5.Format.Borders.Width = 1.5;
            paragraph5.Format.Shading.Color = Colors.Red;
            paragraph5.Format.Borders.Distance = 3;
            paragraph5.Format.Font.Color = Colors.White;
            paragraph5.Format.Font.Bold = true;
            paragraph5.Format.Font.Size = 24;
            paragraph5.Format.Alignment = ParagraphAlignment.Left;
            paragraph5.AddText("THICKNESS & WIDTH");

            Paragraph paragraph6 = document.LastSection.AddParagraph();
            paragraph6.AddFormattedText(reRollingProduction.Product.Tile.Thickness.ToString("G29") + "mm     /     " + reRollingProduction.Product.Width.ToString("G29") + "m", new Font("Verdana"));
            paragraph6.Format.Borders.Width = 1.5;
            paragraph6.Format.Borders.Color = Colors.Black;
            paragraph6.Format.Borders.Distance = 3;
            paragraph6.Format.Font.Size = 40;
            paragraph6.Format.Font.Bold = true;
            paragraph6.Format.Alignment = ParagraphAlignment.Center;

            Paragraph paragraph7 = document.LastSection.AddParagraph();
            paragraph7.AddLineBreak();

            Paragraph paragraph10 = document.LastSection.AddParagraph();
            paragraph10.Format.Borders.Color = Colors.Red;
            paragraph10.Format.Borders.Width = 1.5;
            paragraph10.Format.Shading.Color = Colors.Red;
            paragraph10.Format.Borders.Distance = 3;
            paragraph10.Format.Font.Color = Colors.White;
            paragraph10.Format.Font.Bold = true;
            paragraph10.Format.Font.Size = 24;
            paragraph10.Format.Alignment = ParagraphAlignment.Left;
            paragraph10.AddText("LINEAL METRES");

            Paragraph paragraph11 = document.LastSection.AddParagraph();
            paragraph11.AddFormattedText(reRollingProduction.Product.Tile.MaxYield.ToString("G29") + "L/m", new Font("Verdana"));
            paragraph11.Format.Borders.Width = 1.5;
            paragraph11.Format.Borders.Color = Colors.Black;
            paragraph11.Format.Borders.Distance = 3;
            paragraph11.Format.Font.Size = 66;
            paragraph11.Format.Font.Bold = true;
            paragraph11.Format.Alignment = ParagraphAlignment.Center;

            Paragraph paragraph12 = document.LastSection.AddParagraph();
            paragraph12.AddLineBreak();

            Paragraph paragraph13 = document.LastSection.AddParagraph();
            FormattedText formattedText = paragraph13.AddFormattedText("This product is made from post-consumer recycled tyres, therefore some variation may exist from roll to roll.", new Font("Verdana"));
            formattedText.Size = 9;
            paragraph13.AddLineBreak();
            formattedText = paragraph13.AddFormattedText("Please inspect each roll for such variance prior to adhesion to the substrate as no claims will be approved after installation.", new Font("Verdana"));
            formattedText.Size = 9;
            paragraph13.AddLineBreak();
            formattedText.AddTab();
            formattedText = paragraph13.AddFormattedText("Unroll, allow for acclimatization and install all products in the same rollout direction.", TextFormat.Bold);
            formattedText.Bold = true;
            formattedText.Size = 14;
            paragraph13.AddLineBreak();
            formattedText = paragraph13.AddFormattedText("Do not flip product over once unrolled. See installation guide for further instructions.", TextFormat.Bold);
            formattedText.Bold = true;
            formattedText.Size = 14;
            paragraph13.Format.Borders.Width = 1.5;
            paragraph13.Format.Borders.Color = Colors.Black;
            paragraph13.Format.Borders.Distance = 3;
            paragraph13.Format.Alignment = ParagraphAlignment.Center;

            Paragraph paragraph14 = document.LastSection.AddParagraph();
            paragraph14.AddLineBreak();


            //Put a logo in the footer
            Image image2 = section.Footers.Primary.AddImage("I:/PRODUCTION/DONOTDELETE/Logos/a1rubber_logo.png");
            image2.Height = "2.8cm";
            image2.LockAspectRatio = true;
            image2.RelativeVertical = RelativeVertical.Line;
            image2.RelativeHorizontal = RelativeHorizontal.Margin;
            image2.WrapFormat.Style = WrapStyle.Through;

            Paragraph footerPara2 = section.Footers.Primary.AddParagraph();
            FormattedText formattedText2 = footerPara2.AddFormattedText("P- (07) 3807 3666", new Font("Verdana"));
            formattedText2.AddTab();
            formattedText2 = footerPara2.AddFormattedText("34 Binary St., Yatala QLD 4207", new Font("Verdana"));
            formattedText2.AddLineBreak();
            formattedText2.Size = 9;
            formattedText2 = footerPara2.AddFormattedText("F- (07) 3807 2344", new Font("Verdana"));
            formattedText2.AddTab();
            formattedText2 = footerPara2.AddFormattedText("PO Box 6278, Yatala QLD 4207", new Font("Verdana"));
            formattedText2.AddLineBreak();
            formattedText2.Size = 9;
            formattedText2 = footerPara2.AddFormattedText("E- admin@a1rubber.com                               ", new Font("Verdana"));
            formattedText2.Size = 9;
            formattedText2.AddTab();
            formattedText2 = footerPara2.AddFormattedText("www.a1rubber.com", new Font("Verdana"));
            formattedText2.Size = 9;
            formattedText2.AddLineBreak();
            formattedText2.AddTab();
            formattedText2 = footerPara2.AddFormattedText("©Copyright A1Rubber " + DateTime.Now.Year, new Font("Verdana"));
            formattedText2.Size = 9;
            formattedText2.AddLineBreak();
            formattedText2.AddTab();
            formattedText2 = footerPara2.AddFormattedText("A1Rubber Console", new Font("Verdana"));
            formattedText2.Size = 9;
            footerPara2.Format.Alignment = ParagraphAlignment.Right;


        }
    }
}
