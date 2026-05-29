using FiapAgro.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FiapAgro.Infrastructure.Detectors;

public class DetectorEnchente : DetectorBase<AlertaEnchente>
{
    public DetectorEnchente(ILogger<DetectorEnchente> logger) : base(logger) { }

    public override bool PodeAnalisar(IDictionary<string, object> dados) =>
        dados.ContainsKey("probabilidade") &&
        dados.ContainsKey("volumeMM");

    protected override AlertaEnchente CriarAlerta(Guid propriedadeId, IDictionary<string, object> dados) =>
        AlertaFactory.CriarEnchente(
            propriedadeId,
            Convert.ToDouble(dados["probabilidade"]),
            Convert.ToDouble(dados["volumeMM"]));
}
