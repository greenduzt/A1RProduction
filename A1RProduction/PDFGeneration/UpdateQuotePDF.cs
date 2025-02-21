using A1QSystem.Model;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBox;

namespace A1QSystem.PDFGeneration
{
    public class UpdateQuotePDF
    {
        Document document;
        TextFrame addressFrame;
        MigraDoc.DocumentObjectModel.Tables.Table table;

        private string SoldTo;
        private int QuoteNo;
        private string Prefix;
        private string CustomerName;
        private string ProName;
        private string ShipTo;
        private string RepName;
        private string FreightName;
        private string FreightDescription;
        private string FreightUnit;
        private decimal FreightPrice;
        private decimal FreightQty;
        private decimal FreightDisc;
        private string Instructions;
        private decimal FreightTotal;
        private BindingList<QuoteDetails> ItemCollection;

        public UpdateQuotePDF() {}

        public bool CreateQuote(Quote newQuote)
        {
            bool result = false;
            SoldTo = newQuote.customer.CompanyName;
            QuoteNo = newQuote.QuoteID;
            Prefix = newQuote.Prefix;
            CustomerName = newQuote.customer.CompanyName;
            ProName = newQuote.ProName;
            ShipTo = newQuote.freightDetails.ShipToAddress;
            RepName = newQuote.SalesPerson;
            FreightName = newQuote.freightDetails.FreightName;
            FreightDescription = newQuote.freightDetails.FreightDescription;
            FreightUnit = newQuote.freightDetails.FreightUnit;
            FreightPrice = newQuote.freightDetails.FreightPrice;
            FreightQty = newQuote.freightDetails.FreightPallets;
            FreightDisc = newQuote.freightDetails.FreightDiscount;
            Instructions = newQuote.Instructions;
            FreightTotal = newQuote.freightDetails.FreightTotal;
            ItemCollection = newQuote.quoteDetails;

            Document document = CreateDocument(QuoteNo);
            document.UseCmykColor = true;
            document.DefaultPageSetup.FooterDistance = "-2.0cm";
            document.DefaultPageSetup.LeftMargin = "1.2cm";
            document.DefaultPageSetup.RightMargin = "1.2cm";
            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string str = CustomerName;
            string fStr = "";
            str.Split(' ').ToList().ForEach(i => fStr = fStr + i[0]);

            string filename = fStr + " " + ProName + " " + Prefix + QuoteNo + ".pdf";

            try
            {
                //Just opening the file as open/create
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    
                    pdfRenderer.PdfDocument.Save("I:/SALES SUPPORT/CUSTOMERS/Customer Quotes/" + filename);
                    Process.Start("I:/SALES SUPPORT/CUSTOMERS/Customer Quotes/" + filename);                   
                    
                    result = true;
                }                
            }
            catch (IOException ex)
            {
                result = false;
                Msg.Show("The file is currently opened. Please close the file and try again.", "File is in use", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.Delete(Path.Combine(desktopPath, filename));


            return result;
        }

        public Document CreateDocument(int QuoteNo)
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "A1 Rubber Quotation";
            this.document.Info.Subject = "A1 Rubber Quotation";
            this.document.Info.Author = "A1 Rubber";

            DefineStyles();
            CreatePage(QuoteNo);
            FillContent();

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

        void CreatePage(int QuoteNo)
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

            MigraDoc.DocumentObjectModel.Paragraph paragraph1 = section.Footers.Primary.AddParagraph();
            paragraph1.AddText("QLD Delivery/Pickup Address\n34 Binary Street\nYatala QLD 4207\nPh: 07 3807 3666\nFax: 07 3807 2344");
            paragraph1.Format.Font.Size = 8;
            paragraph1.Format.Font.Bold = false;
            paragraph1.Format.Alignment = ParagraphAlignment.Left;

            MigraDoc.DocumentObjectModel.Paragraph paragraph2 = section.Footers.Primary.AddParagraph();
            paragraph2.AddText("NSW Delivery/Pickup Address\n40 Bentley Street\nWetherill Park NSW 2164\nPh: 02 9756 2146\nFax: 02 9756 2149");
            paragraph2.Format.Font.Size = 8;
            paragraph2.Format.Font.Bold = false;
            paragraph2.Format.Alignment = ParagraphAlignment.Right;

            MigraDoc.DocumentObjectModel.Paragraph paragraph3 = section.Footers.Primary.AddParagraph();
            paragraph3.AddText("VIC Sales Representative\nC/- Leeanne Pagel\nPh: 0408 607 888");
            paragraph3.Format.Font.Size = 8;
            paragraph3.Format.Font.Bold = false;
            paragraph3.Format.Alignment = ParagraphAlignment.Center;

            this.addressFrame = section.AddTextFrame();
            this.addressFrame.MarginLeft = "3.8cm";
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "7.0cm";
            this.addressFrame.Left = ShapePosition.Right;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "-0.1cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;

            paragraph = this.addressFrame.AddParagraph("QUOTATION");
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = 17;
            paragraph.Format.SpaceBefore = "2cm";
            paragraph.Format.Font.Bold = true;

            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.1cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            // Create the item table
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


            Column column2 = this.table.AddColumn("12.7cm");
            column2.Format.Alignment = ParagraphAlignment.Center;

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

            row3.Cells[0].AddParagraph("    www.a1rubber.com");
            row3.Cells[0].Format.Font.Size = 9;
            row3.Cells[0].Format.Font.Bold = true;
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row3.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black;

            row3.Cells[1].AddParagraph("Date");
            row3.Cells[1].Format.Font.Size = 9;
            row3.Cells[1].Format.Font.Bold = true;
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
            row3.Cells[1].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row3.Cells[2].AddParagraph("Quote No");
            row3.Cells[2].Format.Font.Size = 9;
            row3.Cells[2].Format.Font.Bold = true;
            row3.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row3.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
            row3.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            Row row4 = table.AddRow();
            row4.HeadingFormat = true;
            row4.Format.Alignment = ParagraphAlignment.Center;
            row4.Format.Font.Bold = true;

            row4.Cells[0].AddParagraph("    ABN: 92 095 559 130");
            row4.Cells[0].Format.Font.Size = 8;
            row4.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[0].Format.Font.Bold = false;
            row4.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row4.Cells[1].AddParagraph(DateTime.Now.ToString("dd/MM/yyyy"));
            row4.Cells[1].Format.Font.Size = 8;
            row4.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[1].Format.Font.Bold = false;
            row4.Cells[1].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            row4.Cells[2].AddParagraph(Prefix+QuoteNo.ToString());
            row4.Cells[2].Format.Font.Size = 8;
            row4.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row4.Cells[2].Format.Font.Bold = false;
            row4.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.AliceBlue;


            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "0.1cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            // Create the item table
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

            // Before you can add a row, you must define the columns
            Column column1 = this.table.AddColumn("5.85cm");
            column1.Format.Alignment = ParagraphAlignment.Center;

            column1 = this.table.AddColumn("0.5cm");
            column1.Format.Alignment = ParagraphAlignment.Center;
            column1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column1 = this.table.AddColumn("5.85cm");
            column1.Format.Alignment = ParagraphAlignment.Center;

            column1 = this.table.AddColumn("0.5cm");
            column1.Format.Alignment = ParagraphAlignment.Center;
            column1.Shading.Color = MigraDoc.DocumentObjectModel.Colors.White;

            column1 = this.table.AddColumn("5.85cm");
            column1.Format.Alignment = ParagraphAlignment.Center;

            Row row1 = table.AddRow();
            row1.HeadingFormat = true;
            row1.Format.Alignment = ParagraphAlignment.Center;
            row1.Format.Font.Bold = true;

            row1.Cells[0].AddParagraph("Sold To");
            row1.Cells[0].Format.Font.Size = 9;
            row1.Cells[0].Format.Font.Bold = true;
            row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row1.Cells[0].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
            row1.Cells[0].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row1.Cells[1].AddParagraph("");
            row1.Cells[1].Format.Font.Size = 9;
            row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row1.Cells[2].AddParagraph("Project Name");
            row1.Cells[2].Format.Font.Size = 9;
            row1.Cells[2].Format.Font.Bold = true;
            row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row1.Cells[2].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;
            row1.Cells[2].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;

            row1.Cells[3].AddParagraph("");
            row1.Cells[3].Format.Font.Size = 9;
            row1.Cells[3].Format.Alignment = ParagraphAlignment.Left;

            row1.Cells[4].AddParagraph("Ship To");
            row1.Cells[4].Format.Font.Size = 9;
            row1.Cells[4].Format.Font.Bold = true;
            row1.Cells[4].Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.White;
            row1.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row1.Cells[4].Shading.Color = MigraDoc.DocumentObjectModel.Colors.Red;


            Row row2 = table.AddRow();
            row2.HeadingFormat = true;
            row2.Format.Alignment = ParagraphAlignment.Center;
            row2.Format.Font.Bold = true;

            row2.Cells[0].AddParagraph(SoldTo);
            row2.Cells[0].Format.Font.Size = 8;
            row2.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[0].Format.Font.Bold = false;

            row2.Cells[1].AddParagraph("");
            row2.Cells[1].Format.Font.Size = 8;
            row2.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[1].Format.Font.Bold = false;

            row2.Cells[2].AddParagraph(ProName);
            row2.Cells[2].Format.Font.Size = 8;
            row2.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[2].Format.Font.Bold = false;

            row2.Cells[3].AddParagraph("");
            row2.Cells[3].Format.Font.Size = 8;
            row2.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[3].Format.Font.Bold = false;

            row2.Cells[4].AddParagraph(ShipTo);
            row2.Cells[4].Format.Font.Size = 8;
            row2.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row2.Cells[4].Format.Font.Bold = false;


            // Create the text frame for the address 
            this.addressFrame = section.AddTextFrame();
            // this.addressFrame.MarginLeft = "3.0cm";
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "18.5cm";
            this.addressFrame.Left = ShapePosition.Left;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;

            this.addressFrame.MarginTop = "27cm";
            this.addressFrame.RelativeVertical = RelativeVertical.Page;

            /*   paragraph = this.addressFrame.AddParagraph("This Quote cannot be used as an invoice. Should you wish to proceed, please send an acceptance of this quote and you will be sent an invoice.");
               paragraph.Format.Font.Name = "Helvatica";
               paragraph.Format.Font.Size = 8;
               paragraph.Format.SpaceAfter = 10;
               paragraph.Format.Font.Bold = false;
              */
            paragraph = this.addressFrame.AddParagraph("");
            paragraph.Format.Font.Name = "Helvatica";
            paragraph.Format.Font.Size = 8;
            paragraph.Format.SpaceAfter = 3;
            paragraph.Format.Font.Bold = false;
            double height = (1.0);
            MigraDoc.DocumentObjectModel.Color hrFillColor = new MigraDoc.DocumentObjectModel.Color(255, 0, 0);
            MigraDoc.DocumentObjectModel.Color hrBorderColor = new MigraDoc.DocumentObjectModel.Color(255, 0, 0);

            MigraDoc.DocumentObjectModel.Border newBorder = new MigraDoc.DocumentObjectModel.Border { Style = BorderStyle.Single, Color = hrBorderColor };

            paragraph.Format = new ParagraphFormat
            {
                Font = new Font("Courier New", new Unit(height)),
                Shading = new Shading { Visible = true, Color = hrFillColor },
                Borders = new Borders
                {
                    Bottom = newBorder,
                    Left = newBorder.Clone(),
                    Right = newBorder.Clone(),
                    Top = newBorder.Clone()
                }
            };

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "0.1cm";
            paragraph.Style = "Reference";
            paragraph.AddTab();

            // Create the item table
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Format.Font.Name = "Helvatica";
            // this.table.Borders.Color =  TableBorder;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;



            // Before you can add a row, you must define the columns

            Column column = this.table.AddColumn("0.8cm");//Qty
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("7.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("1.9cm");//Price
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("1.2cm");//Unit
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("1.1cm");//Disc
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn("2.5cm");//Tot
            column.Format.Alignment = ParagraphAlignment.Center;


            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Format.Font.Name = "Calibri";

            row.Cells[0].AddParagraph("Qty");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].Format.Font.Size = 9;
            row.Cells[1].AddParagraph("Product Code");
            row.Cells[1].Format.Font.Size = 9;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[2].AddParagraph("Product Description");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[2].Format.Font.Size = 9;
            row.Cells[3].AddParagraph("List Price");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].Format.Font.Size = 9;
            row.Cells[4].AddParagraph("UM");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[4].Format.Font.Size = 9;
            row.Cells[5].AddParagraph("Disc %");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[5].Format.Font.Size = 9;
            row.Cells[6].AddParagraph("Discounted Total");
            row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[6].Format.Font.Size = 9;

        }

