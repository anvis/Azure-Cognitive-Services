using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Custom_Vision
{
    public class OCRDemo
    {
        static string key = "57ee4434116c4cf4a3a54584075cb2d2";
        static string endpoint = "https://anvicomputervisiondemo.cognitiveservices.azure.com/";

        // private const string ImageUrl = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/ComputerVision/Images/printed_text.jpg";
        private const string ImageUrl = @"C:\Users\Anvi\Dropbox\PC\Music\Anvi-learns\Azre\AI\Demos\Computer Vision\OCR\Images\Text01.png";
       // private const string ImageUrl = "https://learn.microsoft.com/azure/ai-services/computer-vision/media/quickstarts/presentation.png";

        public static async Task ReadFromImageAsync()
        {
            ComputerVisionClient client =
              new(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };

            var textHeaders = await client.ReadInStreamAsync(File.OpenRead(ImageUrl));//  .ReadAsync(ImageUrl);
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
            ReadOperationResult results;
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine("Operation Completed");

        }

        public static async Task AnalyzeFromImageAsync()
        {
            ComputerVisionClient client =
              new(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Description, VisualFeatureTypes.Tags, VisualFeatureTypes.Adult };
            var textHeaders = await client.AnalyzeImageAsync(ImageUrl,features);

            foreach (ImageTag tag in textHeaders.Tags)
            {
               
                    Console.WriteLine( "Tag Name :- {0} Tag Hint:- {1} Tag Confidence:- {2}", tag.Name, tag.Hint, tag.Confidence);
            }
            Console.WriteLine("Operation Completed");

        }



        public static async Task ExtractFromImageAsyncWithComments()
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };

            var textHeaders = await client.ReadAsync(ImageUrl);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL

            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;

            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine("Operation Completed");

        }

        public static async Task ReadFromImageUsingRestAPIAsync()
        {
             
            string url = endpoint + "/vision/v3.1/read/analyze";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", key);
            byte[] byteData = GetImageAsByteArray(ImageUrl);
            HttpResponseMessage response;
            using ByteArrayContent content = new(byteData);
            content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(url, content);
            string operationLocation;
            if (response.IsSuccessStatusCode)
            {
                operationLocation =
                   response.Headers.GetValues("Operation-Location").FirstOrDefault();

                string contentString;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++i;
                }
                while (i < 60 && contentString.IndexOf("\"status\":\"succeeded\"") == -1);
                // Display the JSON response.
                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
            }

        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
