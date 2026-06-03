using FiapAgro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FiapAgro.Infrastructure.Detectors;

public class DetectorSeca : DetectorBase<AlertaSeca>
{
    public DetectorSeca(ILogger<DetectorSeca> logger) : base(logger) { }

    public override bool PodeAnalisar(IDictionary<string, object> dados) =>
        dados.ContainsKey("probabilidade") &&
        dados.ContainsKey("diasSemChuva");

    protected override AlertaSeca CriarAlerta(Guid propriedadeId, IDictionary<string, object> dados) =>
        AlertaFactory.CriarSeca(
            propriedadeId,
            Convert.ToDouble(dados["probabilidade"]),
            Convert.ToInt32(dados["diasSemChuva"]));
}
