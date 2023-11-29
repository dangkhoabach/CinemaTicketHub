using CinemaTicketHub.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaTicketHub.API_Calling;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace CinemaTicketHub.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();
        LanguageManager languageManager = new LanguageManager();

        public async Task<ActionResult> Index()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://api.themoviedb.org/3/movie/now_playing?api_key={APIKey.Key}&language=vi-VN&region=VN";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic moviesData = JsonConvert.DeserializeObject(data);
                    List<Movie> NSmovies = new List<Movie>();

                    foreach (var item in moviesData.results)
                    {
                        string releaseDate = item.release_date;
                        DateTime parsedReleaseDate;
                        if (DateTime.TryParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedReleaseDate))
                        {
                            releaseDate = parsedReleaseDate.ToString("dd/MM/yyyy");
                        }

                        Movie movie = new Movie
                        {
                            MovieID = item.id,
                            Title = item.title,
                            Overview = item.overview,
                            PosterPath = "https://image.tmdb.org/t/p/w500" + item.poster_path,
                            ReleaseDate = releaseDate,
                            Language = languageManager.GetLanguageName(item["original_language"].ToString())
                        };
                        NSmovies.Add(movie);
                    }
                    ViewBag.NowShowing = NSmovies;
                }
                else
                {
                    return View("Error");
                }
            }

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://api.themoviedb.org/3/movie/upcoming?api_key={APIKey.Key}&language=vi-VN&region=VN";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic moviesData = JsonConvert.DeserializeObject(data);
                    List<Movie> UCmovies = new List<Movie>();

                    foreach (var item in moviesData.results)
                    {
                        string releaseDate = item.release_date;
                        DateTime parsedReleaseDate;
                        if (DateTime.TryParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedReleaseDate))
                        {
                            releaseDate = parsedReleaseDate.ToString("dd/MM/yyyy");
                        }

                        Movie movie = new Movie
                        {
                            MovieID = item.id,
                            Title = item.title,
                            Overview = item.overview,
                            PosterPath = "https://image.tmdb.org/t/p/w500" + item.poster_path,
                            ReleaseDate = releaseDate,
                            Language = languageManager.GetLanguageName(item["original_language"].ToString())
                        };
                        UCmovies.Add(movie);
                    }
                    ViewBag.Upcoming = UCmovies;                 
                }
                else
                {
                    return View("Error");
                }
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}