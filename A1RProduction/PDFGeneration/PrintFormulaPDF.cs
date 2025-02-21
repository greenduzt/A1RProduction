using A1QSystem.Model;
using A1QSystem.Model.FormulaGeneration;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MsgBox;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace A1QSystem.PDFGeneration
{
   
    
    public class PrintFormulaPDF
    {
        Document document;

        private string productName;
        private string mouldType;
        private int noOfBins;
        private int mesh4;
        private int mesh12;
        private int mesh16;
        private int mesh16to20;
        private int mesh30;
        private int mesh3040;
        private int mesh12mg;
        private int meshRegrind;
        private int binder;
        private string binderType;
        private int mixingMinutes;
        private List<MeshTypes> list;
        private BindingList<ProductColourDetails> colours;
        private ObservableCollection<FormulaColourTableHeaders> formulaColourTable;
        private string specialInstructions;
        private string colourInstructions;
        private string methodPS;
        private string headerColours;
        private decimal headerFontSize;
        private decimal specialInsHeight;
        private decimal topicFontSize;
        private decimal specialInsTextPosHeight;
        private bool enable;
        private string lift1;
        private string lift2;
        private string mixingNotes;
        private string formulaType;

        public PrintFormulaPDF()
        {

        }

        public bool CreateFormula(BindingList<ProductColourDetails> Colours, ObservableCollection<FormulaColourTableHeaders> FormulaColourTable, int Binder, string BinderType, string SelectedProductName, int MixingMinutes, string MouldType, int NoOfBins, int Mesh4, int Mesh12, int Mesh16,
                                  int Mesh16To20, int Mesh30, int Mesh3040, int Mesh12mg, int MeshRegrind, string SpecialInstructions, string ColourInstructions, string MethodPS, string HeaderColours,
                                  decimal HeaderFontSize, decimal TopicFontSize, decimal SpecialInsHeight, decimal SpecialInsTextPosHeight, bool Enable, string Lift1, string Lift2, string MixingNotes, string FormulaType)
        {
            
            bool result = false;
            productName = SelectedProductName;
            mouldType = MouldType;
            noOfBins = NoOfBins;
            mesh4 = Mesh4;
            mesh12 = Mesh12;
            mesh16 = Mesh16;
            mesh16to20 = Mesh16To20;
            mesh30 = Mesh30;
            mesh3040=Mesh3040;
            mesh12mg = Mesh12mg;
            meshRegrind = MeshRegrind;
            colours = Colours;
            binder = Binder;
            binderType = BinderType;
            mixingMinutes = MixingMinutes;
            list =new List<MeshTypes>();
            specialInstructions = SpecialInstructions;
            colourInstructions = ColourInstructions;
            methodPS=MethodPS;
            headerColours = HeaderColours;
            headerFontSize = HeaderFontSize;
            topicFontSize = TopicFontSize;
            specialInsHeight = SpecialInsHeight;
            specialInsTextPosHeight = SpecialInsTextPosHeight;
            enable = Enable;
            lift1 = Lift1;
            lift2 = Lift2;
            mixingNotes = MixingNotes;
            formulaType = FormulaType;
            formulaColourTable = FormulaColourTable;

            if (Mesh16 != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "16 Mesh";
                mt.MeshValue = Mesh16;
                list.Add(mt);
            }           
            if (Mesh12 != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "12 Mesh";
                mt.MeshValue = Mesh12;
                list.Add(mt);
            }
            if (Mesh4 != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "4 Mesh";
                mt.MeshValue = Mesh4;
                list.Add(mt);
            }           
            
            if (Mesh30 != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "30 Mesh";
                mt.MeshValue = Mesh30;
                list.Add(mt);
            }
            if (Mesh16To20 != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "16/20 Mesh";
                mt.MeshValue = Mesh16To20;
                list.Add(mt);
            }
            if (Mesh3040 != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "30/40 Mesh";
                mt.MeshValue = Mesh3040;
                list.Add(mt);
            }
            
            if (Mesh12mg != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "12mg";
                mt.MeshValue = Mesh12mg;
                list.Add(mt);
            }
            if (MeshRegrind != 0)
            {
                MeshTypes mt = new MeshTypes();
                mt.MeshName = "Regrind";
                mt.MeshValue = MeshRegrind;
                list.Add(mt);
            }

           
            string newFileName1 = Regex.Replace(SelectedProductName, @"[^0-9a-zA-Z]+", "_");
            string newFileName2 = Regex.Replace(MouldType, @"[^0-9a-zA-Z]+", "_");
           
            Document document = CreateDocument();
            document.UseCmykColor = true;
            document.DefaultPageSetup.TopMargin = "1.5cm";
            document.DefaultPageSetup.FooterDistance = "-2.5cm";
            document.DefaultPageSetup.LeftMargin = "0.8cm";
            document.DefaultPageSetup.RightMargin = "1cm";
          
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            
            string filename = newFileName1 + " " + newFileName2 + ".pdf";
            string fullPath = "I:/NEW DOCUMENTS/CHAMARA/Formulas/" + filename + ".pdf"; 

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    pdfRenderer.PdfDocument.Save(fullPath);
                    Process.Start(fullPath);

                    result = true;
                }
            }
            catch (IOException ex)
            {
                result = false;
                Msg.Show("The file is currently opened. Please close the file and try again.", "File is in use", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.Delete(Path.Combine("I:/NEW DOCUMENTS/CHAMARA/Formulas/", filename));


            return result;
        }

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "A1 Rubber Product Formula";
            this.document.Info.Subject = "A1 Rubber Product Formula";
            this.document.Info.Author = "A1 Rubber";
            this.document.DefaultPageSetup.Orientation = Orientation.Landscape;


            DefineStyles();

            if (formulaType == "BlockLog")
            {
                CreateBlockLogPage();
            }
            else if (formulaType == "GranuleCoating")
            {
                CreateGranuleCoatingPage();
            }
            
          
            return this.document;
        }

        void DefineStyles()
        {
            MigraDoc.DocumentObjectModel.Style style = this.document.Styles["Normal"];
            style.Font.Name = "Helvatica";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Helvatica";
            style.Font.Size = 9;

            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        void CreateGranuleCoatingPage()
        {
            Section section = this.document.AddSection();
            Paragraph paragraph = section.AddParagraph();
            
            Table coatingTable = new Table();
            coatingTable = section.AddTable();
            coatingTable.Style = "Table";
            coatingTable.Borders.Color = Color.FromCmyk(0, 0, 0, 100);
            coatingTable.Borders.Width = 0.25;
            coatingTable.Borders.Left.Width = 0.5;
            coatingTable.Borders.Right.Width = 0.5;
            coatingTable.Rows.LeftIndent = 0;


            Column column = coatingTable.AddColumn("5.75cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("2.6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("3.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("1.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("3.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = coatingTable.AddColumn("1.75cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            int coatingRow = 0;//Row number

            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Alignment = ParagraphAlignment.Left;
            coatingTable.Rows[coatingRow].Borders.Left.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Right.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Top.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Bottom.Width = 0;
            
            coatingTable[coatingRow, 0].AddParagraph("Always follow the Quality Assurance Check Lists");
            coatingTable[coatingRow, 0].MergeRight = 4;
            coatingTable[coatingRow, 0].Format.Font.Name = "Calibri";
            coatingTable[coatingRow, 0].Format.Font.Size = 24;
            coatingTable[coatingRow, 0].Format.Font.Bold = true;
            coatingTable[coatingRow, 0].Format.Shading.Color = Colors.WhiteSmoke;
            coatingTable[coatingRow, 5].AddParagraph(headerColours);
            coatingTable[coatingRow, 5].MergeRight = 2;
            coatingTable[coatingRow, 5].Format.Font.Name = "Calibri";
            coatingTable[coatingRow, 5].Format.Font.Size = (MigraDoc.DocumentObjectModel.Unit)headerFontSize;
            coatingTable[coatingRow, 5].Format.Alignment = ParagraphAlignment.Center;
            coatingTable[coatingRow, 5].Format.Shading.Color = Colors.LightGray;

            /********************Blank Row***********************/
            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable[coatingRow, 0].AddParagraph();
            coatingTable[coatingRow, 0].Borders.Top.Width = 0;
            coatingTable[coatingRow, 0].Borders.Bottom.Width = 0;
            coatingTable[coatingRow, 0].Borders.Left.Width = 0;
            coatingTable[coatingRow, 7].Borders.Right.Width = 0;
            coatingTable[coatingRow, 0].MergeRight = 7;
            coatingTable[coatingRow, 0].Format.Font.Size = 4;

            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Font.Bold = true;
            coatingTable.Rows[coatingRow].Format.Font.Size = 28;
            coatingTable.Rows[coatingRow].Format.Font.Name = "Calibri";
            coatingTable.Rows[coatingRow].Borders.Left.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Right.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Top.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Bottom.Width = 0;
            coatingTable.Rows[coatingRow].Format.Shading.Color = Colors.Black;
            coatingTable.Rows[coatingRow].Format.Font.Color = Colors.White;

            coatingTable[coatingRow, 0].AddParagraph("MIXING FORMULA").Format.Alignment = ParagraphAlignment.Center;
            coatingTable[coatingRow, 0].MergeRight = 7;


            /********************Blank Row***********************/
            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable[coatingRow, 0].AddParagraph();
            coatingTable[coatingRow, 0].Borders.Top.Width = 0;
            coatingTable[coatingRow, 0].Borders.Bottom.Width = 0;
            coatingTable[coatingRow, 0].Borders.Left.Width = 0;
            coatingTable[coatingRow, 7].Borders.Right.Width = 0;
            coatingTable[coatingRow, 0].MergeRight = 7;
            coatingTable[coatingRow, 0].Format.Font.Size = 20;


            /********************Job Colour**********************/
            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Font.Bold = true;
            coatingTable.Rows[coatingRow].Format.Font.Size = 30;
            coatingTable.Rows[coatingRow].Format.Font.Name = "Calibri";
            coatingTable.Rows[coatingRow].Borders.Left.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Top.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Bottom.Width = 0;
            coatingTable[coatingRow, 0].AddParagraph("JOB COLOUR:").Format.Alignment = ParagraphAlignment.Right;
            coatingTable[coatingRow, 0].MergeRight = 2;

            coatingTable[coatingRow, 3].AddParagraph(productName).Format.Alignment = ParagraphAlignment.Left;
            coatingTable[coatingRow, 3].MergeRight = 4;
            coatingTable[coatingRow, 3].Borders.Left.Width = 1;
            coatingTable[coatingRow, 3].Borders.Right.Width = 1;
            coatingTable[coatingRow, 3].Borders.Top.Width = 1;
            coatingTable[coatingRow, 3].Borders.Bottom.Width = 1;


            /********************Blank Row***********************/
            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable[coatingRow, 0].AddParagraph();
            coatingTable[coatingRow, 0].Borders.Top.Width = 0;
            coatingTable[coatingRow, 0].Borders.Bottom.Width = 0;
            coatingTable[coatingRow, 0].Borders.Left.Width = 0;
            coatingTable[coatingRow, 7].Borders.Right.Width = 0;
            coatingTable[coatingRow, 0].MergeRight = 7;
            coatingTable[coatingRow, 0].Format.Font.Size = 20;

            /*******************Job Colour**********************/

            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Font.Bold = true;
            coatingTable.Rows[coatingRow].Format.Font.Size = 22;
            coatingTable.Rows[coatingRow].Format.Font.Name = "Calibri";
            coatingTable.Rows[coatingRow].Borders.Left.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Right.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Top.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Bottom.Width = 0;
            coatingTable[coatingRow, 0].AddParagraph("Ingredients:").Format.Alignment = ParagraphAlignment.Left;
            coatingTable[coatingRow, 0].MergeRight = 7;

                       
            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Font.Bold = true;
            coatingTable.Rows[coatingRow].Format.Font.Size = 18;
            coatingTable.Rows[coatingRow].Format.Font.Name = "Calibri";

            /*******************Header Names*****************/
            foreach (var x in formulaColourTable)
            {

                coatingTable[coatingRow, 0].AddParagraph(x.HeaderName1).Format.Alignment = ParagraphAlignment.Center;
                coatingTable[coatingRow, 0].MergeRight = 1;

                coatingTable[coatingRow, 2].AddParagraph(x.HeaderName2).Format.Alignment = ParagraphAlignment.Center;
                coatingTable[coatingRow, 2].MergeRight = 1;

                coatingTable[coatingRow, 4].AddParagraph(x.HeaderName3).Format.Alignment = ParagraphAlignment.Center;
                coatingTable[coatingRow, 4].MergeRight = 1;

                coatingTable[coatingRow, 6].AddParagraph(x.HeaderName4).Format.Alignment = ParagraphAlignment.Center;
                coatingTable[coatingRow, 6].MergeRight = 1;
            }
            /*******************Table Details*****************/
            foreach (var x in colours)
            {
                coatingRow = coatingRow + 1;
                coatingTable.AddRow();
                coatingTable.Rows[coatingRow].HeadingFormat = true;
                coatingTable.Rows[coatingRow].Format.Font.Bold = true;
                coatingTable.Rows[coatingRow].Format.Font.Size = 18;
                coatingTable.Rows[coatingRow].Format.Font.Name = "Calibri";

                coatingTable[coatingRow, 0].AddParagraph(x.ColourName).Format.Alignment = ParagraphAlignment.Left;
                coatingTable[coatingRow, 0].MergeRight = 1;

                coatingTable[coatingRow, 2].AddParagraph(x.BagSize1.ToString()).Format.Alignment = ParagraphAlignment.Center;
                coatingTable[coatingRow, 2].MergeRight = 1;

                coatingTable[coatingRow, 4].AddParagraph(x.BagSize2.ToString()).Format.Alignment = ParagraphAlignment.Center;
                coatingTable[coatingRow, 4].MergeRight = 1;

                coatingTable[coatingRow, 6].AddParagraph().Format.Alignment = ParagraphAlignment.Center;
                coatingTable[coatingRow, 6].MergeRight = 1;

            }

            /********************Blank Row***********************/
            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable[coatingRow, 0].AddParagraph();
            coatingTable[coatingRow, 0].Borders.Top.Width = 0;
            coatingTable[coatingRow, 0].Borders.Bottom.Width = 0;
            coatingTable[coatingRow, 0].Borders.Left.Width = 0;
            coatingTable[coatingRow, 7].Borders.Right.Width = 0;
            coatingTable[coatingRow, 0].MergeRight = 7;
            coatingTable[coatingRow, 0].Format.Font.Size = 50;

            coatingRow = coatingRow + 1;
            coatingTable.AddRow();
            coatingTable.Rows[coatingRow].HeadingFormat = true;
            coatingTable.Rows[coatingRow].Format.Font.Bold = true;
            coatingTable.Rows[coatingRow].Format.Font.Size = 100;
            coatingTable.Rows[coatingRow].Format.Font.Name = "Calibri";
            coatingTable.Rows[coatingRow].Borders.Left.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Right.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Top.Width = 0;
            coatingTable.Rows[coatingRow].Borders.Bottom.Width = 0;           
           

            TextFrame txtScaledWeightKG = coatingTable[coatingRow, 2].AddTextFrame();
            txtScaledWeightKG.LineFormat.Width = 1;
            txtScaledWeightKG.Height = "1.5cm";
            txtScaledWeightKG.Width = "10cm";
            txtScaledWeightKG.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            txtScaledWeightKG.RelativeVertical = RelativeVertical.Page;
            paragraph = txtScaledWeightKG.AddParagraph();
            paragraph.AddText(mixingNotes);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Name = "Tahoma";
            paragraph.Format.Font.Size = 30;
            paragraph.Format.Font.Bold = true;

            TextFrame txtDate;
            txtDate = section.AddTextFrame();
            txtDate.LineFormat.Width = 0.5;
            txtDate.Height = "0.8cm";//any number 
            txtDate.Width = "9cm";//any number 
           // txtDate.Left = "1cm";
            txtDate.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            txtDate.Top = "19.6cm";
            txtDate.RelativeVertical = RelativeVertical.Page;
            paragraph = txtDate.AddParagraph();
            paragraph.AddText("DATE _____/____/_____");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Size = 16;

            TextFrame txtQuality;
            txtQuality = section.AddTextFrame();
            txtQuality.LineFormat.Width = 0.5;
            txtQuality.Height = "0.8cm";//any number 
            txtQuality.Width = "18.8cm";//any number 
            txtQuality.Left = "9.1cm";
            txtQuality.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            txtQuality.Top = "19.6cm";
            txtQuality.RelativeVertical = RelativeVertical.Page;
            paragraph = txtQuality.AddParagraph();
            paragraph.AddText("THIS IS AN A1 RUBBER QUALITY ASSURANCE INITIATIVE");
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Size = 19;
            paragraph.Format.Shading.Color = Colors.Black;
            paragraph.Format.Font.Color = Colors.White;

        }

       
        void CreateBlockLogPage()
        {

            string binVal="";
            string gradingHdr = "";
            DateTime now = DateTime.Now;

            if (noOfBins > 0 && noOfBins < 2)
            {
                binVal = " - " + noOfBins + " BIN";
            }
            else if (noOfBins > 1)
            {
                binVal = " - " + noOfBins + " BINS";
            }

            // Each MigraDoc document needs at least one section.
            Section section = this.document.AddSection();

            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100);
            paragraph.Format.Font.Name = "Calibri";
            paragraph.Format.Font.Size = (MigraDoc.DocumentObjectModel.Unit)topicFontSize;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddFormattedText(productName.ToUpper() + " - " + mouldType + binVal, TextFormat.Bold);
            
            
            // Create the HEADER table for the top of every page
            Table table = new Table();
            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Color.FromCmyk(0, 0, 0, 100);
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;
            

            Column column = table.AddColumn("9.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("7.6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2.9cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("8cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row1 = table.AddRow();
            row1.HeadingFormat = true;
            row1.Format.Alignment = ParagraphAlignment.Left;
            row1.Format.Font.Bold = true;
            row1.Format.Font.Size = 14;
            row1.Format.Borders.Distance = "4pt";

            row1.Cells[0].AddParagraph("Always follow the Quality Assurance Check Lists");
            row1.Cells[0].MergeRight = 1;
            row1.Cells[0].Format.Font.Name = "Calibri";
            row1.Cells[0].Format.Font.Size = 24;

            row1.Cells[2].AddParagraph(headerColours);
            row1.Cells[2].MergeRight = 1;
            row1.Cells[2].Format.Font.Name = "Calibri";
            row1.Cells[2].Format.Font.Size = (MigraDoc.DocumentObjectModel.Unit)headerFontSize;
            row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[2].Format.Shading.Color = Colors.LightGray;        
            
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Format.Borders.Distance = "2pt";
            row.Format.Font.Name = "Calibri"; 
            row.Format.Font.Size = 22;
            if (noOfBins > 0)
            {
                binVal = " - " + noOfBins + "BIN" + CheckPlural();
                gradingHdr = "RUBBER GRADING";
                row.Cells[0].AddParagraph(gradingHdr);
                row.Cells[1].AddParagraph("MIXING FORMULA");
                row.Cells[1].MergeRight = 1;
            }
            else
            {
                row.Cells[0].MergeRight = 2;
                row.Cells[0].AddParagraph("MIXING FORMULA");
            }
                        
            row.Cells[3].AddParagraph(methodPS);           

            //Mesh Header
            Row row4 = table.AddRow();
            row4.HeadingFormat = true;
            row4.Format.Alignment = ParagraphAlignment.Left;
            row4.Format.Font.Bold = true;
            row4.Format.Font.Size = 12;
            row4.Format.Font.Name = "Calibri";
            row4.Height = 385;

            string pl = "";
            if (noOfBins > 0){

                if (noOfBins > 0 && noOfBins < 2)
                {
                    pl = "BIN";
                }
                else if (noOfBins > 1)
                {
                    pl = "BINS";
                }

                /*********************Grading Table********************/
                Table gradingTable = table[2, 0].AddTextFrame().AddTable();

                gradingTable.Borders.Width = 0.25;
                gradingTable.AddColumn(75);
                gradingTable.AddColumn(59);
                gradingTable.AddColumn(77);
                gradingTable.AddColumn(58);


                int gradingRow = 0;//Row number

                /********************Blank Row***********************/
                gradingTable.AddRow();
                gradingTable[gradingRow, 0].AddParagraph();
                gradingTable[gradingRow, 0].Borders.Top.Width = 0;
                gradingTable[gradingRow, 0].Borders.Bottom.Width = 0;
                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 3].Borders.Right.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 3;
                gradingTable[gradingRow, 0].Format.Font.Size = 4;


                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable.Rows[gradingRow].HeadingFormat = true;
                gradingTable.Rows[gradingRow].Format.Font.Bold = false;
                gradingTable.Rows[gradingRow].Format.Font.Size = 19;
                gradingTable.Rows[gradingRow].Format.Font.Name = "Calibri";
                gradingTable.Rows[gradingRow].Borders.Width = 0;
                gradingTable.Rows[gradingRow].Borders.Top.Width = 0;

                gradingTable[gradingRow,0].AddParagraph("MAKE " + noOfBins + " " + pl + " OF THE FOLLOWING GRADES:").Format.Alignment = ParagraphAlignment.Left;
                gradingTable[gradingRow,0].Borders.Left.Width = 0;
                gradingTable[gradingRow,0].MergeRight = 3;
                gradingTable[gradingRow,3].Borders.Right.Width = 0;

                /********************Blank Row***********************/
                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable[gradingRow, 0].AddParagraph();
                gradingTable[gradingRow, 0].Borders.Top.Width = 0;
                gradingTable[gradingRow, 0].Borders.Bottom.Width = 0;
                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 3].Borders.Right.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 3;
                gradingTable[gradingRow, 0].Format.Font.Size = 4;

                /******************Meshes*******************/

                for (int x = 0; x < list.Count; x++)
                {

                    gradingRow = gradingRow + 1;
                    gradingTable.AddRow();
                    gradingTable.Rows[gradingRow].HeadingFormat = true;
                    gradingTable.Rows[gradingRow].Format.Font.Bold = false;
                    gradingTable.Rows[gradingRow].Format.Font.Size = 19;
                    gradingTable.Rows[gradingRow].Format.Font.Name = "Calibri";
                    gradingTable.Rows[gradingRow].Borders.Width = 0;
                    gradingTable.Rows[gradingRow].Borders.Top.Width = 0;

                    gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                    gradingTable[gradingRow, 0].MergeRight = 1;

                    //Mesh Header
                    TextFrame MeshHeader;
                    MeshHeader = gradingTable[gradingRow, 0].AddTextFrame();
                    MeshHeader.LineFormat.Width = 0;
                    MeshHeader.Height = "0.5cm";
                    MeshHeader.Width = "4.6cm";
                    MeshHeader.MarginTop = "0.4cm";
                    MeshHeader.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                    MeshHeader.RelativeVertical = RelativeVertical.Page;
                    paragraph = MeshHeader.AddParagraph();
                    paragraph.AddText(list[x].MeshName);
                    paragraph.Format.Alignment = ParagraphAlignment.Right;
                    paragraph.Format.Font.Size = 14;

                    //Mesh Value
                    TextFrame txtFMesh = gradingTable[gradingRow, 2].AddTextFrame();
                    txtFMesh.LineFormat.Width = 1;
                    txtFMesh.Height = "1.6cm";
                    txtFMesh.Width = "2.5cm";
                    txtFMesh.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                    txtFMesh.RelativeVertical = RelativeVertical.Page;
                    paragraph = txtFMesh.AddParagraph();
                    paragraph.AddText(list[x].MeshValue.ToString());
                    paragraph.Format.Alignment = ParagraphAlignment.Right;
                    paragraph.Format.Font.Name = "Tahoma";
                    paragraph.Format.Font.Size = 36;
                    paragraph.Format.Font.Bold = true;

                    //Plus
                    TextFrame Mesh1Plus;
                    Mesh1Plus = gradingTable[gradingRow, 0].AddTextFrame();
                    Mesh1Plus.LineFormat.Width = 0;
                    Mesh1Plus.Height = "0.5cm";//any number 
                    Mesh1Plus.Width = "4.6cm";//any number 
                    Mesh1Plus.MarginTop = "0.6cm";
                    Mesh1Plus.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                    Mesh1Plus.RelativeVertical = RelativeVertical.Page;
                    paragraph = Mesh1Plus.AddParagraph();
                    paragraph.AddText("+");
                    paragraph.Format.Alignment = ParagraphAlignment.Right;
                    paragraph.Format.Font.Size = 32;

                    //KG
                    TextFrame txtKG = gradingTable[gradingRow, 3].AddTextFrame();
                    txtKG.LineFormat.Width = 0;
                    txtKG.Height = "0.7cm";
                    txtKG.Width = "1cm";
                    txtKG.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                    txtKG.RelativeVertical = RelativeVertical.Page;
                    txtKG.MarginTop = "0.9cm";
                    paragraph = txtKG.AddParagraph();
                    paragraph.AddText("KG");
                    paragraph.Format.Alignment = ParagraphAlignment.Left;
                    paragraph.Format.Font.Name = "Tahoma";
                    paragraph.Format.Font.Size = 14;
                    paragraph.Format.Font.Bold = false;

                    /********************Blank Row***********************/
                    gradingRow = gradingRow + 1;
                    gradingTable.AddRow();
                    gradingTable[gradingRow, 0].AddParagraph();
                    gradingTable[gradingRow, 0].Borders.Top.Width = 0;
                    gradingTable[gradingRow, 0].Borders.Bottom.Width = 0;
                    gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                    gradingTable[gradingRow, 3].Borders.Right.Width = 0;
                    gradingTable[gradingRow, 0].MergeRight = 3;
                    gradingTable[gradingRow, 0].Format.Font.Size = 2;
                }

               

                /***********************BIN WEIGHT************************/

                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable.Rows[gradingRow].HeadingFormat = true;
                gradingTable.Rows[gradingRow].Format.Font.Bold = false;
                gradingTable.Rows[gradingRow].Format.Font.Size = 19;
                gradingTable.Rows[gradingRow].Format.Font.Name = "Calibri";
                gradingTable.Rows[gradingRow].Borders.Width = 0;
                gradingTable.Rows[gradingRow].Borders.Top.Width = 0;

                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 1;

                //BinWeight Header
                TextFrame txtFBinWeightHdr;
                txtFBinWeightHdr = gradingTable[gradingRow, 0].AddTextFrame();
                txtFBinWeightHdr.LineFormat.Width = 0;
                txtFBinWeightHdr.Height = "0.5cm";
                txtFBinWeightHdr.Width = "4.6cm";
                txtFBinWeightHdr.MarginTop = "0.4cm";
                txtFBinWeightHdr.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtFBinWeightHdr.RelativeVertical = RelativeVertical.Page;
                paragraph = txtFBinWeightHdr.AddParagraph();
                paragraph.AddText("BIN WEIGHT");
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.Format.Font.Size = 14;

                //Mesh Value
                TextFrame txtFBinWeightVal = gradingTable[gradingRow, 2].AddTextFrame();
                txtFBinWeightVal.LineFormat.Width = 1;
                txtFBinWeightVal.Height = "1.6cm";
                txtFBinWeightVal.Width = "2.5cm";
                txtFBinWeightVal.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                txtFBinWeightVal.RelativeVertical = RelativeVertical.Page;

                //KG
                TextFrame txtFBinWeightKG = gradingTable[gradingRow, 3].AddTextFrame();
                txtFBinWeightKG.LineFormat.Width = 0;
                txtFBinWeightKG.Height = "0.7cm";
                txtFBinWeightKG.Width = "1cm";
                txtFBinWeightKG.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                txtFBinWeightKG.RelativeVertical = RelativeVertical.Page;
                txtFBinWeightKG.MarginTop = "0.9cm";
                paragraph = txtFBinWeightKG.AddParagraph();
                paragraph.AddText("KG");
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.Format.Font.Name = "Tahoma";
                paragraph.Format.Font.Size = 14;
                paragraph.Format.Font.Bold = false;

                //Plus
                TextFrame txtFBinWeightPlus;
                txtFBinWeightPlus = gradingTable[gradingRow, 0].AddTextFrame();
                txtFBinWeightPlus.LineFormat.Width = 0;
                txtFBinWeightPlus.Height = "0.5cm";//any number 
                txtFBinWeightPlus.Width = "4.6cm";//any number 
                txtFBinWeightPlus.MarginTop = "0.6cm";
                txtFBinWeightPlus.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtFBinWeightPlus.RelativeVertical = RelativeVertical.Page;
                paragraph = txtFBinWeightPlus.AddParagraph();
                paragraph.AddText("+");
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.Format.Font.Size = 32;

                /********************Blank Row***********************/
                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable[gradingRow, 0].AddParagraph();
                gradingTable[gradingRow, 0].Borders.Top.Width = 0;
                gradingTable[gradingRow, 0].Borders.Bottom.Width = 0;
                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 3].Borders.Right.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 3;
                gradingTable[gradingRow, 0].Format.Font.Size = 2;

                /******************SCALED WEIGHT**********************/
                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable.Rows[gradingRow].HeadingFormat = true;
                gradingTable.Rows[gradingRow].Format.Font.Bold = false;
                gradingTable.Rows[gradingRow].Format.Font.Size = 19;
                gradingTable.Rows[gradingRow].Format.Font.Name = "Calibri";
                gradingTable.Rows[gradingRow].Borders.Width = 0;
                gradingTable.Rows[gradingRow].Borders.Top.Width = 0;

                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 1;

                //Scaled Weight Header1
                TextFrame txtScaledWeightHdr1;
                txtScaledWeightHdr1 = gradingTable[gradingRow, 0].AddTextFrame();
                txtScaledWeightHdr1.LineFormat.Width = 0;
                txtScaledWeightHdr1.Height = "0.5cm";
                txtScaledWeightHdr1.Width = "4.6cm";
                txtScaledWeightHdr1.MarginTop = "0cm";
                txtScaledWeightHdr1.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtScaledWeightHdr1.RelativeVertical = RelativeVertical.Page;
                paragraph = txtScaledWeightHdr1.AddParagraph();
                paragraph.AddText("SCALED WEIGHT");
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.Format.Font.Size = 14;

                //Scaled Weight Header2
                TextFrame txtScaledWeightHdr2;
                txtScaledWeightHdr2 = gradingTable[gradingRow, 0].AddTextFrame();
                txtScaledWeightHdr2.LineFormat.Width = 0;
                txtScaledWeightHdr2.Height = "0.5cm";
                txtScaledWeightHdr2.Width = "4.6cm";
                txtScaledWeightHdr2.MarginTop = "0.5cm";
                txtScaledWeightHdr2.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtScaledWeightHdr2.RelativeVertical = RelativeVertical.Page;
                paragraph = txtScaledWeightHdr2.AddParagraph();
                paragraph.AddText("INCLUDING BIN");
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.Format.Font.Size = 12;
                paragraph.Format.Font.Underline = Underline.Single;

                //Scaled Value
                TextFrame txtScaledWeightVal = gradingTable[gradingRow, 2].AddTextFrame();
                txtScaledWeightVal.LineFormat.Width = 1;
                txtScaledWeightVal.Height = "1.6cm";
                txtScaledWeightVal.Width = "2.5cm";
                txtScaledWeightVal.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                txtScaledWeightVal.RelativeVertical = RelativeVertical.Page;

                //KG
                TextFrame txtScaledWeightKG = gradingTable[gradingRow, 3].AddTextFrame();
                txtScaledWeightKG.LineFormat.Width = 0;
                txtScaledWeightKG.Height = "0.7cm";
                txtScaledWeightKG.Width = "1cm";
                txtScaledWeightKG.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                txtScaledWeightKG.RelativeVertical = RelativeVertical.Page;
                txtScaledWeightKG.MarginTop = "0.9cm";
                paragraph = txtScaledWeightKG.AddParagraph();
                paragraph.AddText("KG");
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.Format.Font.Name = "Tahoma";
                paragraph.Format.Font.Size = 14;
                paragraph.Format.Font.Bold = false;

                //Equal
                TextFrame txtScaledWeightEqual;
                txtScaledWeightEqual = gradingTable[gradingRow, 0].AddTextFrame();
                txtScaledWeightEqual.LineFormat.Width = 0;
                txtScaledWeightEqual.Height = "0.5cm";//any number 
                txtScaledWeightEqual.Width = "4.6cm";//any number 
                txtScaledWeightEqual.MarginTop = "0.6cm";
                txtScaledWeightEqual.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtScaledWeightEqual.RelativeVertical = RelativeVertical.Page;
                paragraph = txtScaledWeightEqual.AddParagraph();
                paragraph.AddText("=");
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.Format.Font.Size = 32;

                
                /********************Blank Row***********************/
                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable[gradingRow, 0].AddParagraph();
                gradingTable[gradingRow, 0].Borders.Top.Width = 0;
                gradingTable[gradingRow, 0].Borders.Bottom.Width = 0;
                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 3].Borders.Right.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 3;
                gradingTable[gradingRow, 0].Format.Font.Size = 2;

                /***********************Moisture***********************/
                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable.Rows[gradingRow].HeadingFormat = true;
                gradingTable.Rows[gradingRow].Format.Font.Bold = false;
                gradingTable.Rows[gradingRow].Format.Font.Size = 19;
                gradingTable.Rows[gradingRow].Format.Font.Name = "Calibri";
                gradingTable.Rows[gradingRow].Borders.Width = 0;
                gradingTable.Rows[gradingRow].Borders.Top.Width = 0;

                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 1;

                //Moisture Header
                TextFrame txtFMoisture;
                txtFMoisture = gradingTable[gradingRow, 0].AddTextFrame();
                txtFMoisture.LineFormat.Width = 0;
                txtFMoisture.Height = "0.5cm";
                txtFMoisture.Width = "4.6cm";
                txtFMoisture.MarginTop = "0.5cm";
                txtFMoisture.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtFMoisture.RelativeVertical = RelativeVertical.Page;
                paragraph = txtFMoisture.AddParagraph();
                paragraph.AddText("MOISTURE");
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.Format.Font.Size = 14;

                //Moisture Value
                TextFrame txtFMoistureVal = gradingTable[gradingRow, 2].AddTextFrame();
                txtFMoistureVal.LineFormat.Width = 1;
                txtFMoistureVal.Height = "1.6cm";
                txtFMoistureVal.Width = "2.5cm";
                txtFMoistureVal.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                txtFMoistureVal.RelativeVertical = RelativeVertical.Page;

                //Percentage
                TextFrame txtFMoistureKG = gradingTable[gradingRow, 3].AddTextFrame();
                txtFMoistureKG.LineFormat.Width = 0;
                txtFMoistureKG.Height = "0.7cm";
                txtFMoistureKG.Width = "1cm";
                txtFMoistureKG.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                txtFMoistureKG.RelativeVertical = RelativeVertical.Page;
                txtFMoistureKG.MarginTop = "0.9cm";
                paragraph = txtFMoistureKG.AddParagraph();
                paragraph.AddText("%");
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.Format.Font.Name = "Tahoma";
                paragraph.Format.Font.Size = 14;
                paragraph.Format.Font.Bold = false;

                /********************Blank Row***********************/
                gradingRow = gradingRow + 1;
                gradingTable.AddRow();
                gradingTable[gradingRow, 0].AddParagraph();
                gradingTable[gradingRow, 0].Borders.Top.Width = 0;
                gradingTable[gradingRow, 0].Borders.Bottom.Width = 0;
                gradingTable[gradingRow, 0].Borders.Left.Width = 0;
                gradingTable[gradingRow, 3].Borders.Right.Width = 0;
                gradingTable[gradingRow, 0].MergeRight = 3;
                gradingTable[gradingRow, 0].Format.Font.Size = 2;

                Row rQuality1 = gradingTable.AddRow();
                rQuality1.HeadingFormat = true;
                rQuality1.Format.Font.Bold = false;
                rQuality1.Format.Font.Size = 12;
                rQuality1.Format.Font.Name = "Calibri";
                rQuality1.Borders.Width = 0;
                rQuality1.Borders.Top.Width = 0.5;

                rQuality1.Cells[0].AddParagraph("Did you notice anything abnormal about the rubber quality? Please state").Format.Alignment = ParagraphAlignment.Left;
                rQuality1.Cells[0].Borders.Left.Width = 0;
                rQuality1.Cells[0].MergeRight = 3;

                //Checked off by
                TextFrame CheckedOffBy;
                CheckedOffBy = section.AddTextFrame();
                CheckedOffBy.LineFormat.Width = 0.5;
                CheckedOffBy.Height = "0.5cm";//any number 
                CheckedOffBy.Width = "9.5cm";//any number 
                CheckedOffBy.Left = "0.01cm";
                CheckedOffBy.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                CheckedOffBy.Top = "19.05cm";
                CheckedOffBy.RelativeVertical = RelativeVertical.Page;
                paragraph = CheckedOffBy.AddParagraph();
                paragraph.AddText("CHECKED OFF BY________________(FIRST NAME)");
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.Format.Font.Size = 11;

                //DateTIme1
                TextFrame DateTIme1;
                DateTIme1 = section.AddTextFrame();
                DateTIme1.LineFormat.Width = 0.5;
                DateTIme1.Height = "0.5cm";//any number 
                DateTIme1.Width = "9.5cm";//any number 
                DateTIme1.Left = "0.01cm";
                DateTIme1.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                DateTIme1.Top = "19.6cm";
                DateTIme1.RelativeVertical = RelativeVertical.Page;
                paragraph = DateTIme1.AddParagraph();
                paragraph.AddText("DATE GRADED _____/____/_____             TIME:____:____");
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.Format.Font.Size = 11;
            }
            else
            {
                /*********************No Grading Table********************/
                Table gradingTable = table[2, 0].AddTextFrame().AddTable();

                gradingTable.Borders.Width = 0.25;
                gradingTable.AddColumn(75);
                gradingTable.AddColumn(59);
                gradingTable.AddColumn(77);
                gradingTable.AddColumn(58);

                Row gTR1 = gradingTable.AddRow();
                gTR1.HeadingFormat = true;
                gTR1.Format.Font.Bold = false;
                gTR1.Format.Font.Size = 19;
                gTR1.Format.Font.Name = "Calibri";
                gTR1.Borders.Width = 0;
                gTR1.Borders.Top.Width = 0;


                /********************Special Instructions Row**********************/
                Row rS1 = gradingTable.AddRow();
                rS1.HeadingFormat = true;
                rS1.Format.Font.Bold = false;
                rS1.Format.Font.Size = 19;
                rS1.Format.Font.Name = "Calibri";
                rS1.Borders.Width = 2;
                rS1.Borders.Bottom.Width = 0;
                rS1.Borders.Style = BorderStyle.DashSmallGap;
                gradingTable[1, 0].MergeRight = 3;
                               
                //Special Instructions Header
                TextFrame txtFSpecialInsHdr;
                txtFSpecialInsHdr = gradingTable[1, 0].AddTextFrame();
                txtFSpecialInsHdr.LineFormat.Width = 0;
                txtFSpecialInsHdr.Height = (MigraDoc.DocumentObjectModel.Unit)specialInsHeight.ToString() + "cm";
                txtFSpecialInsHdr.Width = "9.3cm";
                txtFSpecialInsHdr.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtFSpecialInsHdr.RelativeVertical = RelativeVertical.Page;
                paragraph = txtFSpecialInsHdr.AddParagraph();
                paragraph.AddText("SPECIAL INSTRUCTIONS");
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.Format.Font.Name = "Calibri";
                paragraph.Format.Font.Size = 19;
                paragraph.Format.Font.Bold = true;
                paragraph.Format.Font.Underline = Underline.Single;

                Row rS2 = gradingTable.AddRow();
                rS2.HeadingFormat = true;
                rS2.Format.Font.Bold = false;
                rS2.Format.Font.Size = 19;
                rS2.Format.Font.Name = "Calibri";
                rS2.Borders.Width = 2;
                rS2.Borders.Top.Width = 0;
                rS2.Borders.Style = BorderStyle.DashSmallGap;
                gradingTable[2, 0].MergeRight = 3;
               
                //Special Instructions Information
                TextFrame txtFSpecialInfo;
                txtFSpecialInfo = gradingTable[2, 0].AddTextFrame();
                txtFSpecialInfo.LineFormat.Width = 0;
                txtFSpecialInfo.Height = (MigraDoc.DocumentObjectModel.Unit)specialInsTextPosHeight.ToString() + "cm";
                txtFSpecialInfo.Width = "9.3cm";
                txtFSpecialInfo.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtFSpecialInfo.RelativeVertical = RelativeVertical.Page;
                paragraph = txtFSpecialInfo.AddParagraph();
                paragraph.AddText(specialInstructions);
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.Format.Font.Size = 12;
                paragraph.Format.Font.Bold = true;

                gradingTable.AddRow();
                gradingTable[3, 0].AddParagraph();
                gradingTable[3, 0].Borders.Top.Width = 0;
                gradingTable[3, 0].Borders.Bottom.Width = 0;
                gradingTable[3, 0].Borders.Left.Width = 0;
                gradingTable[3, 3].Borders.Right.Width = 0;
                gradingTable[3, 0].MergeRight = 3;
                gradingTable.AddRow();
                gradingTable[4, 0].AddParagraph();
                gradingTable[4, 0].Borders.Top.Width = 0;
                gradingTable[4, 0].Borders.Bottom.Width = 0;
                gradingTable[4, 0].Borders.Left.Width = 0;
                gradingTable[4, 3].Borders.Right.Width = 0;
                gradingTable[4, 0].MergeRight = 3;

                Row rS3 = gradingTable.AddRow();
                rS3.HeadingFormat = true;
                rS3.Borders.Width = 0;
                gradingTable[5, 0].MergeRight = 3;

                //Special Instructions Information
                TextFrame txtFSpecialInfo2;
                txtFSpecialInfo2 = gradingTable[5, 0].AddTextFrame();
                txtFSpecialInfo2.LineFormat.Width = 0;
                txtFSpecialInfo2.Height = "4.4cm";
                txtFSpecialInfo2.Width = "9.3cm";
                txtFSpecialInfo2.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtFSpecialInfo2.RelativeVertical = RelativeVertical.Page;
                paragraph = txtFSpecialInfo2.AddParagraph();
                paragraph.AddText(colourInstructions);
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.Format.Font.Name = "Tahoma";
                paragraph.Format.Font.Size = 18;
                paragraph.Format.Font.Bold = true;
                
            }
            //Mixed by
            TextFrame MixedBy;
            MixedBy = section.AddTextFrame();
            MixedBy.LineFormat.Width = 0.5;
            MixedBy.Height = "0.5cm";//any number 
            MixedBy.Width = "10.5cm";//any number 
            MixedBy.Left = "9.5cm";
            MixedBy.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            MixedBy.Top = "19.05cm";
            MixedBy.RelativeVertical = RelativeVertical.Page;
            paragraph = MixedBy.AddParagraph();
            paragraph.AddText("MIXED BY____________________________(FIRST NAME)");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Font.Size = 11;

            string sp = "";

            if(methodPS == "PEELING"){
                sp = "PEELED";
            }
            else if (methodPS == "SLITTING")
            {
                sp = "SLIT";
            }

            //Peeled by
            TextFrame PeeledBy;
            PeeledBy = section.AddTextFrame();
            PeeledBy.LineFormat.Width = 0.5;
            PeeledBy.Height = "0.5cm";//any number 
            PeeledBy.Width = "8cm";//any number 
            PeeledBy.Left = "20cm";
            PeeledBy.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            PeeledBy.Top = "19.05cm";
            PeeledBy.RelativeVertical = RelativeVertical.Page;
            paragraph = PeeledBy.AddParagraph();
            paragraph.AddText(sp + " BY_______________(FIRST NAME)");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Font.Size = 11;         


            //DateTIme2
            TextFrame DateTIme2;
            DateTIme2 = section.AddTextFrame();
            DateTIme2.LineFormat.Width = 0.5;
            DateTIme2.Height = "0.5cm";//any number 
            DateTIme2.Width = "10.5cm";//any number 
            DateTIme2.Left = "9.5cm";
            DateTIme2.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            DateTIme2.Top = "19.6cm";
            DateTIme2.RelativeVertical = RelativeVertical.Page;
            paragraph = DateTIme2.AddParagraph();
            paragraph.AddText("DATE MIXED _____/____/_____                  TIME:____:____");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Font.Size = 11;

            //DateTIme3
            TextFrame DateTIme3;
            DateTIme3 = section.AddTextFrame();
            DateTIme3.LineFormat.Width = 0.5;
            DateTIme3.Height = "0.5cm";//any number 
            DateTIme3.Width = "8cm";//any number 
            DateTIme3.Left = "20cm";
            DateTIme3.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            DateTIme3.Top = "19.6cm";
            DateTIme3.RelativeVertical = RelativeVertical.Page;
            paragraph = DateTIme3.AddParagraph();
            paragraph.AddText("DATE "+sp+" ____/___/____     TIME:___:___");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Font.Size = 11;

            //Created By
            TextFrame UserName;
            UserName = section.AddTextFrame();
            UserName.LineFormat.Width = 0;
            UserName.Height = "0.5cm";//any number 
            UserName.Width = "10cm";//any number 
            UserName.Left = "18cm";
            UserName.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            UserName.Top = "20.3cm";
            UserName.RelativeVertical = RelativeVertical.Page;
            paragraph = UserName.AddParagraph();
            paragraph.AddText("Formula Created By " + " at " + now);
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Font.Size = 8;

           
            /***************MIXING FORMULA******************/
       
            /*********************Colour Table********************/
            Table colTable = table[2, 1].AddTextFrame().AddTable();
            table[2, 1].MergeRight = 1;
            colTable.Borders.Width = 0.25;           
            colTable.AddColumn(64);
            colTable.AddColumn(55);
            colTable.AddColumn(15);
            colTable.AddColumn(69);
            colTable.AddColumn(66);
            colTable.AddColumn(27);

            int r = 0;//Row number

            /********************Blank Row***********************/
            colTable.AddRow();
            colTable[r, 0].AddParagraph();
            colTable[r, 0].Borders.Top.Width = 0;
            colTable[r, 0].Borders.Bottom.Width = 0;
            colTable[r, 0].Borders.Left.Width = 0;
            colTable[r, 5].Borders.Right.Width = 0;
            colTable[r, 0].MergeRight = 5;
            colTable[r, 0].Format.Font.Size = 4;

            /********************Lifts**********************/
            if (lift1.Length != 0)
            {
                r = r + 1;
                colTable.AddRow();
                colTable.Rows[r].HeadingFormat = true;
                colTable.Rows[r].Format.Font.Bold = false;
                colTable.Rows[r].Format.Font.Size = 20;
                colTable.Rows[r].Format.Font.Name = "Calibri";
                colTable.Rows[r].Borders.Width = 0;
                colTable.Rows[r].Borders.Top.Width = 0;
               
                colTable[r, 0].AddParagraph("LIFT").Format.Alignment = ParagraphAlignment.Right;
                colTable[r, 0].Borders.Left.Width = 0;
                colTable[r, 2].AddParagraph("      BLACK RUBBER       BINS THEN:").Format.Alignment = ParagraphAlignment.Left;
                colTable[r, 2].MergeRight = 3;
                colTable[r, 2].Borders.Left.Width = 0;

                TextFrame tFLift1 = colTable[r, 1].AddTextFrame();
                tFLift1.LineFormat.Width = 1;
                tFLift1.Height = "1cm";
                tFLift1.Width = "2.5cm";
                tFLift1.RelativeHorizontal = RelativeHorizontal.Margin;
                tFLift1.RelativeVertical = RelativeVertical.Page;

                paragraph = tFLift1.AddParagraph();
                paragraph.AddText(lift1);
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.Format.Font.Name = "Calibri";
                paragraph.Format.Font.Size = 19;
                paragraph.Format.Font.Bold = true;
                
            }
            /*************************IF COLOURS EXIST ****************************/
            if (colours.Count > 0)
            {
                r = r + 1;
                colTable.AddRow();
                colTable.Rows[r].HeadingFormat = true;
                colTable.Rows[r].Format.Font.Bold = false;
                colTable.Rows[r].Format.Font.Size = 20;
                colTable.Rows[r].Format.Font.Name = "Calibri";
                colTable.Rows[r].Borders.Width = 0;                

                colTable[r, 0].AddParagraph("ADD").Format.Alignment = ParagraphAlignment.Right;
                colTable[r, 0].Borders.Left.Width = 0;
                colTable[r, 1].AddParagraph("FOLLOWING COLOURS:").Format.Alignment = ParagraphAlignment.Left;
                colTable[r, 1].MergeRight = 4;
                colTable[r, 2].Borders.Left.Width = 0;
                colTable[r, 5].Borders.Right.Width = 0;
                colTable[r, 5].Borders.Right.Width = 0;

                /***********************Ingredients Table***********************/
                r = r + 1;
                colTable.AddRow();
                colTable.Rows[r].HeadingFormat = true;
                colTable.Rows[r].Format.Font.Bold = true;
                colTable.Rows[r].Format.Font.Size = 10;
                colTable.Rows[r].Format.Font.Name = "Calibri";
                colTable.Rows[r].Format.Alignment = ParagraphAlignment.Center;

                colTable[r, 0].AddParagraph("INGREDIENT");
                colTable[r, 0].Borders.Left.Width = 0;
                colTable[r, 0].MergeRight = 2;
                colTable[r, 1].AddParagraph();
                colTable[r, 2].AddParagraph();
                colTable[r, 3].AddParagraph("1-4mm");
                colTable[r, 4].AddParagraph("0.5-1.5mm");
                colTable[r, 5].AddParagraph("KG");
                colTable[r, 5].Borders.Right.Width = 0;

                for (int i = 0; i < colours.Count; i++)
                {                   
                    r = r + 1;
                    colTable.AddRow();
                    colTable.Rows[r].HeadingFormat = true;
                    colTable.Rows[r].Format.Font.Bold = false;
                    colTable.Rows[r].Format.Font.Size = 10;
                    colTable.Rows[r].Format.Font.Name = "Calibri";

                    colTable[r, 0].AddParagraph(colours[i].ColourName).Format.Alignment = ParagraphAlignment.Left;
                    colTable[r, 0].Borders.Left.Width = 0;
                    colTable[r, 0].MergeRight = 2;
                    if (colours[i].BagSize1.Length != 0)
                    {
                        colTable[r, 3].AddParagraph(colours[i].BagSize1).Format.Alignment = ParagraphAlignment.Center;
                    }
                    if (colours[i].Other.Length != 0)
                    {
                        colTable[r, 3].MergeRight = 1;
                        colTable[r, 3].AddParagraph(colours[i].Other).Format.Alignment = ParagraphAlignment.Center;

                    }
                    colTable[r, 4].AddParagraph(colours[i].BagSize2).Format.Alignment = ParagraphAlignment.Center;

                    colTable[r, 5].AddParagraph(colours[i].BagWeight.ToString()).Format.Alignment = ParagraphAlignment.Center;
                    colTable[r, 5].Borders.Right.Width = 0;

                 
                }
            }

           
            if (lift2.Length != 0)
            {
                /*****************Blank Row**************/
                r = r + 1;
                colTable.AddRow();
                colTable.Rows[r].HeadingFormat = true;
                colTable.Rows[r].Format.Font.Bold = false;
                colTable.Rows[r].Format.Font.Size = 10;
                colTable.Rows[r].Format.Font.Name = "Calibri";
                colTable.Rows[r].Borders.Width = 0;
               
                //Lift2

                r = r + 1;
                colTable.AddRow();
                colTable.Rows[r].HeadingFormat = true;
                colTable.Rows[r].Format.Font.Bold = false;
                colTable.Rows[r].Format.Font.Size = 20;
                colTable.Rows[r].Format.Font.Name = "Calibri";
                colTable.Rows[r].Borders.Width = 0;

                colTable[r, 0].AddParagraph("LIFT").Format.Alignment = ParagraphAlignment.Right;
                colTable[r, 0].Borders.Left.Width = 0;
                colTable[r, 2].AddParagraph("      BLACK RUBBER       BINS THEN:").Format.Alignment = ParagraphAlignment.Left;
                colTable[r, 2].MergeRight = 3;
                colTable[r, 2].Borders.Left.Width = 0;
                colTable[r, 5].Borders.Right.Width = 0;

                TextFrame tFLift2 = colTable[r, 1].AddTextFrame();
                tFLift2.LineFormat.Width = 1;
                tFLift2.Height = "1cm";
                tFLift2.Width = "2.5cm";
                tFLift2.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
                tFLift2.RelativeVertical = RelativeVertical.Page;
                paragraph = tFLift2.AddParagraph();
                paragraph.AddText(lift2);
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.Format.Font.Name = "Calibri";
                paragraph.Format.Font.Size = 19;
                paragraph.Format.Font.Bold = true;
               
            }

            /*****************Blank Row**************/
            r = r + 1;
            colTable.AddRow();
            colTable.Rows[r].HeadingFormat = true;
            colTable.Rows[r].Format.Font.Bold = false;
            colTable.Rows[r].Format.Font.Size = 5;
            colTable.Rows[r].Format.Font.Name = "Calibri";
            colTable.Rows[r].Borders.Width = 0;
            colTable[r, 0].MergeRight = 5;

            /*********************Binder*********************/            
            r = r + 1;
            colTable.AddRow();
            colTable.Rows[r].HeadingFormat = true;
            colTable.Rows[r].Format.Font.Bold = false;
            colTable.Rows[r].Format.Font.Size = 19;
            colTable.Rows[r].Format.Font.Name = "Calibri";
            colTable.Rows[r].Borders.Width = 0;

            colTable[r, 0].AddParagraph("ADD").Format.Alignment = ParagraphAlignment.Right;
            colTable[r, 0].Borders.Left.Width = 0;
            colTable[r, 3].AddParagraph("   KG BINDER " + "   " + GetBinderType(binderType)).Format.Alignment = ParagraphAlignment.Left;
            colTable[r, 3].MergeRight = 1;
            colTable[r, 3].Borders.Left.Width = 0;
            colTable[r, 4].Borders.Right.Width = 0;

            TextFrame tFBinder = colTable[r, 1].AddTextFrame();
            tFBinder.LineFormat.Width = 1;
            tFBinder.Height = "1.6cm";
            tFBinder.Width = "2.5cm";
            tFBinder.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            tFBinder.RelativeVertical = RelativeVertical.Page;
            paragraph = tFBinder.AddParagraph();
            paragraph.AddText(binder.ToString());
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Name = "Tahoma";
            paragraph.Format.Font.Size = 36;
            paragraph.Format.Font.Bold = true;

            /*****************Blank Row**************/
            r = r + 1;
            colTable.AddRow();
            colTable.Rows[r].HeadingFormat = true;
            colTable.Rows[r].Format.Font.Bold = false;
            colTable.Rows[r].Format.Font.Size = 5;
            colTable.Rows[r].Format.Font.Name = "Calibri";
            colTable.Rows[r].Borders.Width = 0;
            colTable[r, 0].MergeRight = 5;

            /***************************Minutes***************************/

            r = r + 1;
            colTable.AddRow();
            colTable.Rows[r].HeadingFormat = true;
            colTable.Rows[r].Format.Font.Bold = false;
            colTable.Rows[r].Format.Font.Size = 19;
            colTable.Rows[r].Format.Font.Name = "Calibri";
            colTable.Rows[r].Borders.Width = 0;

            colTable[r, 0].AddParagraph("MIX").Format.Alignment = ParagraphAlignment.Right;
            colTable[r, 0].Borders.Left.Width = 0;
            colTable[r, 3].AddParagraph("   MINUTES").Format.Alignment = ParagraphAlignment.Left;
            colTable[r, 3].MergeRight = 1;
            colTable[r, 3].Borders.Left.Width = 0;
            colTable[r, 4].Borders.Right.Width = 0;

            TextFrame tFMinutes = colTable[r, 1].AddTextFrame();
            tFMinutes.LineFormat.Width = 1;
            tFMinutes.Height = "1.6cm";
            tFBinder.Width = "2.5cm";
            tFMinutes.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            tFMinutes.RelativeVertical = RelativeVertical.Page;
            paragraph = tFMinutes.AddParagraph();
            paragraph.AddText(mixingMinutes.ToString());
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Name = "Tahoma";
            paragraph.Format.Font.Size = 36;
            paragraph.Format.Font.Bold = true;

            /**************************Mixing Message*****************************/

            if (mixingNotes.Length != 0)
            {
                /*****************Blank Row**************/
                r = r + 1;
                colTable.AddRow();
                colTable.Rows[r].HeadingFormat = true;
                colTable.Rows[r].Format.Font.Bold = false;
                colTable.Rows[r].Format.Font.Size = 10;
                colTable.Rows[r].Format.Font.Name = "Calibri";
                colTable.Rows[r].Borders.Width = 0;
                colTable[r, 0].MergeRight = 5;

                r = r + 1;
                colTable.AddRow();
                colTable.Rows[r].Borders.Width = 0;
                colTable.Rows[r].Borders.Style = BorderStyle.DashSmallGap;
                colTable[r, 0].MergeRight = 5;
               
                TextFrame txtMixingNotes;
                txtMixingNotes = colTable[r, 0].AddTextFrame();
                txtMixingNotes.LineFormat.Width = 2;
                txtMixingNotes.LineFormat.DashStyle = DashStyle.Dash;
                txtMixingNotes.Height = "2.2cm";
                txtMixingNotes.Width = "10.2cm";
                txtMixingNotes.Left = "1cm";
                txtMixingNotes.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant                
                txtMixingNotes.RelativeVertical = RelativeVertical.Page;
                paragraph = txtMixingNotes.AddParagraph();
                paragraph.AddText(mixingNotes);
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.Format.Font.Name = "Calibri";
                paragraph.Format.Font.Size = 19;
                paragraph.Format.Font.Bold = true;              

            }


            PeelingContent(table);

            /*****************Blank Row**************/
            r = r + 1;
            colTable.AddRow();
            colTable.Rows[r].HeadingFormat = true;
            colTable.Rows[r].Format.Font.Bold = false;
            colTable.Rows[r].Format.Font.Size = 5;
            colTable.Rows[r].Format.Font.Name = "Calibri";
            colTable.Rows[r].Borders.Width = 0;
            colTable[r, 0].MergeRight = 5;

            /***************************Rubber Quality***************************/

            r = r + 1;
            colTable.AddRow();
            colTable.Rows[r].HeadingFormat = true;
            colTable.Rows[r].Format.Font.Bold = false;
            colTable.Rows[r].Format.Font.Size = 12;
            colTable.Rows[r].Format.Font.Name = "Calibri";
            colTable.Rows[r].Borders.Top.Width = 0.5;
            colTable[r, 0].Borders.Left.Width = 0;
            colTable[r, 5].Borders.Right.Width = 0;
            colTable[r, 0].Borders.Bottom.Width = 0;
            colTable[r, 0].AddParagraph("Did you notice anything abnormal about the rubber quality? Please state").Format.Alignment = ParagraphAlignment.Left;
            colTable[r, 0].MergeRight = 5;
        }


        void PeelingContent(Table tblMain)
        {
            string sp = ""; string str1 = "";

            if (methodPS == "PEELING")
            {
                sp = "Peeled";
                str1 = "Lineal Meters Produced";
            }
            else if (methodPS == "SLITTING")
            {
                sp = "Slit";
                str1 = "Paver Quantity produced";
            }

            /*********************Peeling Table********************/
            Table tblPeeling = tblMain[2, 3].AddTextFrame().AddTable();

            tblPeeling.Borders.Width = 0.25;
            tblPeeling.AddColumn(145);
            tblPeeling.AddColumn(81);        

            /********************Lift1**********************/
            
            Row rCusName1 = tblPeeling.AddRow();
            rCusName1.HeadingFormat = true;
            rCusName1.Format.Font.Bold = false;
            rCusName1.Format.Font.Size = 12;
            rCusName1.Format.Font.Name = "Tahoma";
            rCusName1.Borders.Width = 0;
            rCusName1.Cells[0].AddParagraph("Customer Name:___________________").Format.Alignment = ParagraphAlignment.Left;
            rCusName1.Cells[0].MergeRight = 1;

            Row rCusName2 = tblPeeling.AddRow();
            rCusName2.Format.Font.Size = 12;
            rCusName2.Borders.Width = 0;
            rCusName2.Format.Font.Name = "Tahoma";
            rCusName2.Cells[0].AddParagraph("_________________________________").Format.Alignment = ParagraphAlignment.Left;
           

            Row blank1 = tblPeeling.AddRow();
            blank1.Format.Font.Size = 12;
            blank1.Borders.Width = 0;
            blank1.Cells[0].MergeRight = 1;          

            Row rSalesOrder1 = tblPeeling.AddRow();
            rSalesOrder1.Format.Font.Size = 12;
            rSalesOrder1.Borders.Width = 0;
            rSalesOrder1.Format.Font.Name = "Tahoma";
            rSalesOrder1.Cells[0].AddParagraph("Sales Order No:____________________").Format.Alignment = ParagraphAlignment.Left;
            rSalesOrder1.Cells[0].MergeRight = 1;
           
            Row rSalesOrder2 = tblPeeling.AddRow();
            rSalesOrder2.Borders.Width = 0;
            rSalesOrder2.Cells[0].AddParagraph("_______________________________________").Format.Alignment = ParagraphAlignment.Left;
            rSalesOrder2.Cells[0].MergeRight = 1;

            Row blank2 = tblPeeling.AddRow();
            blank2.Borders.Width = 0;
            blank2.Cells[0].MergeRight = 1;

            Row rTarget = tblPeeling.AddRow();
            rTarget.Format.Font.Size = 12;
            rTarget.Borders.Width = 0;
            rTarget.Format.Font.Name = "Tahoma";
            rTarget.Cells[0].AddParagraph("Target Thickness " + sp + ":").Format.Alignment = ParagraphAlignment.Left;
            rTarget.Cells[0].Borders.Left.Width = 0;
            rTarget.Cells[1].AddParagraph("________mm").Format.Alignment = ParagraphAlignment.Left;

            if (methodPS == "PEELING")
            {
                Row rActual = tblPeeling.AddRow();
                rActual.Format.Font.Size = 12;
                rActual.Borders.Width = 0;
                rActual.Format.Font.Name = "Tahoma";
                rActual.Cells[0].AddParagraph("Actual Thickness " + sp + ":").Format.Alignment = ParagraphAlignment.Left;
                rActual.Cells[0].Borders.Left.Width = 0;
                rActual.Cells[1].AddParagraph("________mm").Format.Alignment = ParagraphAlignment.Left;
            }
            Row blank3 = tblPeeling.AddRow();
            blank3.Borders.Width = 0;
            blank3.Cells[0].MergeRight = 1;

            Row rLMP = tblPeeling.AddRow();
            rLMP.HeadingFormat = true;
            rLMP.Format.Font.Bold = false;
            rLMP.Format.Font.Size = 12;
            rLMP.Format.Font.Name = "Tahoma";
            rLMP.Borders.Width = 0;

            rLMP.Cells[0].AddParagraph(str1).Format.Alignment = ParagraphAlignment.Left;
           
            TextFrame tFLMPBox = rLMP.Cells[1].AddTextFrame();
            tFLMPBox.LineFormat.Width = 1;
            tFLMPBox.Height = "2cm";
            tFLMPBox.Width = "2.5cm";           
            tFLMPBox.RelativeHorizontal = RelativeHorizontal.Margin;//irrelevant 
            tFLMPBox.RelativeVertical = RelativeVertical.Page;

            Row blank4 = tblPeeling.AddRow();
            blank4.Borders.Width = 0;
            blank4.Cells[0].MergeRight = 1;
            
            Row rRejectAmount = tblPeeling.AddRow();
            rRejectAmount.Format.Font.Size = 12;
            rRejectAmount.Borders.Width = 0;
            rRejectAmount.Format.Font.Name = "Tahoma";
            rRejectAmount.Cells[0].AddParagraph("Did you get any reject amount?_______").Format.Alignment = ParagraphAlignment.Left;
            rRejectAmount.Cells[0].Borders.Left.Width = 0;
            rRejectAmount.Cells[0].MergeRight = 1;

            Row rStock1 = tblPeeling.AddRow();
            rStock1.Format.Font.Size = 12;
            rStock1.Borders.Width = 0;
            rStock1.Format.Font.Name = "Tahoma";
            rStock1.Cells[0].AddParagraph("Will you have any for stock?__________").Format.Alignment = ParagraphAlignment.Left;
            rStock1.Cells[0].Borders.Left.Width = 0;
            rStock1.Cells[0].MergeRight = 1;

            Row rStock2 = tblPeeling.AddRow();
            rStock2.Format.Font.Size = 12;
            rStock2.Borders.Width = 0;
            rStock2.Format.Font.Name = "Tahoma";
            rStock2.Cells[0].AddParagraph("Quantity__________________________").Format.Alignment = ParagraphAlignment.Left;
            rStock2.Cells[0].Borders.Left.Width = 0;
            rStock2.Cells[0].MergeRight = 1;

            Row blank5 = tblPeeling.AddRow();
            blank5.Borders.Width = 0;
            blank5.Cells[0].MergeRight = 1;

            Row rIrregularities = tblPeeling.AddRow();
            rIrregularities.Format.Font.Size = 12;
            rIrregularities.Format.Font.Bold = false;
            rIrregularities.Borders.Width = 0;
            rIrregularities.Format.Font.Name = "Calibri";
            rIrregularities.Borders.Top.Width = 1;
            rIrregularities.Cells[0].AddParagraph("Did you notice any irregularities with the quality of the finished product? Please State").Format.Alignment = ParagraphAlignment.Left;
            rIrregularities.Cells[0].Borders.Left.Width = 0;
            rIrregularities.Cells[0].MergeRight = 1;

          

        }

        private string CheckPlural()
        {
            string val = "";
            if (noOfBins > 1)
            {
                val = "S";
            }
            else
            {
                val = "";
            }
            return val;
        }

       

        private string GetBinderType(string bType)
        {
            string binderStr = string.Empty;
            if (binderType.Length != 0)
            {
                binderStr = "(" + binderType + ")";
            }
            return binderStr;
        }


    }
}
