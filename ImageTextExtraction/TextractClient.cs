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
	List<BlockType> blockTypes = new List<BlockType>();
    List<string> blockText = new List<string>();
    List<string> lineText = new List<string>();

    //static async Task Main(string[] args)
    //{
    //    try {

    //        await StartDetectAsync();
    //    } 
    //    catch(Exception e)
    //    {
    //        Console.WriteLine(e.Message);
    //    }
    //}

    public async Task StartDetectAsync()
    {
        //My bucket on aws.amazon
        var s3Bucket = "oisinsapps";

        //Image to be uploaded to S3 for textract processing
        var localFile = "temp";

        using(var textractClient = new AmazonTextractClient(RegionEndpoint.EUWest1))
        //using(var s3Client = new AmazonS3Client(RegionEndpoint.EUWest1))
        {
            //Console.WriteLine($"Upload {localFile} to {s3Bucket} bucket");
            //var putRequest = new PutObjectRequest
            //{
            //    BucketName = s3Bucket,
            //    FilePath = localFile,
            //    Key = Path.GetFileName(localFile)
            //};
            //await s3Client.PutObjectAsync(putRequest);

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
                Console.WriteLine($"Job failed with message {getDetectionResponse.StatusMessage}");
            }
        }
    }

    public List<string> getBlockText()
    {
        return this.blockText;
    }

    public List<string> getLineText()
    {
        return this.lineText;
    }
 }

