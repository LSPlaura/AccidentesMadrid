using AccidentesMadrid.Models;
using AccidentesMadrid.Repositories.Accidentes;
using AccidentesMadrid.Storages.Accidentes;

namespace AccidentesMadrid.Services.Csv.Accidentes;

public class AccidentesCsvService(IAccidentesReader accidentesStorage, AccidentesRepository repository) : IAccidentesCsvService
{
    public async Task Salvar(IEnumerable<string> paths)
    {
        var tasks = paths.Select(path => accidentesStorage.Cargar(path).ToListAsync().AsTask());
        List<Accidente>[] resultados = await Task.WhenAll(tasks);
        var list = resultados.SelectMany(x => x);
        repository.AddRange(list);
    }

    public IEnumerable<Accidente> Cargar()
    {
        return repository.GetAll();
    }
}