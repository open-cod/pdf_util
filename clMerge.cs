using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace ConsolePdfwithSig
{
	class clMerge
	{
		public static string getMergePdfwithSig(string pathPdf)
		{

			string StartupPath = Path.GetDirectoryName(pathPdf);
			string FileName = Path.GetFileNameWithoutExtension(pathPdf);
			string FileExt = Path.GetExtension(pathPdf);

			//			string pathPdf = StartupPath + @"\dogovor_magomed.pdf";
			string filePdfNew = "";
			try
			{
				string fileName = StartupPath + @"\" + FileName + "_withSign" + FileExt;
				using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					filePdfNew = StartupPath + @"\" + FileName + "_withSign" + FileExt;
				}
			}
			catch (IOException ioex)
			{
				filePdfNew = StartupPath + @"\" + FileName + "_withSign"+ DateTime.Now.Millisecond + FileExt;
			}



			string tmpPdfSig = StartupPath + @"\sigToPdf.pdf";

			clSignature sig = new clSignature();
			string htmlSig = sig.getHtmlSig(pathPdf);


			//			using (FileStream htmlSource = File.Open(StartupPath + @"\USTUPKA.html", FileMode.Open))
			using (FileStream htmlSource = File.Open(htmlSig, FileMode.Open))
			using (FileStream pdfDest = File.Open(tmpPdfSig, FileMode.OpenOrCreate))
			{
				ConverterProperties converterProperties = new ConverterProperties();
				converterProperties.SetCharset("windows-1251");
				HtmlConverter.ConvertToPdf(htmlSource, pdfDest, converterProperties);


			}

			PdfDocument pdfDocument = new PdfDocument(new PdfReader(pathPdf), new PdfWriter(filePdfNew));
			PdfDocument pdfDocument2 = new PdfDocument(new PdfReader(tmpPdfSig));

			PdfMerger merger = new PdfMerger(pdfDocument);
			merger.Merge(pdfDocument2, 1, pdfDocument2.GetNumberOfPages());

			pdfDocument2.Close();
			pdfDocument.Close();


			return filePdfNew;
		}



		public static string getConvertHtmlToPdf(string pathHtml)
		{

			string StartupPath = Path.GetDirectoryName(pathHtml);
			string FileName = Path.GetFileNameWithoutExtension(pathHtml);
			string FileExt = Path.GetExtension(pathHtml);
			
			string filePdfNew = StartupPath + @"\" + FileName + ".pdf";


			
//			string pdfDest = StartupPath + "output.pdf";
			HtmlConverter.ConvertToPdf(new FileStream(pathHtml, FileMode.Open), new FileStream(filePdfNew, FileMode.Create));
			


			return filePdfNew;
		}
	}
}
