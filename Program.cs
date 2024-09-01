using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Custom_Vision;

Console.WriteLine("Hello, AI!");




//ImageAnalysis.AnalyzeImage();

LanguageService.callLanguageDetection();


string predictionKey = "a6a0bb31892b48ca9d33b9e1978ae3a7";
string trainingKey = "YOUR_CUSTOMVISION_KEY";
string endpoint = "https://anvicustomvisiondemo-prediction.cognitiveservices.azure.com/";
string ProjectID = "8b20b027-f859-4dbc-bdd9-e16361173e43";
string modelname = "Iteration1";
//Predict();



void Predict()
{
    var predictionClient = new CustomVisionPredictionClient(
        new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
    {
        Endpoint = endpoint
    };

    using (var stream = new FileStream(@"C:\Users\Anvi\Dropbox\PC\Music\Anvi-learns\Azre\AI\Demos\Images\Road with Vehicles\main05.jpg",
        FileMode.Open))
    {
       // var result = predictionClient.ClassifyImageAsync(new Guid(ProjectID), "modelname", stream).Result;
        var result = predictionClient.DetectImageAsync(new Guid(ProjectID), modelname, stream).Result;

        foreach (var prediction in result.Predictions.Where(x=>x.Probability > 0.80))
        {
            Console.WriteLine($"Tag: {prediction.TagName} Probability: {prediction.Probability:P1} " +
                $"Location: {prediction.BoundingBox.Left} , {prediction.BoundingBox.Top}");
        }
    }
}


   
    void getProject()

    {
        var trainingApi = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(trainingKey))
        {
            Endpoint = endpoint
        };

        var project = trainingApi.GetProject(new Guid(ProjectID)); 
    }

    //void Predict()
    //{
        
    //}