using Hangfire;
using Hangfire.MemoryStorage;
using Smart_Agriculture_System.BackgroundServices;
using Smart_Agriculture_System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDBContext>();
//builder.Services.AddHostedService<SeedDataHostedService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddTransient<ISencorDataJob, SencorDataJob>();

var flutterAppOrigin = "_myFlutterApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: flutterAppOrigin,
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(flutterAppOrigin);
app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<ISencorDataJob>("ReadSencorDataJob",
    processor => processor.ReadSencorDataAsync(), "*/2 * * * *");  // every 2 minutes
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
