using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

using iText.Html2pdf;
using iText.Kernel.Utils;
using iTextSharp.awt.geom;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;



namespace ConsolePdfwithSig
{
	class clSignature
	{

		public void clearFile(string pathFile)
		{
			
			string StartupPath = Path.GetDirectoryName(pathFile);
			File.Delete(StartupPath + @"\sigToPdf.pdf");
			File.Delete(StartupPath + @"\sig.html");

		}


		//Перевод подписи ЭЦП в html вид
		public string getHtmlSig(string pathFile)
		{
			string StartupPath = Path.GetDirectoryName(pathFile);
			string pathHtml = StartupPath + @"\sig.html";
			string SignHtml = mGetReflectionFromSig4Html(pathFile);
			File.WriteAllText(pathHtml, SignHtml, Encoding.Default);

			return pathHtml;
		}


		/// <summary>
		////получение атрибутов подписи для прикрепления их к полученной выписке
		/// </summary>
		/// <returns></returns>
		private string mGetReflectionFromSig4Html(string pathFile)
		{
			string resultHtml = "<div style = \" text-align: center \"><b>Информация об электронных подписях документа</b> <br></div>";
			var extension = Path.GetExtension(pathFile);
			if (extension != null)
			{
				var dirName = Path.GetDirectoryName(pathFile);
				var fileWithoutExt = Path.GetFileNameWithoutExtension(pathFile);
				string pathSig = String.Empty;

				List<string> arrString = new List<string>();
				

				string fileNameSig = String.Empty;
				//ищем в папке ЭЦП
				// сложность в том, что в наименовании ЭЦП из ЕГРП прихоядт цифры
				DirectoryInfo dir = new DirectoryInfo(dirName);
				foreach (var filesFileInfo in dir.GetFiles())
				{
					if (filesFileInfo.Extension.ToUpper() == ".SIG")
					{
						if (filesFileInfo.Name.IndexOf(fileWithoutExt) == 0 && filesFileInfo.Name.IndexOf(fileWithoutExt+".xml") <0 && filesFileInfo.Name.IndexOf("regsign")<0  )
						{
							//считаем, что подпись наша
							fileNameSig = filesFileInfo.Name;
							arrString.Add(fileNameSig);


						}
					}

				}
				
//				foreach (var fileNameForSig in arrString)
				for (int i = 0; i < arrString.Count; i++)
				
					{

					var fileDescription = string.Empty;
					if (!string.IsNullOrEmpty(arrString[i]))
					{
						pathSig = dirName + "\\" + arrString[i];

						try
						{
							X509Certificate2Collection certCol;
							ContentInfo contentInfo;
							SignedCms signedCms;

							byte[] bytes = File.ReadAllBytes(pathSig);

							contentInfo = new ContentInfo(bytes);
							signedCms = new SignedCms(contentInfo, true);


							try
							{
								signedCms.Decode(bytes);
							}
							catch (Exception e)
							{
								bytes = Convert.FromBase64String(File.ReadAllText(pathSig));

								contentInfo = new ContentInfo(bytes);
								signedCms = new SignedCms(contentInfo, true);
								signedCms.Decode(bytes);
							}



							certCol = new X509Certificate2Collection();
							foreach (var item in signedCms.SignerInfos)
							{
								certCol.Add(item.Certificate);
							}

							/*
						cert.Issuer		        центр сертификации				            CN=KOPR\nburlakova	
						cert.IssuerName.Name	различающееся имя поставщика сертификата	CN=KOPR\nburlakova	
						cert.FriendlyName	    связанный псевдоним для сертификата		    ""
						cert.NotAfter		    Дата действия сертификата			        05.06.2015
						sert.Subject		    Имя субъекта					            CN=KOPR\nburlakova
						cert.SubjectName.Name	различающееся имя субъекта от сертификата.	CN=KOPR\nburlakova
						*/
							X509Certificate cert = X509Certificate.CreateFromSignedFile(Path.GetFullPath(pathSig));
							string dateTimeFile = cert.GetEffectiveDateString();
							//						FileInfo fi = new FileInfo(pathSig);
							//					    string dateTimeFile = fi.CreationTime.ToShortDateString() + " " + fi.CreationTime.ToLongTimeString();
//							return GetCertificationDescription(certCol);
							resultHtml += GetCertificationDescription(certCol);
							
						}
						catch (Exception e)
						{
							return string.Empty;
						}
					}
					
				}
				return resultHtml;
				// Если это файл ЭЦП, вытащим из него текст в комментарий
				//				var fileDescription = string.Empty;
				//				if (!string.IsNullOrEmpty(fileNameSig))
				//				{
				//					pathSig = dirName + "\\" + fileNameSig;
				//
				//					try
				//					{
				//						X509Certificate2Collection certCol;
				//						ContentInfo contentInfo;
				//						SignedCms signedCms;
				//
				//						byte[] bytes = File.ReadAllBytes(pathSig);
				//
				//						contentInfo = new ContentInfo(bytes);
				//						signedCms = new SignedCms(contentInfo, true);
				//
				//
				//						try
				//						{
				//							signedCms.Decode(bytes);
				//						}
				//						catch (Exception e)
				//						{
				//							bytes = Convert.FromBase64String(File.ReadAllText(pathSig));
				//
				//							contentInfo = new ContentInfo(bytes);
				//							signedCms = new SignedCms(contentInfo, true);
				//							signedCms.Decode(bytes);
				//						}
				//
				//
				//
				//						certCol = new X509Certificate2Collection();
				//						foreach (var item in signedCms.SignerInfos)
				//						{
				//							certCol.Add(item.Certificate);
				//						}
				//
				//						/*
				//				    cert.Issuer		        центр сертификации				            CN=KOPR\nburlakova	
				//				    cert.IssuerName.Name	различающееся имя поставщика сертификата	CN=KOPR\nburlakova	
				//				    cert.FriendlyName	    связанный псевдоним для сертификата		    ""
				//				    cert.NotAfter		    Дата действия сертификата			        05.06.2015
				//				    sert.Subject		    Имя субъекта					            CN=KOPR\nburlakova
				//				    cert.SubjectName.Name	различающееся имя субъекта от сертификата.	CN=KOPR\nburlakova
				//				    */
				//						X509Certificate cert = X509Certificate.CreateFromSignedFile(Path.GetFullPath(pathSig));
				//						string dateTimeFile = cert.GetEffectiveDateString();
				//						//						FileInfo fi = new FileInfo(pathSig);
				//						//					    string dateTimeFile = fi.CreationTime.ToShortDateString() + " " + fi.CreationTime.ToLongTimeString();
				//						return GetCertificationDescription(certCol);
				//					}
				//					catch (Exception e)
				//					{
				//						return string.Empty;
				//					}
				//				}
				return string.Empty;
			}
			return string.Empty;
		}

