using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Custom_Vision
{
    public  class ImageAnalysis
    {
       

    public static void AnalyzeImage()
        {
            string key = "57ee4434116c4cf4a3a54584075cb2d2";
            string endpoint = "https://anvicomputervisiondemo.cognitiveservices.azure.com/";
            string url = @"https://media.istockphoto.com/id/1368965646/photo/multi-ethnic-guys-and-girls-taking-selfie-outdoors-with-backlight-happy-life-style-friendship.jpg?s=612x612&w=0&k=20&c=qYST1TAGoQGV_QnB_vMd4E8jdaQUUo95Sa2JaKSl_-4=";
            string localUrl = @"C:\Users\Anvi\Dropbox\PC\Music\Anvi-learns\Azre\AI\Demos\Computer Vision\Image Analysis\Images\people.jpg";
            using FileStream stream = new FileStream(localUrl, FileMode.Open);
            bool useLocal = true;
            ImageAnalysisClient client = new(
                new Uri(endpoint),
                new AzureKeyCredential(key));
           ImageAnalysisResult? result = null;

            if (useLocal)
            {
                result = client.Analyze(
                   BinaryData.FromStream(stream),
                 VisualFeatures.Caption | VisualFeatures.Read | VisualFeatures.People | VisualFeatures.Tags,
                   new ImageAnalysisOptions { GenderNeutralCaption = true });
            }
            else
            {
                 result = client.Analyze(
                   new Uri(url),
                 VisualFeatures.Caption | VisualFeatures.Read | VisualFeatures.People,
                   new ImageAnalysisOptions { GenderNeutralCaption = true });
            }

            Console.WriteLine("Image analysis results:");
            Console.WriteLine(" Caption:");
            Console.WriteLine($"   '{result.Caption.Text}', Confidence {result.Caption.Confidence:F4}");

            Console.WriteLine("Image analysis results:");
            Console.WriteLine(" People:");
            foreach (var person in result.People.Values)
            {
                if(person.Confidence > 0.8)
                Console.WriteLine($"   Person: Bounding box {person.BoundingBox.ToString()}, Confidence {person.Confidence:F4}");
            }

            Console.WriteLine("Image analysis results:");
            Console.WriteLine(" Tags:");
            foreach (DetectedTag tag in result.Tags.Values)
            {
                if (tag.Confidence > 0.8)
                    Console.WriteLine($"   '{tag.Name}', Confidence {tag.Confidence:F4}");
            }


            Console.WriteLine(" Read:");
            foreach (DetectedTextBlock block in result.Read.Blocks)
                foreach (DetectedTextLine line in block.Lines)
                {
                    Console.WriteLine($"   Line: '{line.Text}'");
                    foreach (DetectedTextWord word in line.Words)
                    {
                        Console.WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence.ToString("#.####")}");
                    }
                }
        }

    }
}
