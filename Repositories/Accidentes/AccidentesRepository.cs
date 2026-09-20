using AccidentesMadrid.Models;

namespace AccidentesMadrid.Repositories.Accidentes;

public class AccidentesRepository : IAccidentesRepository
{
    private readonly List<Accidente> _accidentes = new();

    public IEnumerable<Accidente> GetAll() {
        return _accidentes;
    }
    
    public void AddRange(IEnumerable<Accidente> accidentes) {
        _accidentes.AddRange(accidentes);
    }
}