using AccidentesMadrid.Models;
using AccidentesMadrid.Storages.Common;

namespace AccidentesMadrid.Storages.Accidentes.Reader;

//patrón storage de csv simplificado para que solo importe
public interface IAccidentesReader : IReader<Accidente>
{
    
}