        void FillContent()
        {

            MigraDoc.DocumentObjectModel.Paragraph paragraph = this.addressFrame.AddParagraph();
            int x = 1;
            decimal total = 0;
            decimal listPriceTotal = 0;
            decimal tax = 0;
            decimal finalTot = 0;
            //double freightTotal = 0;
            
            
            MigraDoc.DocumentObjectModel.Color col = MigraDoc.DocumentObjectModel.Colors.AliceBlue;

            for (int i = 0; i < ItemCollection.Count; i++)
            {
                if (IsOdd(i))
                {
                    col = MigraDoc.DocumentObjectModel.Colors.AliceBlue;
                }
                else
                {
                    col = MigraDoc.DocumentObjectModel.Colors.White;
                }

                Row row = this.table.AddRow();
                row.TopPadding = 1.5;
                row.Shading.Color = col;

                row.Cells[0].AddParagraph(ItemCollection[i].Quantity.ToString());
                row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[0].Format.SpaceBefore = 2;
                row.Cells[0].Format.SpaceAfter = 2;
                row.Cells[0].Format.Font.Size = 8;
                row.Cells[0].Borders.Bottom.Visible = false;

                row.Cells[1].AddParagraph(ItemCollection[i].ProductCode);
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].Format.SpaceBefore = 2;
                row.Cells[1].Format.SpaceAfter = 2;
                row.Cells[1].Format.Font.Size = 8;
                row.Cells[1].Borders.Bottom.Visible = false;

                row.Cells[2].AddParagraph(ItemCollection[i].ProductDescription);
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[2].Format.SpaceBefore = 2;
                row.Cells[2].Format.SpaceAfter = 2;
                row.Cells[2].Format.Font.Size = 8;
                row.Cells[2].Borders.Bottom.Visible = false;

                row.Cells[3].AddParagraph("$" + String.Format("{0:n}", ItemCollection[i].ProductPrice));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[3].Format.SpaceBefore = 2;
                row.Cells[3].Format.SpaceAfter = 2;
                row.Cells[3].Format.Font.Size = 8;
                row.Cells[3].Borders.Bottom.Visible = false;

                row.Cells[4].AddParagraph(ItemCollection[i].ProductUnit);
                row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[4].Format.SpaceBefore = 2;
                row.Cells[4].Format.SpaceAfter = 2;
                row.Cells[4].Format.Font.Size = 8;
                row.Cells[4].Borders.Bottom.Visible = false;

                row.Cells[5].AddParagraph(ItemCollection[i].Discount.ToString() + "%");
                row.Cells[5].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[5].Format.SpaceBefore = 2;
                row.Cells[5].Format.SpaceAfter = 2;
                row.Cells[5].Format.Font.Size = 8;
                row.Cells[5].Borders.Bottom.Visible = false;
                                        
                row.Cells[6].AddParagraph("$" + String.Format("{0:n}", ItemCollection[i].Total));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[6].Format.SpaceBefore = 2;
                row.Cells[6].Format.SpaceAfter = 2;
                row.Cells[6].Format.Font.Size = 8;
                row.Cells[6].Borders.Bottom.Visible = false;

                total += Math.Round(ItemCollection[i].Total, 2);
                listPriceTotal += (Math.Round(ItemCollection[i].ProductPrice, 2)) * (Math.Round(ItemCollection[i].Quantity, 2));

                x++;
            }

