using System.Text;
using FiapAgro.Domain.Enums;
using FiapAgro.Domain.Exceptions;

namespace FiapAgro.Domain.Helpers;

/// <summary>
/// Conversão entre enums C# (PascalCase) e strings snake_case esperadas pelo app mobile.
/// </summary>
public static class EnumHelper
{
    /// <summary>Converte PascalCase para snake_case. Ex.: "FerrugemAsiatica" → "ferrugem_asiatica".</summary>
    public static string ToSnakeCase(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase)) return pascalCase;

        var sb = new StringBuilder();
        for (var i = 0; i < pascalCase.Length; i++)
        {
            var c = pascalCase[i];
            if (char.IsUpper(c) && i > 0)
                sb.Append('_');
            sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString();
    }

    /// <summary>Converte snake_case para enum PascalCase. Ex.: "ferrugem_asiatica" → PragaTipo.FerrugemAsiatica.</summary>
    public static T FromSnakeCase<T>(string snakeCase) where T : struct, Enum
    {
        var pascal = string.Join("", snakeCase.Split('_')
            .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));

        if (Enum.TryParse<T>(pascal, ignoreCase: true, out var result))
            return result;

        throw new RegraDeNegocioException($"Valor '{snakeCase}' inválido para {typeof(T).Name}.");
    }

    /// <summary>Label PT-BR para exibição no mobile.</summary>
    public static string CulturaLabel(CulturaTipo cultura) => cultura switch
    {
        CulturaTipo.Milho => "Milho",
        CulturaTipo.Tomate => "Tomate",
        CulturaTipo.Alface => "Alface",
        CulturaTipo.Feijao => "Feijão",
        CulturaTipo.Mandioca => "Mandioca",
        CulturaTipo.Soja => "Soja",
        CulturaTipo.Cana => "Cana-de-açúcar",
        _ => cultura.ToString()
    };

    /// <summary>Label PT-BR para exibição no mobile.</summary>
    public static string PragaLabel(PragaTipo praga) => praga switch
    {
        PragaTipo.Sadia => "Sadia",
        PragaTipo.FerrugemAsiatica => "Ferrugem Asiática",
        PragaTipo.LagartaDoCartucho => "Lagarta-do-cartucho",
        PragaTipo.ManchaFoliar => "Mancha Foliar",
        PragaTipo.Oidio => "Oídio",
        PragaTipo.MoscaBranca => "Mosca-branca",
        PragaTipo.BrocaDoCafe => "Broca-do-café",
        PragaTipo.Antracnose => "Antracnose",
        _ => praga.ToString()
    };
}
