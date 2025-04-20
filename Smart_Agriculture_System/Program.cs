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
builder.Services.AddTransient<ISensorDataJob, SensorDataJob>();

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

BackgroundJob.Enqueue<ISensorDataJob>(processor => processor.LoadSensorDataAsync());
BackgroundJob.Enqueue<ISensorDataJob>(processor => processor.LoadImageDataAsync());

RecurringJob.AddOrUpdate<ISensorDataJob>("ReadSencorDataJob", processor => processor.LoadSensorDataAsync(), "0 */2 * * *");  // every 2 hours
RecurringJob.AddOrUpdate<ISensorDataJob>("ReadSencorDataJob", processor => processor.LoadImageDataAsync(), "0 */2 * * *");  // every 2 hours

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