		private string GetCertificationDescription(X509Certificate2Collection certCol)
		{
			string[] sTrim = { "CN=", "O=", "L=", "S=", "C=", "STREET=", "G=", "SN=", "OU=", "E=" };
			string fileDescription = string.Empty;
			string sSubject;
			string sIssuer;

			string regReccordhtml = String.Empty;

			//			regReccordhtml =
			//				"<!DOCTYPE HTML>" + Environment.NewLine +
			//				"<html>" + Environment.NewLine +
			//				"<head>" + Environment.NewLine +
			//				"<meta http - equiv = \"content-type\" content = \"text/html\" />" + Environment.NewLine +
			//				"<meta name = \"author\" content = \"admin\" />" + Environment.NewLine +
			//				"<title > Специальная регистрационная надпись</title>" + Environment.NewLine +
			//				"</head>" + Environment.NewLine;

			for (int i = 0; i < certCol.Count; i++)
			{
				sSubject = certCol[i].Subject;
				sIssuer = certCol[i].Issuer;
				foreach (var item in sTrim)
				{
					sSubject = sSubject.Replace(item, string.Empty);
					sIssuer = sIssuer.Replace(item, string.Empty);
				}

				fileDescription +=
					"<div style = \"width: 400px; height: 220px; color: #0000; border-radius: 20px 20px 20px 20px; padding:0px; display: table; border: 4px double slateblue;margin-left: 100px; text-align:center\" > " +
					Environment.NewLine +
					"<div style = \"display: table-row; width:100%\" > " + Environment.NewLine +
					"<p style = \"color: slateblue; border-bottom: 1px solid slateblue; \" ><b> ДОКУМЕНТ ПОДПИСАН<br> ЭЛЕКТРОННОЙ ПОДПИСЬЮ </b></p> " +
					Environment.NewLine +
					"</div > " + Environment.NewLine +
					"<div style = \"width:100%; height: 160px; display: table\" > " + Environment.NewLine +
					"<div style = \"display: table-row\" > " + Environment.NewLine +
					"<div style = \"display: table-cell; width:25%;border-right: 1px solid slateblue;   color:slateblue; font-size: 12px; text-align: left; padding: 0 0 0 5\" > " +
					Environment.NewLine +
					"<b> Сертификат </b> " + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"<div style = \"display: table-cell; width:80%;  color:slateblue; font-size: 12px; text-align: left; padding: 5 15\" > " +
					Environment.NewLine +
					 certCol[i].SerialNumber + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"<div style = \"display: table-row\" > " + Environment.NewLine +
					"<div style = \"display: table-cell; width:25%;border-right: 1px solid slateblue;   color:slateblue; font-size: 12px; text-align: left; padding: 0 0 0 5\" > " +
					Environment.NewLine +
					"<b> Владелец </b> " + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"<div style = \"display: table-cell; width:80%;  color:slateblue; font-size: 12px; text-align: left; padding: 5 15\" > " +
					Environment.NewLine +
					sSubject + Environment.NewLine +
					" </div> " + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"<div style = \"display: table-row\" > " + Environment.NewLine +
					"<div style = \"display: table-cell; width:25%;border-right: 1px solid slateblue;  color:slateblue; font-size: 12px; text-align: left; padding: 0 0 0 5\" > " +
					Environment.NewLine +
					"<b> Действителен </b> " + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"<div style = \"display: table-cell; width:80%; color:slateblue; font-size: 12px; text-align: left; padding:5 15\" > " +
					Environment.NewLine +
					"Период действия сертификата " + "с " + certCol[i].NotBefore.ToString("dd/MM/yyyy") + " по " + certCol[i].NotAfter.ToString("dd/MM/yyyy") + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"</div> " + Environment.NewLine +
					"</div> " + Environment.NewLine;


			}




			//			fileDescription = regReccordhtml +
			//			                  "<body>" + Environment.NewLine +
			//			                  fileDescription + Environment.NewLine +
			//			                  "</body>" + Environment.NewLine +
			//			                  "</html>";

			return fileDescription;
		}



