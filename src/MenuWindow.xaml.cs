using Flurl.Http;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace J_Film_Downloader
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        Movie[] movies;
        readonly GridLengthConverter GL;
        private readonly SolidColorBrush yellow;
        private readonly SolidColorBrush darkGrey;
        private readonly SolidColorBrush white;
        private readonly SolidColorBrush orange;
        private readonly SolidColorBrush green;
        private readonly SolidColorBrush Dblue;
        private readonly SolidColorBrush dgreen;
        private readonly SolidColorBrush purple;
        private readonly SolidColorBrush blue;
        private readonly SolidColorBrush whitesmoke;
        readonly List<SolidColorBrush> colors;
        List<Image> covers;
        readonly IDictionary<int, string> links;
        int Gcounter = 0;
        public MenuWindow(Movie[] m)
        {
            movies = m;
            InitializeComponent();
             GL = new GridLengthConverter();
            yellow   =new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF00"));
            darkGrey = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0c0c0c"));
            white = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            orange = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC7633"));
            green = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#28B463"));
            blue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB"));
            purple = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8E44AD"));
            Dblue = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E"));
            dgreen = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#17A589"));
            whitesmoke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
            colors = new List<SolidColorBrush> { Dblue, purple, orange,green,blue,dgreen};
            links = new Dictionary<int, string>();
            covers = new List<Image>();
            flash();
            Debug.WriteLine("links gathered : "+links.Count);
            Debug.WriteLine("starting image flashing...");
            imageFlasing();

        }

        private void imageFlasing()
        {
            int i = 0;
            foreach (Movie M in movies)
            {
                string url = M.medium_cover_image;
                
                try
                {
                    
                    covers[i].Source = new BitmapImage(new Uri(url));
                    Debug.WriteLine("Flashing - " + M.title);
                }
                catch
                {
                   
                    Debug.WriteLine("Error cannot access/load: "+url);
                }
                i++;
            }
            //covers[3].Source= new BitmapImage(new Uri("trial.jpg", UriKind.Relative) );
        }

        public void flash()
        {

            foreach (Movie M in movies){

                //
               // Debug.WriteLine(M.url + "," + M.medium_cover_image);
                //
                Card card=new Card();
                card.Height = 220; card.Width = 1100;
                card.Background = whitesmoke;
                Grid grid = getGrid();
        
                grid.Children.Add(getHeader(M));
               
                grid.Children.Add(getRating(M));

                grid.Children.Add(getRuntime(M));

               
                grid.Children.Add(GetGenre(M));

                Separator sep = new Separator
                {
                    Style = TryFindResource("ToolBar.SeparatorStyleKey") as Style
                };
                Grid.SetRow(sep, 4); Grid.SetColumn(sep, 2);
                sep.Margin = new Thickness(2, 0, 10, 0);

                grid.Children.Add(ImageTemplater(M));

                grid.Children.Add(sep);
                grid.Children.Add(getTorrents(M));

                //

                card.Content = grid;
                stack.Children.Add(card);
                
            }
        }

        private Image ImageTemplater(Movie m)
        {
            Image img = new Image();
            img.Width = 140; img.Height = 210;
            img.Margin = new Thickness(0, 3, 0,8);
            Binding b = new Binding();
            b.IsAsync = true;
            b.Source = img;
            Grid.SetRow(img, 0);Grid.SetColumn(img, 0); Grid.SetRowSpan(img,6);
            covers.Add(img);
            return img;
            
        }

        private StackPanel getTorrents(Movie M)
        {
            Torrent[] torrents = M.torrents;
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            foreach(Torrent torrent in torrents)
            {
                stack.Children.Add(getTorrentButton(torrent));
            }

            Grid.SetRow(stack,5); Grid.SetColumn(stack, 2);
            stack.VerticalAlignment = VerticalAlignment.Bottom;
            stack.Margin = new Thickness(0, 0, 0, 10);
            return stack;
        }

        private Badged getTorrentButton(Torrent torrent)
        {
            Badged badge = new Badged();
            string txt = torrent.quality + " " + torrent.type;
            badge.Badge = torrent.size;
            badge.BadgeColorZoneMode = ColorZoneMode.Dark;
            Button btn = new Button();
            Gcounter++;
            btn.Tag = Gcounter;
            links.Add(Gcounter, torrent.url);
            ButtonAssist.SetCornerRadius(btn, new CornerRadius(15));
            
            btn.Content = txt;
            btn.FontSize = 10;
            
            btn.Click += new RoutedEventHandler(buttonOnClick);
            badge.Content = btn;
            badge.Margin = new Thickness(1, 10, 30, 1);
            return badge;
        }

        private void goBackButton(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            Close();

        }

        private void buttonOnClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            ButtonProgressAssist.SetIsIndicatorVisible(button,true);
            ButtonProgressAssist.SetIsIndeterminate(button, true);
            int i =(int) button.Tag;
            Debug.WriteLine(i+"-"+links[i]);
            downloadAndOpen(links[i],button);
        }

        private async void  downloadAndOpen(string v,Button button)
        {
            var path = await v.DownloadFileAsync(AppDomain.CurrentDomain.BaseDirectory, "filed.torrent");
            ButtonProgressAssist.SetIsIndicatorVisible(button, false);
            ButtonProgressAssist.SetIsIndeterminate(button, false);
            Debug.WriteLine("Path : "+path);
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch
            {
                Debug.WriteLine("Error");
                MessageBox.Show("Make sure any Torrent client is installed");
            }
        }

        public TextBlock getHeader(Movie M)
        {
            TextBlock texty = new TextBlock();
            texty.Style = TryFindResource("MaterialDesignHeadline5TextBlock") as Style;
            string Header = M.title + " ( " + M.year + " )";
            texty.Text = Header;
            Grid.SetRow(texty, 0); Grid.SetColumn(texty, 2);
            return texty;
        }

        public StackPanel getRating(Movie M)
        {
            StackPanel rater = new StackPanel();
            rater.Orientation = Orientation.Horizontal;
            RatingBar rating = new RatingBar();
            rating.VerticalAlignment = VerticalAlignment.Center;

            rating.Max = 10;
            rating.Value = (int)M.rating;
            Label label = new Label();
            label.Content = "RATING : ";
            label.FontWeight = FontWeights.Bold;
            label.FontSize = 12;
            label.FontFamily = new FontFamily("Monospace");
            label.VerticalAlignment = VerticalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            rater.Children.Add(label);
            rater.Children.Add(rating);

            label = new Label();
            label.Content = "IMDB : " + M.rating;
            label.FontSize = 11;
            label.FontFamily = new FontFamily("Comic Sans MS");
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Foreground = yellow;
            label.Margin = new Thickness(5,2,5,2);
            Border border = new Border();
            border.VerticalAlignment = VerticalAlignment.Center;
            border.CornerRadius = new CornerRadius(15);
            border.BorderBrush = darkGrey;
            border.Background = darkGrey; // left,top,right,bottom
            border.Margin= new Thickness(10, 1, 0,1);
            border.Child = label;
            rater.Children.Add(border);
            Grid.SetRow(rater, 1); Grid.SetColumn(rater, 2);
            rater.VerticalAlignment = VerticalAlignment.Center;
            return rater;

        }
        Border createGenreLabel(string s,int i)
        {
            Label label = new Label();
            label.Content = s;
            label.FontSize = 11;
            label.FontFamily = new FontFamily("Abadi MT");
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Foreground = white;
            label.Margin = new Thickness(5, 2, 5, 2);
            Border border = new Border();
            border.VerticalAlignment = VerticalAlignment.Center;
            border.CornerRadius = new CornerRadius(8);
            border.BorderBrush = colors[i%6];
            border.Background = colors[i%6];// left,top,right,bottom
            border.Margin = new Thickness(10, 1, 0, 1);
            border.Child = label;
            return border;
        }

        public StackPanel GetGenre(Movie M)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            int i = 0;
            foreach(string genre in M.genres)
            {
                stack.Children.Add(createGenreLabel(genre,i));
                i++;
            }
            Grid.SetRow(stack, 3); Grid.SetColumn(stack, 2);
            stack.VerticalAlignment = VerticalAlignment.Center;
            return stack;

        }
        public TextBlock getRuntime(Movie M)
        {
            int hours = M.runtime / 60, minutes = M.runtime % 60;
            String c = "Runtime : " + hours + " Hour";
            if (hours != 1) c += "s";
            c += " " + minutes + " Minutes";
            TextBlock text = new TextBlock
            {
                Style = TryFindResource("MaterialDesignSubtitle1TextBlock") as Style
            };
            
            text.Text = c;
            Grid.SetRow(text, 2); Grid.SetColumn(text, 2);
            text.VerticalAlignment = VerticalAlignment.Center;
            return text;
        }
        public Grid getGrid()
        {
            Grid grid = new Grid();
            RowDefinition rowDefinition = new RowDefinition
            {
                Height = (GridLength)GL.ConvertFrom("40")
            };
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition
            {
                Height = (GridLength)GL.ConvertFrom("45")
            };
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition
            {
                Height = (GridLength)GL.ConvertFrom("36")
            };
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition
            {
                Height = (GridLength)GL.ConvertFrom("42")
            };
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition
            {
                Height = (GridLength)GL.ConvertFrom("2")
            };
            grid.RowDefinitions.Add(rowDefinition);
            rowDefinition = new RowDefinition
            {
                Height = (GridLength)GL.ConvertFrom("60")
            };
            grid.RowDefinitions.Add(rowDefinition);


            ColumnDefinition colDefinition = new ColumnDefinition
            {
                Width = (GridLength)GL.ConvertFrom("150")
            };
            grid.ColumnDefinitions.Add(colDefinition);
            colDefinition = new ColumnDefinition
            {
                Width = (GridLength)GL.ConvertFrom("10")
            };
            grid.ColumnDefinitions.Add(colDefinition);
            colDefinition = new ColumnDefinition
            {
                Width = (GridLength)GL.ConvertFrom("*")
            };
            grid.ColumnDefinitions.Add(colDefinition);
            return grid;

        }
        
    }
}
