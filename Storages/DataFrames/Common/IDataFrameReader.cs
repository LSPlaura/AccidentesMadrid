using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Storages.DataFrames.Common;

public interface IDataFrameReader
{
    DataFrame Load(string path);
}