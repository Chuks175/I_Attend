using I_Attend.Data;
using I_Attend.Models;
using Microsoft.AspNetCore.Mvc;

namespace I_Attend.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;
        private readonly I_AttendDAO _context;
        private readonly ILogger<EmailController> _logger;

        public EmailController(I_AttendDAO context, ILogger<EmailController> logger, EmailService emailService) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = new EmailService();
        }

        [HttpGet]
        public IActionResult EmailSender()
        {
            var dt = _context.GetData();

            var model = new ReportViewModel
            {
                ReportData = dt
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SendReport(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dt = _context.GetData();
                var csv = CsvHelper.DataTableToCsv(dt);

                _emailService.SendEmailWithCsv(
                    model.SenderEmail,
                    model.SenderPassword,
                    model.RecipientEmail,
                    "Database Report",
                    "Please find the attached database report in CSV format.",
                    csv
                );

                TempData["Message"] = "CSV report emailed successfully!";
                return RedirectToAction("EmailSender");
            }

            TempData["Error"] = "Please fill in all fields.";
            return RedirectToAction("EmailSender");
        }
    }
}
