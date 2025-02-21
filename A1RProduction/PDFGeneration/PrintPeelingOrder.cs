using A1QSystem.Model.Production.SlitingPeeling;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.PDFGeneration
{
    public class PrintPeelingOrder
    {
        Document document;
        private PeelingProductionDetails peelingProductionDetails;

        public PrintPeelingOrder(PeelingProductionDetails ppd)
        {
            peelingProductionDetails = ppd;

            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.TopMargin = "7.5cm";
            document.DefaultPageSetup.FooterDistance = "-2.5cm";
            document.DefaultPageSetup.LeftMargin = "0.8cm";
            document.DefaultPageSetup.RightMargin = "1cm";

            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string filename = "CurrentPeel.pdf";
            string fullPath = @"C:\Orders\CurrentPeel.pdf";
            bool exe = false;
            using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        try
                        {
                            fileStream.Dispose();
                            fileStream.Close(); 
                            pdfRenderer.PdfDocument.Save(fullPath);
                            exe = true;
                            //Process.Start(fullPath);              
              
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            exe = false;
                            Msg.Show("You dont have access to this file", "Permision Denied", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                        catch (FileNotFoundException ex)
                        {
                            exe = false;
                            Msg.Show("File not found.", "File Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                        catch (IOException ex)
                        {
                            int hresult = Marshal.GetHRForException(ex);

                            switch (hresult & 0x0000FFFF)
                            {
                                case 32:
                                    exe = false;
                                    Msg.Show("The file is currently opened. Please close the file and try again." + ex.ToString(), "File is in use", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                                    break;
                            }
                        }
                        finally
                        {
                            fileStream.Dispose();
                            fileStream.Close(); 
                            if (exe)
                            {
                                ProcessStartInfo info = new ProcessStartInfo(fullPath);
                                info.Verb = "Print";
                                info.CreateNoWindow = true;
                                info.WindowStyle = ProcessWindowStyle.Hidden;
                                Process.Start(info);
                            }
                            
                            
                        }
                    }
                }
        

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "A1 Rubber Product Formula";
            this.document.Info.Subject = "A1 Rubber Product Formula";
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

            int coatingRow = 0;//Row number

            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Alignment = ParagraphAlignment.Left;
            coatingTable.Rows[coatingRow].Borders.Left.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Right.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Top.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Bottom.Width = 0;

            coatingTable[coatingRow, 0].AddParagraph("Sale Order No - " + peelingProductionDetails.SalesOrder);
            coatingTable[coatingRow, 0].MergeRight = 2;
            coatingTable[coatingRow, 0].Format.Alignment = ParagraphAlignment.Right;
            coatingTable[coatingRow, 0].Format.Font.Name = "Calibri";
            coatingTable[coatingRow, 0].Format.Font.Size = 22;
            coatingTable[coatingRow, 0].Format.Font.Bold = true;


            coatingTable.AddRow();
            coatingTable[1, 0].AddParagraph();
            coatingTable[1, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[1, 0].MergeRight = 2;
            coatingTable[1, 0].Borders.Top.Width = 0;
            coatingTable[1, 0].Borders.Bottom.Width = 0;
            coatingTable[1, 0].Borders.Left.Width = 0;
            coatingTable[1, 2].Borders.Right.Width = 0;
            coatingTable[1, 0].Format.Font.Size = 30;

            coatingTable.AddRow();
            coatingTable[2, 0].AddParagraph("PEELING ORDER");
            coatingTable[2, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[2, 0].MergeRight = 2;
            coatingTable[2, 0].Borders.Top.Width = 0;
            coatingTable[2, 0].Borders.Bottom.Width = 0;
            coatingTable[2, 0].Borders.Left.Width = 0;
            coatingTable[2, 2].Borders.Right.Width = 0;
            coatingTable[2, 0].Format.Font.Size = 36;

            coatingTable.AddRow();
            coatingTable[3, 0].AddParagraph();
            coatingTable[3, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[3, 0].MergeRight = 2;
            coatingTable[3, 0].Borders.Top.Width = 0;
            coatingTable[3, 0].Borders.Bottom.Width = 0;
            coatingTable[3, 0].Borders.Left.Width = 0;
            coatingTable[3, 2].Borders.Right.Width = 0;
            coatingTable[3, 0].Format.Font.Size = 30;

            coatingTable.AddRow();
            coatingTable[4, 0].AddParagraph("Customer Name - ");
            coatingTable[4, 0].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[4, 0].Format.Font.Name = "Calibri";
            coatingTable[4, 0].Format.Font.Size = 22;
            coatingTable[4, 0].Format.Font.Bold = true;

            coatingTable[4, 1].AddParagraph(peelingProductionDetails.Customer.CompanyName);
            coatingTable[4, 1].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[4, 1].MergeRight = 1;
            coatingTable[4, 1].Format.Font.Name = "Calibri";
            coatingTable[4, 1].Format.Font.Size = 22;
            coatingTable[4, 1].Format.Font.Bold = true;

            coatingTable.AddRow();
            coatingTable[5, 0].AddParagraph();
            coatingTable[5, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[5, 0].MergeRight = 2;
            coatingTable[5, 0].Borders.Top.Width = 0;
            coatingTable[5, 0].Borders.Bottom.Width = 0;
            coatingTable[5, 0].Borders.Left.Width = 0;
            coatingTable[5, 2].Borders.Right.Width = 0;
            coatingTable[5, 0].Format.Font.Size = 15;

            coatingTable.AddRow();
            coatingTable[6, 0].AddParagraph("Product Name");
            coatingTable[6, 0].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[6, 0].Format.Font.Name = "Calibri";
            coatingTable[6, 0].Format.Font.Size = 22;
            coatingTable[6, 0].Format.Font.Bold = true;

            coatingTable[6, 1].AddParagraph(peelingProductionDetails.Product.ProductDescription);
            coatingTable[6, 1].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[6, 1].MergeRight = 1;
            coatingTable[6, 1].Format.Font.Name = "Calibri";
            coatingTable[6, 1].Format.Font.Size = 22;
            coatingTable[6, 1].Format.Font.Bold = true;

            coatingTable.AddRow();
            coatingTable[7, 0].AddParagraph();
            coatingTable[7, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[7, 0].MergeRight = 2;
            coatingTable[7, 0].Borders.Top.Width = 0;
            coatingTable[7, 0].Borders.Bottom.Width = 0;
            coatingTable[7, 0].Borders.Left.Width = 0;
            coatingTable[7, 2].Borders.Right.Width = 0;
            coatingTable[7, 0].Format.Font.Size = 15;

            coatingTable.AddRow();
            coatingTable[8, 0].AddParagraph("Thickness");
            coatingTable[8, 0].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[8, 0].Format.Font.Name = "Calibri";
            coatingTable[8, 0].Format.Font.Size = 22;
            coatingTable[8, 0].Format.Font.Bold = true;

            coatingTable[8, 1].AddParagraph(peelingProductionDetails.Product.Tile.Thickness.ToString() + "mm");
            coatingTable[8, 1].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[8, 1].MergeRight = 1;
            coatingTable[8, 1].Format.Font.Name = "Calibri";
            coatingTable[8, 1].Format.Font.Size = 22;
            coatingTable[8, 1].Format.Font.Bold = true;

            coatingTable.AddRow();
            coatingTable[9, 0].AddParagraph();
            coatingTable[9, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[9, 0].MergeRight = 2;
            coatingTable[9, 0].Borders.Top.Width = 0;
            coatingTable[9, 0].Borders.Bottom.Width = 0;
            coatingTable[9, 0].Borders.Left.Width = 0;
            coatingTable[9, 2].Borders.Right.Width = 0;
            coatingTable[9, 0].Format.Font.Size = 15;

            coatingTable.AddRow();
            coatingTable[10, 0].AddParagraph("Quantity");
            coatingTable[10, 0].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[10, 0].Format.Font.Name = "Calibri";
            coatingTable[10, 0].Format.Font.Size = 22;
            coatingTable[10, 0].Format.Font.Bold = true;

            coatingTable[10, 1].AddParagraph(peelingProductionDetails.SlitPeelProduction.BlockLogQty.ToString() + " " + peelingProductionDetails.Product.ProductUnit);
            coatingTable[10, 1].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[10, 1].MergeRight = 1;
            coatingTable[10, 1].Format.Font.Name = "Calibri";
            coatingTable[10, 1].Format.Font.Size = 22;
            coatingTable[10, 1].Format.Font.Bold = true;

            coatingTable.AddRow();
            coatingTable[11, 0].AddParagraph();
            coatingTable[11, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[11, 0].MergeRight = 2;
            coatingTable[11, 0].Borders.Top.Width = 0;
            coatingTable[11, 0].Borders.Bottom.Width = 0;
            coatingTable[11, 0].Borders.Left.Width = 0;
            coatingTable[11, 2].Borders.Right.Width = 0;
            coatingTable[11, 0].Format.Font.Size = 50;

            coatingTable.AddRow();
            coatingTable[12, 0].AddParagraph();
            coatingTable[12, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[12, 0].MergeRight = 2;
            coatingTable[12, 0].Borders.Top.Width = 0;
            coatingTable[12, 0].Borders.Bottom.Width = 0;
            coatingTable[12, 0].Borders.Left.Width = 0;
            coatingTable[12, 2].Borders.Right.Width = 0;
            coatingTable[12, 0].Format.Font.Size = 50;


            coatingTable.AddRow();
            coatingTable[13, 0].AddParagraph("Date Started");
            coatingTable[13, 0].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[13, 0].Format.Font.Name = "Calibri";
            coatingTable[13, 0].Format.Font.Size = 22;
            coatingTable[13, 0].Format.Font.Bold = true;
            coatingTable[13, 0].Borders.Top.Width = 0;
            coatingTable[13, 0].Borders.Bottom.Width = 0;
            coatingTable[13, 0].Borders.Left.Width = 0;
            coatingTable[13, 0].Borders.Right.Width = 0;

            coatingTable[13, 1].AddParagraph(peelingProductionDetails.SlittingStartTime.ToString());
            coatingTable[13, 1].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[13, 1].MergeRight = 1;
            coatingTable[13, 1].Format.Font.Name = "Calibri";
            coatingTable[13, 1].Format.Font.Size = 22;
            coatingTable[13, 1].Format.Font.Bold = true;
            coatingTable[13, 1].Borders.Top.Width = 0;
            coatingTable[13, 1].Borders.Bottom.Width = 0;
            coatingTable[13, 1].Borders.Left.Width = 0;
            coatingTable[13, 2].Borders.Right.Width = 0;

            coatingTable.AddRow();
            coatingTable[14, 0].AddParagraph();
            coatingTable[14, 0].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[14, 0].MergeRight = 2;
            coatingTable[14, 0].Borders.Top.Width = 0;
            coatingTable[14, 0].Borders.Bottom.Width = 0;
            coatingTable[14, 0].Borders.Left.Width = 0;
            coatingTable[14, 2].Borders.Right.Width = 0;
            coatingTable[14, 0].Format.Font.Size = 15;


            coatingTable.AddRow();
            coatingTable[15, 0].AddParagraph("Date Finished");
            coatingTable[15, 0].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[15, 0].Format.Font.Name = "Calibri";
            coatingTable[15, 0].Format.Font.Size = 22;
            coatingTable[15, 0].Format.Font.Bold = true;
            coatingTable[15, 0].Borders.Top.Width = 0;
            coatingTable[15, 0].Borders.Bottom.Width = 0;
            coatingTable[15, 0].Borders.Left.Width = 0;
            coatingTable[15, 0].Borders.Right.Width = 0;

            coatingTable[15, 1].AddParagraph("");
            coatingTable[15, 1].Format.Alignment = ParagraphAlignment.Left;
            coatingTable[15, 1].MergeRight = 1;
            coatingTable[15, 1].Format.Font.Name = "Calibri";
            coatingTable[15, 1].Format.Font.Size = 22;
            coatingTable[15, 1].Format.Font.Bold = true;



        }
    }
}
