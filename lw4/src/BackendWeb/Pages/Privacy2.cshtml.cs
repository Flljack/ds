using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace aspnetcoreapp.Pages
{
    public class Privacy2Model : PageModel
    {
        private readonly ILogger<Privacy2Model> _logger;

        public Privacy2Model(ILogger<Privacy2Model> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
