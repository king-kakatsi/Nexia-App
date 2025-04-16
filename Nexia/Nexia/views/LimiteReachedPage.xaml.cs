using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Nexia.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LimiteReachedPage : ContentPage
	{
		public LimiteReachedPage ()
		{
            InitializeComponent ();
            if (App.EXPIRATION_DATE <= DateTime.Now.Date)
            {
                noRunAppMessage.Text = "Oops! This version of the App has expired.";
            }
        }
	}
}