            tax = Math.Round((total + FreightTotal) * 10 / 100, 2);

            /***************************ADDING FREIGHT***************************/
            if (col == MigraDoc.DocumentObjectModel.Colors.AliceBlue)
            {
                col = MigraDoc.DocumentObjectModel.Colors.White;
            }
            else
            {
                col = MigraDoc.DocumentObjectModel.Colors.AliceBlue;
            }

            Row rowF = this.table.AddRow();
            rowF.TopPadding = 1.5;
            rowF.Shading.Color = col;

            rowF.Cells[0].AddParagraph(FreightQty.ToString());
            rowF.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            rowF.Cells[0].Format.SpaceBefore = 2;
            rowF.Cells[0].Format.SpaceAfter = 2;
            rowF.Cells[0].Format.Font.Size = 8;
            rowF.Cells[0].Borders.Bottom.Visible = false;

            rowF.Cells[1].AddParagraph(FreightName);
            rowF.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            rowF.Cells[1].Format.SpaceBefore = 2;
            rowF.Cells[1].Format.SpaceAfter = 2;
            rowF.Cells[1].Format.Font.Size = 8;
            rowF.Cells[1].Borders.Bottom.Visible = false;

            rowF.Cells[2].AddParagraph(FreightDescription);
            rowF.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            rowF.Cells[2].Format.SpaceBefore = 2;
            rowF.Cells[2].Format.SpaceAfter = 2;
            rowF.Cells[2].Format.Font.Size = 8;
            rowF.Cells[2].Borders.Bottom.Visible = false;

