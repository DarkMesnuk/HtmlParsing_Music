using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

namespace HtmlParsing_Music
{
    public class PlaylistSong
    {
        public string name { get; set; }
        public string artist { get; set; }
        public string likes { get; set; }
        public string image { get; set; }

        public PlaylistSong(string _name, string _artist, string _likes, string _image)
        {
            name = _name;
            artist = _artist;
            likes = _likes;
            image = _image;
        }
        public PlaylistSong()
        {

        }
    }
    class Program
    {
        public static List<PlaylistSong> Playlist = new List<PlaylistSong>();
        static void Main(string[] args)
        {
            Parsing();
        }

        public static string playlist_name = "";
        public static void Parsing()
        {
            string url = "https://rockbot.com/playlists/43387/new-on-rockbot";
            try
            {
                using (HttpClientHandler hd1 = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None })
                {
                    using (var clnt = new HttpClient(hd1))
                    {
                        using (HttpResponseMessage resp = clnt.GetAsync(url).Result)
                        {
                            if (resp.IsSuccessStatusCode)
                            {
                                var html = resp.Content.ReadAsStringAsync().Result;

                                if (!string.IsNullOrEmpty(html))
                                {
                                    HtmlDocument doc = new HtmlDocument();
                                    doc.LoadHtml(html);
                                    var playlist_name = doc.DocumentNode.SelectNodes(".//div[@class='header header-hero']//div[@class='container']//div[@class='row center']//h1")[0].InnerText;

                                    Console.WriteLine(playlist_name + "\n\n\n");

                                    var list = doc.DocumentNode.SelectNodes(".//div[@class='container']//div[@class='row content']//div[@class='col-md-6']//ul[@class='list-small']//li[@class]");
                                    if (list != null && list.Count > 0)
                                    {

                                        foreach (var song in list)
                                        {
                                            string name = song.SelectSingleNode(".//section//h3[@class='list-preview-song']").InnerText;
                                            string artist = song.SelectSingleNode(".//section//p").InnerText;
                                            string likes = song.SelectSingleNode(".//section[@class='list-small-details hidden-md-down']//i[@class='likes']").InnerText;
                                            string image = Remove(song.SelectSingleNode(".//img[@class='artwork']").OuterHtml);

                                            Console.WriteLine(name + "\n" + artist + "\n" + likes + "\n" + image + "\n");

                                            try
                                            {
                                                Playlist.Add(new PlaylistSong(name, artist, likes, image));
                                            }
                                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No songs");
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private static string Remove(string value)
        {
            var arr = value.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string img = Convert.ToString(arr[4]);
            char[] RemoveChar = { '"', '>' };
            img = img.TrimEnd(RemoveChar);
            img = img.Remove(0, 5);
            return img;
        }
    }
}