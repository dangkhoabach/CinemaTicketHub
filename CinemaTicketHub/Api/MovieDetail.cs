using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaTicketHub.Api
{
    public class MovieDetail
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string ReleaseDate { get; set; }
        public List<string> Genres { get; set; }
        public string Certification { get; set; }
        public string Trailer { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; }
    }
}