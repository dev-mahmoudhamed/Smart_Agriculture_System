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
            // تأكد من اتصال MongoDB
            await _context.Database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");

            // تهيئة بيانات التربة إذا كانت المجموعة فارغة
            if (await _context.Soils.CountDocumentsAsync(_ => true) == 0)
            {
                var soils = new List<Soil>
            {
                new Soil { Temperature = 25.5, Humidity = 60, PH = 6.8 },
                new Soil { Temperature = 22.0, Humidity = 55, PH = 7.2 }
            };
                await _context.Soils.InsertManyAsync(soils);
            }

            // تهيئة الأمراض إذا كانت المجموعة فارغة
            if (await _context.Diseases.CountDocumentsAsync(_ => true) == 0)
            {
                var diseases = new List<Disease>
            {
                new Disease {
                    Name = "Leaf Spot",
                    Symptoms = "Brown spots on leaves",
                    Treatment = "Apply fungicide"
                },
                new Disease {
                    Name = "Powdery Mildew",
                    Symptoms = "White powder on leaves",
                    Treatment = "Use neem oil"
                }
            };
                await _context.Diseases.InsertManyAsync(diseases);
            }

            // تهيئة النباتات مع ربطها بالأمراض
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

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
