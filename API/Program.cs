using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);

// Replace default logging with Serilog
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext());

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg1NjI4ODAwIiwiaWF0IjoiMTc1NDE0ODA3OCIsImFjY291bnRfaWQiOiIwMTk4NmI1ZjBjYjM3OTQ4YTU3Njk3OGUxMTFhNDQ3MiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazFubnoxMDhiMmt5cnk4NmdkM2NobWRzIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.mAqEt3aE_41HmU-GtXTtTI0cgIAOdS2Mhz8F5U_2RpTXkvmZUC1MUJY8hPAYz90qidxTpbJCVgAccCBMXoTc86_XHO6hDNmXpa95FY3sRPT626YyIxAWOazc8zhLKsqSnDwBwjtmdJhVoibseuxVHG5tRzgPp1q24u4r99YLFzdW_QIUJ2X7MP-U9v9QMWH2oiA4fO5NKi1MF35vfj-F0OxCOMXwdxt5QqtdjOTfBmZUttKUdill4AY3-Y8LjY98lE3mKTdXMo5Q6uLJoUHklCPIOI0GZ_J8D0vleEnB0m1j5X0WZm6F6SavLtrXkjA9jm-U1iWUhj_R6dtKy4Kn3Q",
    typeof(Program).Assembly);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

await ApplyMigrationsAsync(app);

// Request logging (HTTP method/path/status + duration)
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    const int maxAttempts = 10;
    var delay = TimeSpan.FromSeconds(5);

    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ScoreDbContext>();

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Applying database migrations. Attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(
                ex,
                "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelaySeconds} seconds",
                attempt,
                maxAttempts,
                delay.TotalSeconds);

            await Task.Delay(delay);
        }
    }

    logger.LogInformation("Applying database migrations. Final attempt {MaxAttempts}/{MaxAttempts}", maxAttempts, maxAttempts);
    logger.LogInformation("CI CD also works");
    await dbContext.Database.MigrateAsync();
}

static void LoadDotEnv()
{
    foreach (var path in GetDotEnvPaths())
    {
        if (!File.Exists(path))
        {
            continue;
        }

        foreach (var line in File.ReadAllLines(path))
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = trimmedLine.IndexOf('=');

            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = trimmedLine[..separatorIndex].Trim();
            var value = trimmedLine[(separatorIndex + 1)..].Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        return;
    }
}

static IEnumerable<string> GetDotEnvPaths()
{
    var currentDirectory = Directory.GetCurrentDirectory();

    yield return Path.Combine(currentDirectory, ".env");
    yield return Path.Combine(currentDirectory, "..", ".env");
}
