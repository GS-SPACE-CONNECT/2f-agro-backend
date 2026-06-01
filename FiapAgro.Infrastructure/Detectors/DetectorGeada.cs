using FiapAgro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FiapAgro.Infrastructure.Detectors;

public class DetectorGeada : DetectorBase<AlertaGeada>
{
    public DetectorGeada(ILogger<DetectorGeada> logger) : base(logger) { }

    public override bool PodeAnalisar(IDictionary<string, object> dados) =>
        dados.ContainsKey("probabilidade") &&
        dados.ContainsKey("temperaturaMinima");

    protected override AlertaGeada CriarAlerta(Guid propriedadeId, IDictionary<string, object> dados) =>
        AlertaFactory.CriarGeada(
            propriedadeId,
            Convert.ToDouble(dados["probabilidade"]),
            Convert.ToDouble(dados["temperaturaMinima"]));
}