            rowF.Cells[3].AddParagraph("$" + String.Format("{0:n}", FreightPrice));
            rowF.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            rowF.Cells[3].Format.SpaceBefore = 2;
            rowF.Cells[3].Format.SpaceAfter = 2;
            rowF.Cells[3].Format.Font.Size = 8;
            rowF.Cells[3].Borders.Bottom.Visible = false;

            rowF.Cells[4].AddParagraph(FreightUnit);
            rowF.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            rowF.Cells[4].Format.SpaceBefore = 2;
            rowF.Cells[4].Format.SpaceAfter = 2;
            rowF.Cells[4].Format.Font.Size = 8;
            rowF.Cells[4].Borders.Bottom.Visible = false;

            rowF.Cells[5].AddParagraph(FreightDisc + "%");
            rowF.Cells[5].Format.Alignment = ParagraphAlignment.Right;
            rowF.Cells[5].Format.SpaceBefore = 2;
            rowF.Cells[5].Format.SpaceAfter = 2;
            rowF.Cells[5].Format.Font.Size = 8;
            rowF.Cells[5].Borders.Bottom.Visible = false;

            rowF.Cells[6].AddParagraph("$" + String.Format("{0:n}", FreightTotal));
            rowF.Cells[6].Format.Alignment = ParagraphAlignment.Right;
            rowF.Cells[6].Format.SpaceBefore = 2;
            rowF.Cells[6].Format.SpaceAfter = 2;
            rowF.Cells[6].Format.Font.Size = 8;
            rowF.Cells[6].Borders.Bottom.Visible = false;

