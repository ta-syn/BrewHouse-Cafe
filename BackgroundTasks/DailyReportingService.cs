using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CafeManagement.Services;
using System.Text;

namespace CafeManagement.BackgroundTasks
{
    public class DailyReportingService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DailyReportingService> _logger;

        public DailyReportingService(IServiceProvider services, ILogger<DailyReportingService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Daily Reporting Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow.AddHours(6);
                var reportTime = new DateTime(now.Year, now.Month, now.Day, 23, 0, 0);

                if (now > reportTime) reportTime = reportTime.AddDays(1);

                var delay = reportTime - now;
                _logger.LogInformation($"Next Daily Summary will be generated in {delay.TotalHours:F1} hours.");

                // 🛡️ Safety check for Task.Delay
                var delayMs = (int)delay.TotalMilliseconds;
                if (delayMs <= 0) delayMs = 1000; // Minimum 1 second delay

                try 
                {
                    await Task.Delay(delayMs, stoppingToken);
                }
                catch (TaskCanceledException) { break; }

                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var dashboardService = scope.ServiceProvider.GetRequiredService<DashboardService>();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                        decimal todayRevenue = await dashboardService.GetTodayRevenueAsync();
                        int todayOrders = await dashboardService.GetTodayOrdersAsync();

                        string summary = $"--- BREWHOUSE CAFE DAILY SUMMARY ---\n" +
                                         $"Date: {DateTime.UtcNow.AddHours(6):dd MMM yyyy}\n" +
                                         $"Total Orders: {todayOrders}\n" +
                                         $"Total Revenue: ৳{todayRevenue}\n" +
                                         $"------------------------------------";

                        await emailService.SendEmailAsync("owner@brewhouse.com", "Daily Closing Summary", summary);
                        _logger.LogInformation("Daily summary email sent successfully.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in DailyReportingService: {ex.Message}");
                }
            }
        }
    }
}
