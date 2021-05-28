using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication8.Pages
{
    public class IndexModel : PageModel
    {
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}

        public string? ConnectionString { get; set; }
        public List<BackgroundTask?> BackgroundTasks { get; set; } = null!;   


        public async Task OnGet()
        {
            var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
            ConnectionString = connectionString;

            var result = await Db.GetAllBackgroundTasks(connectionString);
            BackgroundTasks = result;
        }
    }
}