            /**********************END OF ADDING FREIGHT************************/
            int y = 20 - x;

            for (int i = 0; i < y; i++)
            {
                Row row = this.table.AddRow();
                row.TopPadding = 1.5;

                row.Cells[0].AddParagraph("");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].Format.SpaceBefore = 2;
                row.Cells[0].Format.SpaceAfter = 2;
                row.Cells[0].Format.Font.Size = 8;
                row.Cells[0].MergeDown = y;
                row.Cells[1].AddParagraph("");
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].Format.SpaceBefore = 2;
                row.Cells[1].Format.SpaceAfter = 2;
                row.Cells[1].Format.Font.Size = 8;
                row.Cells[1].MergeDown = y;
                row.Cells[2].AddParagraph("");
                row.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[2].Format.SpaceBefore = 2;
                row.Cells[2].Format.SpaceAfter = 2;
                row.Cells[2].Format.Font.Size = 8;
                row.Cells[2].MergeDown = y;
                row.Cells[3].AddParagraph("");
                row.Cells[3].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[3].Format.SpaceBefore = 2;
                row.Cells[3].Format.SpaceAfter = 2;
                row.Cells[3].Format.Font.Size = 8;
                row.Cells[3].MergeDown = y;
                row.Cells[4].AddParagraph("");
                row.Cells[4].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[4].Format.SpaceBefore = 2;
                row.Cells[4].Format.SpaceAfter = 2;
                row.Cells[4].Format.Font.Size = 8;
                row.Cells[4].MergeDown = y;
                row.Cells[5].AddParagraph("");
                row.Cells[5].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[5].Format.SpaceBefore = 2;
                row.Cells[5].Format.SpaceAfter = 2;
                row.Cells[5].Format.Font.Size = 8;
                row.Cells[5].MergeDown = y;
                row.Cells[6].AddParagraph("");
                row.Cells[6].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[6].Format.SpaceBefore = 2;
                row.Cells[6].Format.SpaceAfter = 2;
                row.Cells[6].Format.Font.Size = 8;
                row.Cells[6].Borders.Bottom.Visible = false;

                x++;
            }


            finalTot = Math.Round(total, 2) + Math.Round(tax, 2) + Math.Round(FreightTotal, 2);

            // Add an invisible row as a space line to the table
            Row rowListPrice = this.table.AddRow();
            rowListPrice.Borders.Visible = false;
            rowListPrice.TopPadding = "0.6cm";

            // Add Quote text
            rowListPrice = this.table.AddRow();
            rowListPrice.Cells[0].Borders.Visible = true;
            rowListPrice.Cells[0].Format.Font.Size = 10;
            rowListPrice.Cells[0].AddParagraph("Quoted By: " + RepName);
            rowListPrice.Cells[0].Format.Font.Bold = false;
            rowListPrice.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowListPrice.Cells[0].MergeRight = 6;

            rowListPrice = this.table.AddRow();
            rowListPrice.Cells[0].AddParagraph("Comments :" + Instructions);
            rowListPrice.Cells[0].Format.Font.Name = "Calibri";
            rowListPrice.Cells[0].Format.Font.Size = 10;
            rowListPrice.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowListPrice.Cells[0].MergeRight = 2;
            rowListPrice.Cells[0].Format.SpaceBefore = 2;
            rowListPrice.Cells[0].Format.SpaceAfter = 2;
            rowListPrice.Cells[0].Borders.Visible = true;

            rowListPrice.Cells[0].Format.Font.Bold = false;
            rowListPrice.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowListPrice.Cells[0].MergeRight = 2;
            rowListPrice.Cells[0].MergeDown = 2;
            rowListPrice.Cells[3].Borders.Visible = false;

            // Add the list price row
            rowListPrice.Cells[3].Borders.Visible = true;
            rowListPrice.Cells[3].AddParagraph("List Price Total");
            rowListPrice.Cells[3].Format.Font.Name = "Calibri";
            rowListPrice.Cells[3].Format.Font.Size = 12;
            rowListPrice.Cells[3].Format.Font.Bold = true;
            rowListPrice.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[3].MergeRight = 2;

            rowListPrice.Cells[6].AddParagraph("$" + String.Format("{0:n}", listPriceTotal));
            rowListPrice.Cells[6].Format.Font.Size = 9;
            rowListPrice.Cells[6].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[6].Format.SpaceBefore = 2;
            rowListPrice.Cells[6].Format.SpaceAfter = 2;

            //Discounted Total
            rowListPrice = this.table.AddRow();
            rowListPrice.Cells[0].Borders.Visible = true;
            rowListPrice.Cells[0].Format.Font.Size = 8;
            rowListPrice.Cells[0].AddParagraph();
            rowListPrice.Cells[0].Format.Font.Bold = false;
            rowListPrice.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowListPrice.Cells[0].MergeRight = 2;
            rowListPrice.Cells[3].Borders.Visible = false;

            rowListPrice.Cells[3].Borders.Visible = true;
            rowListPrice.Cells[3].AddParagraph("Discounted Total");
            rowListPrice.Cells[3].Format.Font.Name = "Calibri";
            rowListPrice.Cells[3].Format.Font.Size = 12;
            rowListPrice.Cells[3].Format.Font.Bold = true;
            rowListPrice.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[3].MergeRight = 2;

            rowListPrice.Cells[6].Format.Font.Size = 9;
            rowListPrice.Cells[6].AddParagraph("$" + String.Format("{0:n}", total));
            rowListPrice.Cells[6].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[6].Format.SpaceBefore = 2;
            rowListPrice.Cells[6].Format.SpaceAfter = 2;

            //Freight
            rowListPrice = this.table.AddRow();
            rowListPrice.Cells[0].Borders.Visible = true;
            rowListPrice.Cells[0].Format.Font.Size = 8;
            rowListPrice.Cells[0].AddParagraph();
            rowListPrice.Cells[0].Format.Font.Bold = false;
            rowListPrice.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowListPrice.Cells[0].MergeRight = 2;

            rowListPrice.Cells[3].Borders.Visible = true;
            rowListPrice.Cells[3].AddParagraph("Freight");
            rowListPrice.Cells[3].Format.Font.Name = "Calibri";
            rowListPrice.Cells[3].Format.Font.Size = 12;
            rowListPrice.Cells[3].Format.Font.Bold = true;
            rowListPrice.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[3].MergeRight = 2;

            rowListPrice.Cells[6].Format.Font.Size = 9;
            rowListPrice.Cells[6].AddParagraph("$" + String.Format("{0:n}", FreightTotal));
            rowListPrice.Cells[6].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[6].Format.SpaceBefore = 2;
            rowListPrice.Cells[6].Format.SpaceAfter = 2;

            //GST
            rowListPrice = this.table.AddRow();
            rowListPrice.Cells[0].Borders.Visible = true;
            rowListPrice.Cells[0].Format.Font.Size = 8;
            rowListPrice.Cells[0].AddParagraph("This Quote is valid for 30 days only and cannot be used as an invoice. Should you wish to proceed, please send an acceptance of this quote and you will be sent an invoice.");
            rowListPrice.Cells[0].Format.Font.Bold = false;
            rowListPrice.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowListPrice.Cells[0].MergeRight = 2;
            rowListPrice.Cells[0].MergeDown = 1;

            rowListPrice.Cells[3].Borders.Visible = true;
            rowListPrice.Cells[3].AddParagraph("GST");
            rowListPrice.Cells[3].Format.Font.Name = "Calibri";
            rowListPrice.Cells[3].Format.Font.Size = 12;
            rowListPrice.Cells[3].Format.Font.Bold = true;
            rowListPrice.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[3].MergeRight = 2;

            rowListPrice.Cells[6].Format.Font.Size = 9;
            rowListPrice.Cells[6].AddParagraph("$" + String.Format("{0:n}", tax));
            rowListPrice.Cells[6].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[6].Format.SpaceBefore = 2;
            rowListPrice.Cells[6].Format.SpaceAfter = 2;

            // Add the sub total row
            rowListPrice = this.table.AddRow();
            rowListPrice.Cells[0].Borders.Visible = true;
            rowListPrice.Cells[0].Format.Font.Size = 9;
            rowListPrice.Cells[0].AddParagraph();
            rowListPrice.Cells[0].Format.Font.Bold = false;
            rowListPrice.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            rowListPrice.Cells[0].MergeRight = 2;

            rowListPrice.Cells[3].Borders.Visible = true;
            rowListPrice.Cells[3].AddParagraph("Total Amount");
            rowListPrice.Cells[3].Format.Font.Name = "Calibri";
            rowListPrice.Cells[3].Format.Font.Bold = true;
            rowListPrice.Cells[3].Format.Font.Size = 16;
            rowListPrice.Cells[3].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[3].MergeRight = 2;

            rowListPrice.Cells[6].AddParagraph("$" + String.Format("{0:n}", finalTot));
            rowListPrice.Cells[6].Format.Alignment = ParagraphAlignment.Right;
            rowListPrice.Cells[6].Format.Font.Size = 10;
            rowListPrice.Cells[6].Format.Font.Bold = true;
            rowListPrice.Cells[6].Format.SpaceBefore = 2;
            rowListPrice.Cells[6].Format.SpaceAfter = 2;

        }

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        
    }
}
