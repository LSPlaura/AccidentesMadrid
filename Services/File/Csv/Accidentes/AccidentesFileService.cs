using AccidentesMadrid.Models;
using AccidentesMadrid.Repositories.Accidentes;
using AccidentesMadrid.Storages.Accidentes.Reader;
using AccidentesMadrid.Storages.Accidentes.Writer;

namespace AccidentesMadrid.Services.File.Csv.Accidentes;

public class AccidentesFileService(IAccidentesReader accidentesReader, IAccidentesWriter accidentesWriter, AccidentesRepository repository) : IAccidentesFileService
{
    public async Task Import(IEnumerable<string> paths, int batchSize = 1000)
    {
        var batch = new List<Accidente>(batchSize);

        foreach (var path in paths)
        {
            await foreach (var accidente in accidentesReader.Load(path))
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

    public async Task Export(string path)
    {
        var list =  repository.GetAll();
        await accidentesWriter.Load(list, path);
    }

    public IEnumerable<Accidente> GetAll()
    {
        return repository.GetAll();
    }
}