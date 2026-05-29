using FiapAgro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FiapAgro.Infrastructure.Detectors;

public class DetectorPraga : DetectorBase<AlertaPraga>
{
    public DetectorPraga(ILogger<DetectorPraga> logger) : base(logger) { }

    public override bool PodeAnalisar(IDictionary<string, object> dados) =>
        dados.ContainsKey("probabilidade") &&
        dados.ContainsKey("especiePraga") &&
        dados.ContainsKey("culturaAfetada");

    protected override AlertaPraga CriarAlerta(Guid propriedadeId, IDictionary<string, object> dados) =>
        AlertaFactory.CriarPraga(
            propriedadeId,
            Convert.ToDouble(dados["probabilidade"]),
            dados["especiePraga"].ToString()!,
            dados["culturaAfetada"].ToString()!);
}
