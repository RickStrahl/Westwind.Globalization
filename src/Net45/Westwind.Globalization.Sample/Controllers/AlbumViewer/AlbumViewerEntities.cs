using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace AlbumViewerBusiness
{

    public class AlbumViewerSampleData
    {
        public static List<Album> LoadAlbums()
        {
            string albumFile = HttpContext.Current.Server.MapPath("~/App_Data/albums.js");

            string json = File.ReadAllText(albumFile);

            return JsonConvert.DeserializeObject<List<Album>>(json);
        }

        public static List<Artist> LoadArtists()
        {
            string artistFile = HttpContext.Current.Server.MapPath("~/App_Data/artists.js");

            string json = File.ReadAllText(artistFile);

            return JsonConvert.DeserializeObject<List<Artist>>(json);
        }



    }


    public class Album
    {
       
        public int Id { get; set; }

        public int ArtistId { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public string ImageUrl { get; set; }
        public string AmazonUrl { get; set; }


        public virtual Artist Artist { get; set; }
        public virtual IList<Track> Tracks { get; set; }

        public Album()
        {
            Artist = new Artist();
            Tracks = new List<Track>();
        }

    }


    public class Artist
    {
        public int Id { get; set; }

        public string ArtistName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string AmazonUrl { get; set; }

        //public List<Album> Albums { get; set; }
    }

    
    public class Track
    {
        public int Id { get; set; }

        public int AlbumId { get; set; }
        
        public int ArtistId { get; set; }
        public string SongName { get; set; }
        public string Length { get; set; }
        public int Bytes { get; set; }
        public decimal UnitPrice { get; set; }

        public override string ToString()
        {
            return SongName;
        }
    }
}