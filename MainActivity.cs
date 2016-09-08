using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using static Android.App.ActionBar;
using System.Threading;

namespace RALConfigAndroidApp
{
    [Activity(Label = "RALConfigAndroidApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        private ProgressBar pb1;
        private int progress = 1;
        public System.Timers.Timer timer;
        int dynamicControleID = 99;
        string baseURL = "http://10.0.2.2:8086/Config/";
        string GlobalUser = string.Empty;
        private Handler handler = new Handler();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            EditText et = (EditText)FindViewById<EditText>(Resource.Id.PhoneNumberText);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            TextView nameView = FindViewById<TextView>(Resource.Id.NameView);
            nameView.SetTextColor(Android.Graphics.Color.Red);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.CallButton);
            pb1 = (ProgressBar)FindViewById(Resource.Id.pb1);
            pb1.Visibility = ViewStates.Invisible;

            button.Click += async (sender, e) =>
            {
                pb1.Visibility = ViewStates.Visible;
                pb1.Progress = 30;
                EditText edtxt = (EditText)FindViewById<EditText>(Resource.Id.PhoneNumberText);
                string url = "http://192.168.4.7:8145/api/demo/";   //+ edtxt.Text;
                JsonValue json = await GetUserCredentials(url);
                json = "#" + json;
                pb1.Progress = 98;
                //string userName = json["SmtpUsername"];
                nameView.Text = json;
                pb1.Visibility = ViewStates.Invisible;
                var layout = (LinearLayout)FindViewById(Resource.Id.the_big_layout);
                layout.SetBackgroundColor(Android.Graphics.Color.ParseColor(json));

            };

            timer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromSeconds(20).TotalMilliseconds,
                AutoReset = true

            };
            timer.Elapsed += async (sender, e) =>
            {
                EditText etPhoneHome = (EditText)FindViewById<EditText>(Resource.Id.PhoneNumberText);
               // if (etPhoneHome.Text != String.Empty)
               // {
                    string url = "http://192.168.4.7:8145/api/demo/";

                    JsonValue json = await GetUserCredentials(url);
                    json = "#" + json;
                    //GlobalUser = json["SmtpUsername"];
                    Console.Out.WriteLine("USer color is: " + json);
                    //etPhoneHome.SetTextColor(Android.Graphics.Color.Blue);
                   // if (etPhoneHome != null)
                   // {
                        RunOnUiThread(() =>
                        {
                            nameView.SetTextColor(Android.Graphics.Color.Blue);
                            nameView.Text = json;
                            var layout = (LinearLayout)FindViewById(Resource.Id.the_big_layout);
                            layout.SetBackgroundColor(Android.Graphics.Color.ParseColor(json));
                        });

                   // }
              //  }
                //timer.Stop();

            };
          //  AddTextViews();

            timer.Start();
        }

        private bool AddTextViews()
        {
            var layout = (LinearLayout)FindViewById(Resource.Id.the_big_layout);
            for (int i = 0; i < 10; i++)
            { 
                TextView tv = new TextView(this);
                tv.Text = "Placeholder";
                this.dynamicControleID++;
                tv.Id = this.dynamicControleID;
                tv.LayoutParameters = (new LayoutParams(
               ViewGroup.LayoutParams.FillParent,
               LayoutParams.WrapContent));

                layout.AddView(tv);
            }
            return true;
        }

        private async Task<JsonValue> GetUserCredentials(string url)
        {
            System.Net.HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            using  (WebResponse response = await request.GetResponseAsync())
            {
                using (System.IO.Stream stream = response.GetResponseStream())
                {
                    try
                    {
                        JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                        Console.Out.WriteLine("Response: {0}, ", jsonDoc.ToString());

                        return jsonDoc;
                    }
                    catch (System.Exception ex)
                    {
                        Console.Out.WriteLine(ex.StackTrace);
                        return null;
                    }
                    
                }
            }
           // return null;
        }


    }
}

