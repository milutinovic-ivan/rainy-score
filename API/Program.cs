using Application.Intefraces;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg1NjI4ODAwIiwiaWF0IjoiMTc1NDE0ODA3OCIsImFjY291bnRfaWQiOiIwMTk4NmI1ZjBjYjM3OTQ4YTU3Njk3OGUxMTFhNDQ3MiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazFubnoxMDhiMmt5cnk4NmdkM2NobWRzIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.mAqEt3aE_41HmU-GtXTtTI0cgIAOdS2Mhz8F5U_2RpTXkvmZUC1MUJY8hPAYz90qidxTpbJCVgAccCBMXoTc86_XHO6hDNmXpa95FY3sRPT626YyIxAWOazc8zhLKsqSnDwBwjtmdJhVoibseuxVHG5tRzgPp1q24u4r99YLFzdW_QIUJ2X7MP-U9v9QMWH2oiA4fO5NKi1MF35vfj-F0OxCOMXwdxt5QqtdjOTfBmZUttKUdill4AY3-Y8LjY98lE3mKTdXMo5Q6uLJoUHklCPIOI0GZ_J8D0vleEnB0m1j5X0WZm6F6SavLtrXkjA9jm-U1iWUhj_R6dtKy4Kn3Q", typeof(Program));

var app = builder.Build();


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
