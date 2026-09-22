using AccidentesMadrid.Models;
using AccidentesMadrid.Storages.Common;

namespace AccidentesMadrid.Storages.Accidentes;

//patrón storage de csv simplificado para que solo importe
public interface IAccidentesReader : IReader<Accidente>
{
    
}