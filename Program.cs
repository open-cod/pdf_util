using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePdfwithSig
{
	class Program
	{
		static int Main(string[] args)
		{
//			args[0] = @"d:\temp_egron\20191225\доверенность копия.pdf";
			if (args.Length == 0)
			{
				System.Console.WriteLine("Не указаны параметры вызова");
				Console.ReadKey();
				return 1;
			}
			else
			{
				int result = 1;


				//если нет вторго параметра, то объединяем только указанный файл
				if (!string.IsNullOrEmpty(args[0]) && !string.IsNullOrEmpty(args[1] ) && args[1] == "file")
				{
					clSignature sig = new clSignature();
					//			string pathPdf = clMerge.getMergePdfwithSig(@"d:\work_temp\паке_ЕГРОН\report-0fda88b2-d87b-44b1-9967-335035bb6625-BC-2020-05-28-141452-05-03[2].pdf");
					string pathPdf = clMerge.getMergePdfwithSig(args[0]);
					
					Process.Start(pathPdf);
					sig.clearFile(args[0]);
					//			Environment.Exit(0);
					result = 0;
				}


				//второй параметр "path"/ указывает на то что работаем с дирректорией, и объединять необходимо все найденные pdf
				if (!string.IsNullOrEmpty(args[0]) && !string.IsNullOrEmpty(args[1] ) && args[1]=="path")
				{


					var fileList = Directory.GetFiles(args[0]);
					foreach (var filePath in fileList)
					{

						FileInfo fileInf = new FileInfo(filePath);
//						Console.WriteLine("Имя файла: {0}", fileInf.Name);
//						Console.WriteLine("Время создания: {0}", fileInf.CreationTime);
//						Console.WriteLine("Размер: {0}", fileInf.Length);
//						Console.WriteLine("sign: {0}", fileInf.Name.Contains("_withSign"));

						if (fileInf.Exists)
						{

							if (fileInf.Extension.ToUpper() == ".PDF" && !fileInf.Name.Contains("_withSign") && !fileInf.Name.Contains("sigToPdf.pdf"))
							{
								clSignature sig = new clSignature();
								string pathPdf = clMerge.getMergePdfwithSig(filePath);
								
								Process.Start(pathPdf);
								sig.clearFile(args[0]);
							}
						}


					}

					result = 0;
				}


				//если нет вторго параметра, то объединяем только указанный файл
				if (!string.IsNullOrEmpty(args[0]) && !string.IsNullOrEmpty(args[1]) && args[1] == "htmlToPdf")
				{
//					clSignature sig = new clSignature();

					string pathPdf = clMerge.getConvertHtmlToPdf(args[0]);

					Process.Start(pathPdf);

					result = 0;
				}
				return result;

			}
			



			

		}
	}
}
