using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SGSX.Exploria.Application.Models.Weather;
public class WeatherReportQuery
{
    public required IReadOnlyDictionary<string, string> Params { get; init; }

    public Guid ToUniqueId()
    {
        if (!Params.Any())
            return Guid.Empty;

        var queryData = Params
            .OrderBy(x => x.Key)
            .Aggregate(new StringBuilder(),
            (builder, param) =>
                builder
                .Append(param.Key)
                .Append('=')
                .Append(param.Value)
                .Append(';'))
            .ToString();

        var hashedData = MD5.HashData(Encoding.UTF8.GetBytes(queryData));

        return MemoryMarshal.Read<Guid>(hashedData);
    }
}
