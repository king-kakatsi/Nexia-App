using Newtonsoft.Json;
using Nexia.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Nexia.services
{
    public static class CognitiveService
    {
        // %%%%%%%%%%%%%%%%%%%%%%%% PROPERTIES %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // Use SecretKeys for API credentials
        // %%%%%%%%%%%%%%%%%%%%%%%% END - PROPERTIES %%%%%%%%%%%%%%%%%%%%%%%%%%%%%






        // %%%%%%%%%%%%%%%%% DETECT FACE WITH CANCELLATION %%%%%%%%%%%%%%%%%%%%%%%%%%
        public static async Task<Face> DetectFace(Stream imageStream, CancellationToken token)
        {
            // Build the URL with the API key and secret as query parameters
            var url = $"https://api-us.faceplusplus.com/facepp/v3/detect?api_key={SecretKeys.API_KEY}&api_secret={SecretKeys.API_SECRET}";

            if (imageStream == null)
                return null;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var form = new MultipartFormDataContent();

                    // Include return attributes (age, gender)
                    form.Add(new StringContent("age,gender"), "return_attributes");

                    // Add image stream to form data
                    var streamContent = new StreamContent(imageStream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    form.Add(streamContent, "image_file", "image.jpg");

                    // Send POST request with cancellation token
                    var response = await httpClient.PostAsync(url, form, token);

                    // Update attempt counter and history
                    App.UpdateAttemptCounterAndHistory();

                    // If the operation is canceled, return early
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Request canceled.");
                        return null;
                    }

                    // Read the response as a string
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var detectFaceResult = JsonConvert.DeserializeObject<DetectFaceResponse>(jsonResponse);

                        // If faces are detected, return the first one
                        if (detectFaceResult.faces != null && detectFaceResult.faces.Count > 0)
                        {
                            var scannedFace = detectFaceResult.faces[0];
                            return scannedFace;  // Return first detected face result
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {jsonResponse}");
                    }

                    return null;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Operation canceled.");
                    return null; // Handle the cancellation gracefully
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }
            }
        }
        // %%%%%%%%%%%%%%%%% END - DETECT FACE WITH CANCELLATION %%%%%%%%%%%%%%%%%%%%%%%%%%






        // %%%%%%%%%%%%%%%%%%%%% READ STREAM ( transform file into byte[] ) %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private static byte[] ReadStream(Stream input)
        {
            BinaryReader b = new BinaryReader(input);
            byte[] data = b.ReadBytes((int)input.Length);

            return data;
        }
        // %%%%%%%%%%%%%%%%%%%%% END - READ STREAM ( transform file into byte[] ) %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }
}
