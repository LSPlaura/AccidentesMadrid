using AccidentesMadrid.Models;
using AccidentesMadrid.Repositories.Accidentes;
using AccidentesMadrid.Storages.Accidentes;

namespace AccidentesMadrid.Services.Csv.Accidentes;

public class AccidentesCsvService(IAccidentesReader accidentesStorage, AccidentesRepository repository) : IAccidentesCsvService
{
    public async Task Salvar(IEnumerable<string> paths, int batchSize = 1000)
    {
        var batch = new List<Accidente>(batchSize);

        foreach (var path in paths)
        {
            await foreach (var accidente in accidentesStorage.Cargar(path))
            {
                batch.Add(accidente);

                if (batch.Count >= batchSize)
                {
                    repository.AddRange(batch);
                    batch.Clear();
                }
            }
        }
        
        if (batch.Count > 0)
        {
            repository.AddRange(batch);
        }
    }

    public IEnumerable<Accidente> Cargar()
    {
        return repository.GetAll();
    }
}