
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using Flurl.Http;
using Flurl;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;


namespace J_Film_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        readonly string link = "https://yts.mx/api/v2/list_movies.json";
        bool DEVELOPER = false;


        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("Started...");
            if (DEVELOPER)
            {
               //
            }
          

        }

       

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string name = filmNameGetter.Text;
                Debug.WriteLine(name);
                fetch(name);
            }
        }
        async void fetch(string name)
        {
            spinner.Visibility = Visibility.Visible;
            
        
            Debug.WriteLine("fetching");
            try
            {
                Url url = new Url(link).SetQueryParam("query_term", name).SetQueryParam("with_images", "true");
                SearchResult res = await url.WithTimeout(5).GetJsonAsync<SearchResult>();
                if (res.data.movie_count == 0)
                {
                    dText.Text = "No results for : ' " + name + " '";
                    DialogHost.IsOpen = true;
                    spinner.Visibility = Visibility.Hidden;
                    return;

                }
                Movie[] movies = res.data.movies;
                Debug.WriteLine(res.data.movie_count);
               
                MenuWindow window = new MenuWindow(movies);
                spinner.Visibility = Visibility.Hidden;
                window.Show();
                Close();
            }
            catch (FlurlHttpException)
            {
                dText.Text = "Connect VPN. you may turn it off before starting download";
                DialogHost.IsOpen = true;
                spinner.Visibility = Visibility.Hidden;
                Debug.WriteLine("Error");
            }


        }

        private void mouseEntered(object sender, MouseEventArgs e)
        {
            goButton.Foreground = new SolidColorBrush(Colors.White);
        }

        private void mouseLeaved(object sender, MouseEventArgs e)
        {
            goButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff0959a8"));
        }

        private void searchButtonClicked(object sender, MouseButtonEventArgs e)
        {
            string name = filmNameGetter.Text;
            Debug.WriteLine(name);
            fetch(name);
        }




        
    }
}
