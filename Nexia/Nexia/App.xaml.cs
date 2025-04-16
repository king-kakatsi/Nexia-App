using Newtonsoft.Json;
using Nexia.views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nexia
{
    public partial class App : Application
    {
        // %%%%%%%%%%%%%%%%%%%%% PROPERTIES %%%%%%%%%%%%%%%%%%%%%%%%%%
        private static string attemptHistoryFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "attemptHistoryFile.json");

        public static DateTime EXPIRATION_DATE = DateTime.ParseExact("10-10-2024", "dd-MM-yyyy", null);

        private static bool ATTEMPT_IS_LIMITED = false;
        public static int AttemptCounter { get; set; }
        private static int TOTAL_ATTEMPT = 2;
        // %%%%%%%%%%%%%%%%%%%%% END - PROPERTIES %%%%%%%%%%%%%%%%%%%%%%%%%%





        // %%%%%%%%%%%%%%%%%%%%%% CONSTRUCTOR %%%%%%%%%%%%%%%%%%%%%%%%%%
        public App()
        {
            InitializeComponent();
            GetAttemptHistory();
            if(ATTEMPT_IS_LIMITED && (EXPIRATION_DATE <= DateTime.Now.Date || AttemptCounter >= TOTAL_ATTEMPT))
            {
                MainPage = new LimiteReachedPage();
            }
            else
            {
                MainPage = new NavigationPage(new MainPage());
            }

            //MainPage = new ScannerPage(null);
        }
        // %%%%%%%%%%%%%%%%%%%%%% END - CONSTRUCTOR %%%%%%%%%%%%%%%%%%%%%%%%%%





        // %%%%%%%%%%%%%%%%%%% GET ATTEMPT HISTORY FROM FILE %%%%%%%%%%%%%%%%%%
        private void GetAttemptHistory()
        {
            if (!File.Exists(attemptHistoryFile))
            {
                AttemptCounter = 0;
            }
            else
            {
                try
                {
                    string attemptHistoryJson = File.ReadAllText(attemptHistoryFile);
                    AttemptCounter = int.Parse(attemptHistoryJson);
                }
                catch
                {
                    AttemptCounter = 0;
                }
            }
        }
        // %%%%%%%%%%%%%%%%%%% END - GET ATTEMPT HISTORY FROM FILE %%%%%%%%%%%%%%%%%%





        // %%%%%%%%%%%%%%%%%%%%%% CHECK ATTEMPT COUNTER %%%%%%%%%%%%%%%%%%%%%%%%%%
        /// <summary>
        /// CHeck if total attempt is reached
        /// </summary>
        /// <returns>accessDenied true or false</returns>
        public static bool CheckAttemptCounterAndAccessStatus()
        {
            bool accessDenied = EXPIRATION_DATE <= DateTime.Now.Date || AttemptCounter >= TOTAL_ATTEMPT;
            if (!ATTEMPT_IS_LIMITED)
            {
                return false;
            }
            return accessDenied;
        }
        // %%%%%%%%%%%%%%%%%%%%%% END - CHECK ATTEMPT COUNTER %%%%%%%%%%%%%%%%%%%%%%%%%%





        // %%%%%%%%%%%%%%%%%%%%%%%% SAVE ATTEMPT HISTORY %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public static void UpdateAttemptCounterAndHistory()
        {
            AttemptCounter++;
            try
            {
                File.WriteAllText(attemptHistoryFile, $"{AttemptCounter}");
            }
            catch
            {
                File.WriteAllText(attemptHistoryFile, $"{TOTAL_ATTEMPT}");
            }
        }
        // %%%%%%%%%%%%%%%%%%%%%%%% END - SAVE ATTEMPT HISTORY %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
