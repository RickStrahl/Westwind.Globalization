using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AlbumViewerBusiness;
using Newtonsoft.Json;

namespace Westwind.Globalization.Sample.Controllers
{

    /// <summary>
    /// Simple business object that loads an albums and artists
    /// into a memory based list
    /// </summary>
    public class AlbumsBusiness
    {
        public List<Album> Albums
        {
            get
            {
                if (_albums == null)
                    _albums = LoadAlbums();
                return _albums;
            }
        }

        private List<Album> _albums;

        public List<Artist> Artists
        {
            get
            {
                if (_artists == null)
                    _artists = LoadArtists();
                return _artists;                 
            }            
        }
        private List<Artist> _artists;
        
        
        public List<Album> GetAlbums()
        {
            return Albums.OrderBy(alb => alb.Title).ToList();
        }

        public List<Artist> GetArtists()
        {
            return Artists.OrderBy(art => art.ArtistName).ToList();
        }

        List<Album> LoadAlbums()
        {
            string albumFile = HttpContext.Current.Server.MapPath("~/App_Data/albums.js");

            string json = File.ReadAllText(albumFile);

            return JsonConvert.DeserializeObject<List<Album>>(json);
        }

        List<Artist> LoadArtists()
        {
            string artistFile = HttpContext.Current.Server.MapPath("~/App_Data/artists.js");

            string json = File.ReadAllText(artistFile);

            return JsonConvert.DeserializeObject<List<Artist>>(json);
        }

    }
}