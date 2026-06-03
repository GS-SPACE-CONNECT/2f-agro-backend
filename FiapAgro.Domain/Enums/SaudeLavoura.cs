namespace FiapAgro.Domain.Enums;

/// <summary>
/// Estado de saúde de uma lavoura. Mapeia 1:1 com <c>LavouraSaudeKey</c> do app mobile.
/// </summary>
public enum SaudeLavoura
{
    Saudavel,
    Atencao,
    Risco,
    Perdida
}