		/// <summary>
		////вставка в HTML электронной подпсис
		/// </summary>
		/// <param name="htmlFilePath"></param>
		/// <param name="signHtml"></param>
		public void SetSignToHtml(string htmlFilePath, string signHtml)
		{

			var fileName = htmlFilePath;
			var extension = Path.GetExtension(fileName);
			if (extension != null)
			{
				var fileExt = extension.ToLower();
				if (fileExt == ".html")
				{
					//var htmlStr = Encoding.Default.GetString(Result[0]);
					string htmlStr = System.IO.File.ReadAllText(htmlFilePath, Encoding.UTF8);
					// Вставка метаданных для отображения текста в кодировке UTF-8 


					var pos = htmlStr.IndexOf("</body>", StringComparison.Ordinal);

					htmlStr = htmlStr.Insert(pos, signHtml);
					File.WriteAllText(htmlFilePath, htmlStr);

					return;
				}
				//				    if (fileExt == ".xml")
				//				    {
				//					    //var htmlStr = Encoding.Default.GetString(Result[0]);
				//					    string htmlStr = System.IO.File.ReadAllText(htmlFilePath, Encoding.UTF8);
				//					    // Вставка метаданных для отображения текста в кодировке UTF-8 
				//
				//
				//					   // var pos = htmlStr.IndexOf("</special_registration_inscription>", StringComparison.Ordinal);
				//					    htmlStr = htmlStr.Insert(0, "<P>") + Environment.NewLine;
				//					    htmlStr = htmlStr + "</P>" + Environment.NewLine;
				//						htmlStr = htmlStr + signHtml;
				//					    File.WriteAllText(htmlFilePath, htmlStr);
				//
				//					    return;
				//				    }
			}


		}

