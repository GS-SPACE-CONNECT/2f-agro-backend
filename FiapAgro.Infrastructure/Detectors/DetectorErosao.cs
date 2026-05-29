using FiapAgro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FiapAgro.Infrastructure.Detectors;

public class DetectorErosao : DetectorBase<AlertaErosao>
{
    public DetectorErosao(ILogger<DetectorErosao> logger) : base(logger) { }

    public override bool PodeAnalisar(IDictionary<string, object> dados) =>
        dados.ContainsKey("probabilidade") &&
        dados.ContainsKey("inclinacaoSolo");

    protected override AlertaErosao CriarAlerta(Guid propriedadeId, IDictionary<string, object> dados) =>
        AlertaFactory.CriarErosao(
            propriedadeId,
            Convert.ToDouble(dados["probabilidade"]),
            Convert.ToDouble(dados["inclinacaoSolo"]));
}
