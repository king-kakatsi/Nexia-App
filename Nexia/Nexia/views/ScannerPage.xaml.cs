
using Nexia.models;
using Nexia.services;
using Plugin.Media.Abstractions;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nexia.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        //%%%%%%%%%%%%%%%%%%%%%% PROPERTIES %%%%%%%%%%%%%%%%%%%%%
        private MediaFile imageFile = null;
        private Stream imageStream = null;
        private Stream imageStream2 = null;

        public bool resultIsPending = true;
        public Face DetectionResult { get; set; } = null;

        public int AgeIntervalStartFactor { get; set; } = -6;
        public int AgeIntervalEndFactor { get; set; } = -2;
        public int AgeIntervalStart { get; set; }
        public int AgeIntervalEnd { get; set; }

        public string PreviousSpeechLangageCode { get; set; } = "__none__";
        public SpeechOptions SpeechSettings { get; set; }

        private CancellationTokenSource cts; // Token source for cancellation
        //%%%%%%%%%%%%%%%%%%%%%% END - PROPERTIES %%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%% CONSTRUCTOR %%%%%%%%%%%%%%%%%%%%%%
        public ScannerPage(MediaFile file)
        {
            InitializeComponent();
            cts = new CancellationTokenSource(); // Initialize token source
            imageFile = file;

            // °°°°°°°°°°°°°°°°°°°°°° DUPLICATE THE STREAM TO AVOID CONFLIT °°°°°°°°°°°°°°
            List<Stream> tempList = DuplicateStream(imageFile, 2);
            try
            {
                imageStream = tempList.FirstOrDefault();
                imageStream2 = tempList.LastOrDefault();
            }
            catch { }
            // °°°°°°°°°°°°°°°°°°°°°° END - DUPLICATE THE STREAM TO AVOID CONFLIT °°°°°°°°

            AnalyseDectectionResult(cts.Token); // Pass token
            DisplayImage(cts.Token); // Pass token
            AnimScannerLaserWithSound(cts.Token); // Pass token
        }
        // %%%%%%%%%%%%%%%%%%% END - CONSTRUCTOR %%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%%%%% CANCEL ALL PROCESSES %%%%%%%%%%%%%%%%%%%%%%%
        public void CancelAllProcesses()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel(); // Cancel the token
            }
        }
        // %%%%%%%%%%%%%%%%%%%%%%% END - CANCEL ALL PROCESSES %%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%% DUPLICATE THE STREAM TO AVOID CONFLIT %%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private List<Stream> DuplicateStream(MediaFile file, int duplicationNumber)
        {
            try
            {
                var streamList = new List<Stream>();
                var fileStream = file.GetStreamWithImageRotatedForExternalStorage();

                for (int i = 0; i < duplicationNumber; i++)
                {
                    var duplicatedStream = new MemoryStream();
                    fileStream.Position = 0; // Reset position before each copy
                    fileStream.CopyTo(duplicatedStream);
                    duplicatedStream.Position = 0; // Reset the position of the copied stream
                    streamList.Add(duplicatedStream);
                }

                return streamList;
            }
            catch
            {
                return null;
            }
        }
        // %%%%%%%%%%%%%%%%% END - DUPLICATE THE STREAM TO AVOID CONFLIT %%%%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%%%% DISPLAY THE IMAGE %%%%%%%%%%%%%%%%%%%%%%%%%%
        public async Task DisplayImage(CancellationToken token)
        {
            try
            {
                imageBox.Source = ImageSource.FromStream(() =>
                {
                    return imageStream;
                });
            }
            catch
            {
                if (token.IsCancellationRequested)
                    return; // Cancel display if token requested
            }
        }
        // %%%%%%%%%%%%%%%%%%%%%% END - DISPLAY THE IMAGE %%%%%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%%% ANALYSE DETECTION RESULT %%%%%%%%%%%%%%%%%%%%%%%%
        public async Task AnalyseDectectionResult(CancellationToken token)
        {
            try
            {
                var detectionResult = await CognitiveService.DetectFace(imageStream2, token); // Pass token

                if (token.IsCancellationRequested)
                    return; // Exit if cancellation requested

                resultIsPending = false;
                DetectionResult = detectionResult;

                // Handle if no face is detected
                if (detectionResult == null)
                {
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                // Handle operation canceled gracefully
            }
        }
        // %%%%%%%%%%%%%%%%%%%%% END - ANALYSE DETECTION RESULT %%%%%%%%%%%%%%%%%%%%%%%%



        // %%%%%%%%%%%%%%%%%%%%%%%%% ANIMATE SCANNER LASER WITH SOUND %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private async Task AnimScannerLaserWithSound(CancellationToken token)
        {
            scannerLaser.Opacity = 0;
            await Task.Delay(500, token); // Pass token to delay
            await scannerLaser.FadeTo(1, 500);

            PlaySound("scan.wav");
            var imageHeight = 360;
            await scannerLaser.TranslateTo(0, imageHeight, 1800, Easing.CubicInOut);

            double translationYTarget = 0;

            // Loop until the scan is completed or canceled
            while (resultIsPending && !token.IsCancellationRequested)
            {
                PlaySound("scan.wav", withLoad: false);
                await scannerLaser.TranslateTo(0, translationYTarget, 1800, Easing.CubicInOut);
                translationYTarget = translationYTarget == 0 ? imageHeight : 0;
            }

            if (token.IsCancellationRequested)
            {
                return; // Exit if cancellation requested
            }

            if (DetectionResult == null)
            {
                DisplayResultUI(displayForFailure: true);
            }
            else
            {
                PlaySound("result.wav");
                DisplayResultUI();
            }

            VoiceReadResult(cts.Token); // Pass token
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%% END - ANIMATE SCANNER LASER WITH SOUND %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%%%%%%% PLAY SOUND %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void PlaySound(string soundName, bool withLoad = true)
        {
            ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            try
            {
                if (withLoad)
                {
                    player.Load(GetStreamFromFile(soundName));
                }
                else
                {
                    player.Stop();
                }
                player.Play();
            }
            catch
            {
                player.Stop();
            }
        }

        private Stream GetStreamFromFile(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("Nexia." + filename);
            return stream;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%% END - PLAY SOUND %%%%%%%%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%%%%% DISLAY LAYOUT AFTER RECEIVING RESULT %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void DisplayResultUI(bool displayForFailure = false)
        {
            if (displayForFailure)
            {
                scanMsgLabel.Text = "Scan failed.";
                continueButton.Text = "Retry";
                return;
            }

            scannerLaser.IsVisible = false;
            genderLabel.Text = DetectionResult.attributes.gender.value.ToUpper();

            int age = DetectionResult.attributes.age.value;
            AgeIntervalStart = age + AgeIntervalStartFactor;
            AgeIntervalEnd = age + AgeIntervalEndFactor;


            // °°°°°°°°°°° ADJUSTMENT FOR FEMALE °°°°°°°°°°°°°°°
            if (DetectionResult.attributes.gender.value.ToUpper().Contains("FEMALE"))
            {
                int femaleAgeFactor = 2;
                if (age <= 30)
                {
                    AgeIntervalStart -= femaleAgeFactor;
                    AgeIntervalEnd -= femaleAgeFactor;
                }

                if (age >= 40)
                {
                    AgeIntervalStart += femaleAgeFactor;
                    AgeIntervalEnd += femaleAgeFactor;
                }
            }
            // °°°°°°°°°°° END - ADJUSTMENT FOR FEMALE °°°°°°°°°°°°°°°


            ageLabel.Text = $"{AgeIntervalStart} - {AgeIntervalEnd}";

            detectedAttributesView.IsVisible = true;
            scanMsgLabel.Text = "Scan finished";
            continueButton.Text = "Click to continue";
        }
        // %%%%%%%%%%%%%%%%%%%%%%% END - DISLAY LAYOUT AFTER RECEIVING RESULT %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%% READ RESULT %%%%%%%%%%%%%%%%%%%%%%%%%%
        private async Task VoiceReadResult(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            if (DetectionResult == null)
            {
                await Talk("Human not detected", token);
            }
            else
            {
                await Talk("Human detected", token);

                await Talk("Gender", token);
                await Talk(DetectionResult.attributes.gender.value, token);

                await Talk("Age", token);
                await Talk($"Between {AgeIntervalStart} and {AgeIntervalEnd} years old", token);
            }
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%% END - READ RESULT %%%%%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%%%%%%%%%%%% SPEAK AND READ TEXT %%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private async Task Talk(string text, CancellationToken token, string languageCode = "en")
        {
            if (token.IsCancellationRequested)
                return;

            if (languageCode != PreviousSpeechLangageCode)
            {
                var locales = await TextToSpeech.GetLocalesAsync();

                var locale = locales.Where(l => l.Language.ToLower().Contains("en")).FirstOrDefault();

                SpeechSettings = new SpeechOptions()
                {
                    Volume = .75f,
                    Pitch = .1f,
                    Locale = locale
                };
            }

            await TextToSpeech.SpeakAsync(text, SpeechSettings, cancelToken: token);
            PreviousSpeechLangageCode = languageCode;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%% END - SPEAK AND READ TEXT %%%%%%%%%%%%%%%%%%%%%%%%%%%%



        // %%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTINUE TOWARD HOME %%%%%%%%%%%%%%%%%%%%%
        private void ContinueButton_Clicked(object sender, EventArgs e)
        {
            CancelAllProcesses();  // Cancel ongoing tasks
            this.Navigation.PopAsync();
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%% CONTINUE TOWARD HOME %%%%%%%%%%%%%%%%%%%%%
    }
}
