using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ItextSharp
{
    class Program
    {


        private static bool _addBarcode = false;
        private static bool _addTick = true;
        private static bool _addOneLineText = false;
        private static bool _addMultiLineText = false;
        private static bool _addSpaceBetweenCharacter = false;


        private static string _sampleLongText =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
            "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private static string _sampleSmallText = "a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a a";

        private static readonly BaseFont StfHelveticaTurkish = BaseFont.CreateFont("Helvetica", "CP1254", BaseFont.NOT_EMBEDDED);
        static void Main(string[] args)
        {
            byte[] modifiedByte;
            var pdfByte = File.ReadAllBytes(@"mono.pdf");

            var reader = new PdfReader(pdfByte);

            int numPages = reader.NumberOfPages;
            using (MemoryStream outputStream = new MemoryStream())
            {
                var stamper = new PdfStamper(reader, outputStream);
                var pageSize = reader.GetPageSize(1);
                Console.WriteLine("Width :" + pageSize.Width + "-- Height :" + pageSize.Height);
                if (numPages % 2 == 1)
                {
                    stamper.InsertPage(numPages + 1, pageSize);
                }

                var cb = stamper.GetOverContent(1);
                cb.BeginText();
                if (_addBarcode)
                {
                    var image = GetBarcodeImage(cb, "24206102894", 225, 50);
                    image.SetAbsolutePosition(0, 0);
                    cb.AddImage(image);
                }

                if (_addTick)
                {
                    cb.SetFontAndSize(StfHelveticaTurkish, 14);
                    cb.SetTextMatrix(173, 387 + (395 - 387) / 2);
                    cb.ShowText("X");
                }

                if (_addOneLineText)
                {
                    //x0,y0+(ycenter-y0)/2
                    cb.SetFontAndSize(StfHelveticaTurkish, 12);
                    cb.SetTextMatrix(118, 579 + (586 - 579) / 2);
                    cb.ShowText("24206102894");
                }

                if (_addMultiLineText)
                {
                    ColumnText cvv = new ColumnText(cb);
                    //x0,y0,x1,y1
                    cvv.SetSimpleColumn(new Phrase(new Chunk(_sampleSmallText)), 0, 698, 188, 842, 12,
                        Element.ALIGN_TOP);
                    cvv.Go();
                }

                if (_addSpaceBetweenCharacter)
                {
                    //gelen px / 2 char spacing   12 punto için 15 ise 14 için ters orantı kur
                    cb.SetCharacterSpacing(7.5f);
                    cb.SetFontAndSize(StfHelveticaTurkish, 12);
                    cb.SetTextMatrix(119, 699 + (706 - 699) / 2);
                    cb.ShowText("24206102894");
                }



                cb.EndText();
                stamper.Close();
                modifiedByte = outputStream.ToArray();
            }
            File.WriteAllBytes("test.pdf", modifiedByte);
            System.Diagnostics.Process.Start("test.pdf");
            Console.ReadLine();
        }

        public static iTextSharp.text.Image GetBarcodeImage(PdfContentByte cb, string barcodeText, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(barcodeText))
                barcodeText = "123123123";

            Barcode128 code128 = new Barcode128();
            code128.CodeType = iTextSharp.text.pdf.Barcode.CODE128;
            code128.ChecksumText = true;
            code128.GenerateChecksum = true;
            code128.StartStopText = true;
            code128.Code = barcodeText;
            iTextSharp.text.Image image = code128.CreateImageWithBarcode(cb, BaseColor.BLACK, BaseColor.BLACK);
            image.ScaleAbsoluteWidth(width);
            image.ScaleAbsoluteHeight(height);
            return image;
        }
    }
}