		/// <summary>
		////вставка в HTML электронной подпсис
		/// </summary>
		/// <param name="htmlFilePath"></param>
		/// <param name="signHtml"></param>
		public void SetSignToHtmlEgrn(string htmlFilePath, string signHtml)
		{

			var fileName = htmlFilePath;
			var extension = Path.GetExtension(fileName);
			if (extension != null)
			{
				var fileExt = extension.ToLower();

				if (fileExt == ".html")
				{
					//var htmlStr = Encoding.Default.GetString(Result[0]);
					string htmlStr = System.IO.File.ReadAllText(htmlFilePath, Encoding.UTF8);
					// Вставка метаданных для отображения текста в кодировке UTF-8 

					//htmlStr = htmlStr.Replace(">", ">"+"<br>");

					//				    htmlStr = htmlStr.Replace("</record_type>", "</record_type>" + "<br>");
					//				    htmlStr = htmlStr.Replace("</registration_date>", "</registration_date>" + "<br>");
					//
					//				    htmlStr = htmlStr.Replace("</registration_kind>", "</registration_kind>" + "<br>");
					//				    htmlStr = htmlStr.Replace("</registration_number>", "</registration_number>" + "<br>");
					//				    htmlStr = htmlStr.Replace("</inscription_info>", "</inscription_info>" + "<br>");
					//				    htmlStr = htmlStr.Replace("<X509Data>", "<X509Data>" + "<br>");


					string regReccordhtml = String.Empty;

					regReccordhtml =
						"<!DOCTYPE HTML>" + Environment.NewLine +
						"<html>" + Environment.NewLine +
						"<head>" + Environment.NewLine +
						"<meta http - equiv = \"content-type\" content = \"text/html\" />" + Environment.NewLine +
						"<meta name = \"author\" content = \"admin\" />" + Environment.NewLine +
						"<title > Специальная регистрационная надпись</title>" + Environment.NewLine +
						"</head>" + Environment.NewLine;

					regReccordhtml +=
						"<body>" + Environment.NewLine +
						"<div style = \"border: solid 2px; width: fit-content; max-width: inherit; padding: 12px; \" >" + Environment.NewLine +
						"<p style = \"text-align: center\"> СФОРМИРОВАНА СПЕЦИАЛЬНАЯ РЕГИСТРАЦИОННАЯ НАДПИСЬ </p>" + Environment.NewLine +
						"<hr/>" + Environment.NewLine;


					XmlDocument xDoc = new XmlDocument();
					//xDoc.Load("users.xml");
					xDoc.LoadXml(htmlStr);
					// получим корневой элемент
					XmlElement xRoot = xDoc.DocumentElement;
					// обход всех узлов в корневом элементе
					foreach (XmlNode xnode in xRoot)
					{
						if (xnode.Name == "inscription_info")
						{
							string nameTypeReg = String.Empty;

							// обходим все дочерние узлы элемента user
							foreach (XmlNode childnode in xnode.ChildNodes)
							{
								// если узел 
								if (childnode.Name == "record_type")
								{

									var typeReg = childnode.InnerText.Trim();
									switch (typeReg)
									{
										case "deal_record":
											nameTypeReg = "сделки";
											break;
										case "right_record":
											nameTypeReg = "права";
											break;
										default:
											nameTypeReg = "ограничения или обременения";
											break;


									}
								}
								// если узел age
								if (childnode.Name == "record_info")
								{
									foreach (XmlNode recInfo in childnode.ChildNodes)
									{
										if (recInfo.Name == "registration_kind")
										{
											//		    Console.WriteLine("Компания: {0}", childnode.InnerText);
											regReccordhtml +=
											"<div style = \"display: table-row;\" >" + Environment.NewLine +
											"<div style = \"display: table-cell;\" > <b> Произведена государственная регистрация</b></div>" + Environment.NewLine +
											"<div style = \"display: table-cell; padding: 12px;\" >" + " " + nameTypeReg + " " + "(" + recInfo.InnerText.Trim() + ")" + "</div>" + Environment.NewLine +
											"</div>" + Environment.NewLine;

										}
										if (recInfo.Name == "registration_date")
										{
											//		    Console.WriteLine("Компания: {0}", childnode.InnerText);
											regReccordhtml +=
												"<div style = \"display: table-row;\" >" + Environment.NewLine +
												"<div style = \"display: table-cell;\" > <b>Дата регистрации</b></div>" + Environment.NewLine +
												"<div style = \"display: table-cell; padding: 12px;\" >" + DateTime.Parse(recInfo.InnerText).ToString() + "</div>" + Environment.NewLine +
												"</div>" + Environment.NewLine;
										}

										if (recInfo.Name == "registration_number")
										{
											//		    Console.WriteLine("Компания: {0}", childnode.InnerText);
											regReccordhtml +=
												"<div style = \"display: table-row;\" >" + Environment.NewLine +
												"<div style = \"display: table-cell;\" > <b>Номер регистрации</b ></div>" + Environment.NewLine +
												"<div style = \"display: table-cell; padding: 12px;\" >" + recInfo.InnerText.Trim() + "</div>" + Environment.NewLine +
												"</div>" + Environment.NewLine;
										}

									}
									regReccordhtml += "</div>" + Environment.NewLine;

								}
							}
						}

					}





					//				htmlStr = htmlStr.Replace("<", "&lt;");
					//				    htmlStr = htmlStr.Replace(">", "&gt;");

					// var pos = htmlStr.IndexOf("</special_registration_inscription>", StringComparison.Ordinal);
					//					htmlStr = htmlStr.Insert(0, "<pre>") + Environment.NewLine;
					//					htmlStr = htmlStr + "</pre>" + Environment.NewLine;

					htmlStr = regReccordhtml + signHtml + Environment.NewLine +
						"</body>" + Environment.NewLine +
						"</html>";
					File.WriteAllText(htmlFilePath, htmlStr);

					return;
				}
			}


		}


		public string GetReflectionFromEGRNXml(string xmlFilePath)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFilePath);
			XPathNavigator nav = doc.CreateNavigator();
			nav.MoveToChild("special_registration_inscription", string.Empty);

			nav.MoveToChild("Signature", "http://www.w3.org/2000/09/xmldsig#");

			nav.MoveToChild("KeyInfo", "http://www.w3.org/2000/09/xmldsig#");
			nav.MoveToChild("X509Data", "http://www.w3.org/2000/09/xmldsig#");
			nav.MoveToChild("X509Certificate", "http://www.w3.org/2000/09/xmldsig#");
			string strCertivicate = nav.Value;

			X509Certificate2 crtf = new X509Certificate2(Convert.FromBase64String(strCertivicate.Trim().Replace(" ", "+")));
			return GetCertificationDescription(new X509Certificate2Collection(crtf));
		}


	}
}
