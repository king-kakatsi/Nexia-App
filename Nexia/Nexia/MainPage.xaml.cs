using Nexia.views;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Nexia
{
    public partial class MainPage : ContentPage
    {
        // %%%%%%%%%%%%%%%%%%%% PROPERTIES %%%%%%%%%%%%%%%%%%%%%%%
        public ICommand LottieAnimationCommand { get; set; }
        // %%%%%%%%%%%%%%%%%%%% END - PROPERTIES %%%%%%%%%%%%%%%%%%%%%%%



        // %%%%%%%%%%%%%%%%%% CONSTRUCTOR %%%%%%%%%%%%%%%%
        public MainPage()
        {
            LottieAnimationCommand = new Command(() =>
            {
                SelectPictureOptions();
            });
            BindingContext = this;

            InitializeComponent();
        }
        // %%%%%%%%%%%%%%%%%% END - CONSTRUCTOR %%%%%%%%%%%%%%%%



        protected override void OnAppearing()
        {
            base.OnAppearing();

            bool accessDenied = App.CheckAttemptCounterAndAccessStatus();
            if (accessDenied)
            {
                App.Current.MainPage = new LimiteReachedPage();
                return;
            }
        }





        // %%%%%%%%%%%%%%%%%%%%%% TAKE A PICTURE CLICKED %%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private void SelectPictureOptions()
        {
            // °°°°°°°°°°°°°°°°° ALLOW ACTION ONLY IF CONNECTED TO INTERNET °°°°°°°°°°°
            var networkAccess = Connectivity.NetworkAccess;
            if (networkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Sorry", "You have to be connected to internet.\n\nPlease check your internet connection.", "OK");
                return;
            }
            // °°°°°°°°°°°°°°°°° END - ALLOW ACTION ONLY IF CONNECTED TO INTERNET °°°°°°°°°°°

            pictureOptionsDialog.IsVisible = true;
            pictureOptionsDialog.FadeTo(1, 250, Easing.CubicInOut);
        }


        private void SelectPictureOptions_Clicked(object sender, EventArgs e)
        {
            SelectPictureOptions();
        }
        // %%%%%%%%%%%%%%%%%%%%%% END - TAKE A PICTURE CLICKED %%%%%%%%%%%%%%%%%%%%%%%%%%%%




        // %%%%%%%%%%%%%%%%%% DISMISS PICTURE OPTIONS DIALOG %%%%%%%%%%%%
        private void DismissDialog()
        {
            pictureOptionsDialog.FadeTo(0, 250, Easing.CubicInOut);
            pictureOptionsDialog.IsVisible = false;
        }


        private void DismissDialog_clicked(object sender, EventArgs e)
        {
            DismissDialog();
        }
        // %%%%%%%%%%%%%%%%%% END - DISMISS PICTURE OPTIONS DIALOG %%%%%%%%%%%%





        // %%%%%%%%%%%%%%%%%%%%%%%%%%% TAKE PICTURE %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private async void TakePicture_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "Sorry, no camera available.", "OK");
                return;
            }

            DismissDialog();

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (file == null)
            {
                return;
            }

            await this.Navigation.PushAsync(new ScannerPage(file), false);
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%% END - TAKE PICTURE %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%






        // %%%%%%%%%%%%%%%%%%%%%%%%%%% SELECT PICTURE %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        private async void SelectPicture_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Sorry", "It's not possible to pick picture on this device.", "OK");
                return;
            }

            DismissDialog();

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium });

            if (file == null)
            {
                return;
            }

            await this.Navigation.PushAsync(new ScannerPage(file), false);
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%% END - SELECT PICTURE %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    }
}
