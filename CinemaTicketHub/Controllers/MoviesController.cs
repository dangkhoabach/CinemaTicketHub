using CinemaTicketHub.API_Calling;
using CinemaTicketHub.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Controllers
{
    public class MoviesController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();
        LanguageManager languageManager = new LanguageManager();

        // GET: Movies
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> NowShowing()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://api.themoviedb.org/3/movie/now_playing?api_key={APIKey.Key}&language={APIKey.Language}&region={APIKey.Region}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic moviesData = JsonConvert.DeserializeObject(data);
                    List<Movie> movies = new List<Movie>();

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
                        movies.Add(movie);
                    }
                    return View(movies);
                }
                else
                {
                    return View("Error");
                }
            }
        }

        public async Task<ActionResult> Upcoming()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://api.themoviedb.org/3/movie/upcoming?api_key={APIKey.Key}&language={APIKey.Language}&region={APIKey.Region}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic moviesData = JsonConvert.DeserializeObject(data);
                    List<Movie> movies = new List<Movie>();

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
                        movies.Add(movie);
                    }
                    return View(movies);
                }
                else
                {
                    return View("Error");
                }
            }
        }

        public async Task<ActionResult> Detail(int id, DateTime? selectedDate)
        {
            string apiUrl = $"https://api.themoviedb.org/3/movie/{id}?api_key={APIKey.Key}&language={APIKey.Language}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic movieData = JsonConvert.DeserializeObject(data);

                    string releaseDate = movieData.release_date;
                    DateTime parsedReleaseDate;
                    if (DateTime.TryParseExact(releaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedReleaseDate))
                    {
                        releaseDate = parsedReleaseDate.ToString("dd/MM/yyyy");
                    }

                    var genresArray = movieData.genres as JArray;
                    List<string> genres = genresArray?.Select(genre => genre["name"].ToString()).ToList() ?? new List<string>();

                    MovieDetail movieDetail = new MovieDetail
                    {
                        Title = movieData.title,
                        Overview = movieData.overview,
                        PosterPath = "https://image.tmdb.org/t/p/w500" + movieData.poster_path,
                        ReleaseDate = releaseDate,
                        Genres = genres,
                        Certification = movieData.certification,
                        Trailer = GetTrailerEmbedUrl(id),
                        Duration = movieData.runtime,
                        Language = languageManager.GetLanguageName(movieData["original_language"].ToString())
                    };

                    //Chọn ngày hiện suất chiếu
                    if (selectedDate.HasValue)
                    {
                        DateTime selectedDateTruncated = selectedDate.Value.Date;

                        ViewBag.SuatChieu = _dbContext.SuatChieu.Where(m => m.MaPhim == id && DbFunctions.TruncateTime(m.NgayChieu) == selectedDateTruncated).ToList();
                    }
                    else
                    {
                        ViewBag.SuatChieu = _dbContext.SuatChieu.Where(m => m.MaPhim == id && DbFunctions.TruncateTime(m.NgayChieu) == DateTime.Today).ToList();
                    }
                    return View(movieDetail);
                }
                else
                {
                    return View("Error");
                }
            }
        }

        private string GetTrailerEmbedUrl(int movieId)
        {
            string apiUrl = $"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key={APIKey.Key}&language={APIKey.Language}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    dynamic videosData = JsonConvert.DeserializeObject(data);

                    // Lặp qua danh sách video và trả về URL nhúng trailer nếu có
                    foreach (var video in videosData.results)
                    {
                        if (video.type == "Trailer" && video.site == "YouTube")
                        {
                            return $"https://www.youtube.com/embed/{video.key}";
                        }
                    }
                }
                // Nếu không tìm thấy trailer hoặc có lỗi, trả về một URL nhúng mặc định hoặc thông báo lỗi
                return "https://www.youtube.com/embed/default-trailer-key"; // Hoặc trả về URL nhúng mặc định khác hoặc thông báo lỗi
            }
        }
    }
}