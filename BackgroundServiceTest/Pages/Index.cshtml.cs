using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackgroundServiceTest;
using BackgroundServiceTest.BackgroundServices;

namespace WebApplication8.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Test2Channel _test2Channel;
        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}

        public string? ConnectionString { get; set; }
        public List<BackgroundTask?> BackgroundTasks { get; set; } = null!;   
        public DateTime Now { get; set; }

        public IndexModel(Test2Channel test2Channel)
        {
            _test2Channel = test2Channel;
        }

        public async Task OnGet()
        {
            var filename = DateTime.Now.ToLongTimeString();
            await _test2Channel.AddFileAsync(filename);

            //var connectionString = AppConfiguration.LoadFromEnvironment().ConnectionString;
            //ConnectionString = connectionString;

            //var result = await Db.GetAllBackgroundTasks(connectionString);
            //BackgroundTasks = result;

            //Now = DateTime.Now;
        }
    }
}
