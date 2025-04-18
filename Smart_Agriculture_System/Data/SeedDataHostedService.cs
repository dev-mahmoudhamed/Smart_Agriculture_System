using MongoDB.Driver;
using Smart_Agriculture_System.Models;


namespace Smart_Agriculture_System.Data
{
    public class SeedDataHostedService : IHostedService
    {
        private readonly MongoDBContext _context;

        public SeedDataHostedService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _context.Database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
            await SeedReadings();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task SeedSoil()
        {
            if (await _context.Soils.CountDocumentsAsync(_ => true) == 0)
            {
                var soils = new List<Soil>
            {
                new Soil { Temperature = 25.5, Humidity = 60, PH = 6.8 },
                new Soil { Temperature = 22.0, Humidity = 55, PH = 7.2 }
            };
                await _context.Soils.InsertManyAsync(soils);
            }
        }

        private async Task SeedDiseases()
        {
            if (await _context.Diseases.CountDocumentsAsync(_ => true) == 0)
            {
                var diseases = new List<Disease>
            {
                new Disease {
                    Name = "Leaf Spot",
                    Symptoms = new List<string> { "Brown spots on leaves", "Yellowing of leaves" },
                    Treatment = "Apply fungicide"
                },
                new Disease {
                    Name = "Powdery Mildew",
                    Symptoms = new List<string> { "White powder on leaves", "Stunted growth" },
                    Treatment = "Use neem oil"
                }
            };
                await _context.Diseases.InsertManyAsync(diseases);
            }
        }

        private async Task SeedPlants()
        {

            if (await _context.Plants.CountDocumentsAsync(_ => true) == 0)
            {
                var diseases = await _context.Diseases.Find(_ => true).ToListAsync();

                var plants = new List<Plant>
            {
                new Plant {
                    Type = "Tomato",
                    Status = "Healthy",
                    DiseaseIds = diseases.Take(1).Select(d => d.Id).ToList()
                },
                new Plant {
                    Type = "Rose",
                    Status = "Infected",
                    DiseaseIds = diseases.Select(d => d.Id).ToList()
                }
            };
                await _context.Plants.InsertManyAsync(plants);
            }
        }

        private async Task SeedReadings()
        {
            if (await _context.Readings.CountDocumentsAsync(_ => true) == 0)
            {
                var readings = new List<Reading>
            {
                new Reading {
                    Temperature = 25.5,
                    Humidity = 60,
                    Time = DateTime.Now.ToLocalTime().ToString("d/M/yyyy h:mm:ss tt"),
                    ImageAsBase64 = "test"
                },
                new Reading {
                    Temperature = 22.0,
                    Humidity = 55,
                    Time = DateTime.Now.ToLocalTime().ToString("d/M/yyyy h:mm:ss tt"),
                    ImageAsBase64 = "test"
                }
            };
                await _context.Readings.InsertManyAsync(readings);
            }
        }
    }
}
