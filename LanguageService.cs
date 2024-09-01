using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.TextAnalytics;

namespace Custom_Vision
{
	public static class LanguageService
	{
		private static readonly AzureKeyCredential credentials = new AzureKeyCredential("79a4e5621a7543e0819dec6ad869d592");
		private static readonly Uri endpoint = new Uri("https://anvilanguagedemo.cognitiveservices.azure.com/");
		static TextAnalyticsClient client = new TextAnalyticsClient(endpoint, credentials);

		public static void NamedEntityRecognition(List<string> texts)
		{
            foreach (var text in texts)
            {
				Console.WriteLine($"\n");
				Console.WriteLine($"********  {text} ***************");
				var response = client.RecognizeEntities(text);
				
				Console.WriteLine($"\n");
				foreach (var entity in response.Value)
				{
					Console.WriteLine($"Text: {entity.Text}");
					Console.WriteLine($"\tCategory: {entity.Category};\tSub-Category: {entity.SubCategory};\tScore: {entity.ConfidenceScore:F2}");
			    }

			}			
		}

		public static void RecognizePIIExample(List<string> texts)
		{
			foreach (var text in texts)
			{
				PiiEntityCollection entities = client.RecognizePiiEntities(text).Value;
				Console.WriteLine("\n");
				Console.WriteLine($"Given Text: {text}");
				Console.WriteLine("\n");
				Console.WriteLine($"Redacted Text: {entities.RedactedText}");
				Console.WriteLine("\n");
				if (entities.Count > 0)
				{
					Console.WriteLine($"Recognized {entities.Count} PII entit{(entities.Count > 1 ? "ies" : "y")}:");
					foreach (PiiEntity entity in entities)
					{
						Console.WriteLine($"Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
					}
				}
				else
				{
					Console.WriteLine("No entities were found.");
				}
			}
		}

		static void KeyPhraseExtraction(List<string> texts)
		{
			foreach (var text in texts)
			{
				Console.WriteLine($"\n");
				Console.WriteLine($"********  {text} ***************");
				var response = client.ExtractKeyPhrases(text);

				Console.WriteLine("Key phrases:");
				foreach (string keyphrase in response.Value)
				{
					Console.WriteLine($"\t{keyphrase}");
				}
			}
		}

		public static void EntityLinking(List<string> texts)
		{
			foreach (var text in texts)
			{
				var response = client.RecognizeLinkedEntities(
				text);
				Console.WriteLine($"Given Text: {text}");
				Console.WriteLine("Linked Entities:");
				foreach (var entity in response.Value)
				{
					Console.WriteLine($"\tName: {entity.Name}\tID: {entity.DataSourceEntityId}\tURL: {entity.Url}\tData Source: {entity.DataSource}");
					//Console.WriteLine("\tMatches:");
					//foreach (var match in entity.Matches)
					//{
					//	Console.WriteLine($"\t\tText: {match.Text}");
					//	Console.WriteLine($"\t\tScore: {match.ConfidenceScore:F2}");
					//	Console.WriteLine($"\t\tLength: {match.Length}");
					//	Console.WriteLine($"\t\tOffset: {match.Offset}\n");
					//}
				}
				Console.WriteLine("\n");
			}
		}

		public static void SentimentAnalysis(List<string> texts)
		{
			foreach (var text in texts)
			{
				
				DocumentSentiment documentSentiment = client.AnalyzeSentiment(text);
				Console.WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");

				foreach (var sentence in documentSentiment.Sentences)
				{
					Console.WriteLine($"\tText: \"{sentence.Text}\"");
					Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
					Console.WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
					Console.WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
					Console.WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");
				}
				Console.WriteLine("\n");
			}
		}

		public static void SentimentAnalysisOptionMinning(List<string> texts)
		{
			foreach (var text in texts)
			{

				DocumentSentiment documentSentiment = client.AnalyzeSentiment(text, 
					options: new AnalyzeSentimentOptions { IncludeOpinionMining = true});

				Console.WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");

				foreach (var sentence in documentSentiment.Sentences)
				{
					Console.WriteLine($"\tText: \"{sentence.Text}\"");
					Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
					Console.WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
					Console.WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
					Console.WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");

					Console.WriteLine($"\tOptions: ");
					foreach (var opinion in sentence.Opinions)
					{
						Console.WriteLine($"\tTarget: {opinion.Target.Text}");
						foreach (var assesment in opinion.Assessments)
						{
							Console.WriteLine($"\t Text: {assesment.Text} ; Sentiment: {assesment.Sentiment}");

						}
					}
				}
				Console.WriteLine("\n");
			}
		}

		public static void LanguageDetection(List<string> texts)
		{
			foreach (var text in texts)
			{
			Console.WriteLine($"Text: {text}");
				DetectedLanguage detectedLanguage = client.DetectLanguage(text);
				Console.WriteLine("Language:");
				Console.WriteLine($"\t{detectedLanguage.Name}\tISO-6391: {detectedLanguage.Iso6391Name}\n");
			}
		}

		public static void callLanguageDetection()
		{
			List<string> texts = new List<string>();
			texts.Add("Happy Birthday");
			texts.Add("feliz cumpleaños");
			texts.Add(" పుట్టినరోజు శుభాకాంక్షలు");
			texts.Add("जन्मदिन की शुभकामनाएँ");
			texts.Add("ಜನ್ಮದಿನದ ಶುಭಾಶಯಗಳು");
			texts.Add("Alles Gute zum Geburtstag");
			LanguageDetection(texts);
		}



		public static void callSentimentAnalysis()
		{
			List<string> texts = new List<string>();
			texts.Add("The room was great, but staff was unfriendly.");
			texts.Add("The Laptop performance was good, but keypad is bad.");
			texts.Add("Neeraj Chopra won Silver Medal.");
			SentimentAnalysisOptionMinning(texts);
		}



		public static void callEntityLinking()
		{
			List<string> texts = new List<string>();
			texts.Add("Narendra Modi has beacame Primeminister for third time");
			texts.Add("People in Bangladesh try to flee to India amid violence");
			texts.Add("Neeraj Chopra won Silver Medal.");
			EntityLinking(texts);
		}

		public static void callRecognizePIIExample()
        {
			List<string> texts = new List<string>();
			texts.Add("A customer with SSN 859 - 98 - 0987 and bank account number 12345678 whose phone number is 800-102-1100 applied for a loan");
			texts.Add("Hello, My aadhar card number is 456378923456");
			texts.Add("People in Bangladesh try to flee to India amid violence");
			texts.Add("Neeraj Chopra won Silver Medal.");
			RecognizePIIExample(texts);
		}

		public static void callKeyPhraseExtraction()
		{
			List<string> texts = new List<string>();
			texts.Add("A customer with SSN 859 - 98 - 0987 and bank account number 12345678 whose phone number is 800 - 102 - 1100 applied for a loan");
			texts.Add("Hello, My aadhar card number is 456378923456");
			texts.Add("My cat might need to see a veterinarian.");
			texts.Add("Neeraj Chopra won Silver Medal.");
			KeyPhraseExtraction(texts);
		}




		public static void callNamedEntity()
        {
			List<string> texts = new List<string>();
			texts.Add("India Won 6 Medals at Olympics");
			texts.Add("Trump and Kamala Harris were in President race in America");
			texts.Add("Hindus in Bangladesh try to flee to India amid violence");
			texts.Add("Neeraj Chopra won Silver Medal.");
			NamedEntityRecognition(texts);
        }

	}


}
