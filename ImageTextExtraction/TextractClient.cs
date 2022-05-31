using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Amazon;
using Amazon.Textract;
using Amazon.Textract.Model;
using Amazon.S3;
using Amazon.S3.Model;
public class TextractClient
{
    //These arrays are responsible for storing text/data from the images
	List<BlockType> blockTypes = new List<BlockType>();
    List<string> blockText = new List<string>();
    List<string> lineText = new List<string>();


    public async Task StartDetectAsync()
    {
        //My bucket on aws.amazon
        var s3Bucket = "oisinsapps";

        //Name of image to be located.
        var localFile = "temp";

        using(var textractClient = new AmazonTextractClient(RegionEndpoint.EUWest1))
        {
           
            Console.WriteLine("Start document text detection.");
            var startResponse = await textractClient.StartDocumentTextDetectionAsync(new StartDocumentTextDetectionRequest
            {
                DocumentLocation = new DocumentLocation
                {
                    S3Object = new Amazon.Textract.Model.S3Object
                    {
                        Bucket = s3Bucket,
                        Name = "temp"
                    }
                }
            });

            Console.WriteLine($"Job ID: {startResponse.JobId}");

            var getDetectionRequest = new GetDocumentTextDetectionRequest
            {
                JobId = startResponse.JobId
            };

            Console.WriteLine("Poll for detect job complete");

            //Poll till job is no longer inporgress
            GetDocumentTextDetectionResponse getDetectionResponse = null;
            do
            {
                Thread.Sleep(1000);
                getDetectionResponse = await textractClient.GetDocumentTextDetectionAsync(getDetectionRequest);
            } while (getDetectionResponse.JobStatus == JobStatus.IN_PROGRESS);

            Console.WriteLine("Print out results if the job was successful.");

            //If the job succeeded, loop through results and print the detected text
            if(getDetectionResponse.JobStatus == JobStatus.SUCCEEDED)
            {
                do
                {
                    foreach (var block in getDetectionResponse.Blocks)
                    {
                        Console.WriteLine($"Type {block.BlockType}, Text: {block.Text}");
                        blockTypes.Add(block.BlockType);
                        blockText.Add(block.Text);
                        if (block.BlockType == "LINE")
                        {
                            lineText.Add(block.Text);
                        }
                    }

                    //Check to see if there are more pages of data. Break if there isn't.
                    if (string.IsNullOrEmpty(getDetectionResponse.NextToken))
                    {
                        break;
                    }

                    getDetectionRequest.NextToken = getDetectionResponse.NextToken;
                    getDetectionResponse = await textractClient.GetDocumentTextDetectionAsync(getDetectionRequest);

                } while (!string.IsNullOrEmpty(getDetectionResponse.NextToken));
            }
            else
            {
                //If the job failed print error message.
                Console.WriteLine($"Job failed with message {getDetectionResponse.StatusMessage}");
            }
        }
    }

    //This method returns individual words in array format.
    public List<string> GetBlockText()
    {
        return this.blockText;
    }
    //This method returns lines of text as they apprear on the
    //image in array format.
    public List<string> GetLineText()
    { 
        return this.lineText;
    }
 }

