using System;
using System.Linq;

namespace SMART_ERP.Services;

public static class HondurasRtnValidator
{
    /// <summary>
    /// Valida el RTN (Registro Tributario Nacional) de Honduras
    /// Formato: 0801-1972-00001XX
    /// </summary>
    /// <param name="rtn">RTN a validar</param>
    /// <returns>Tupla con (esVálido, mensaje de error)</returns>
    public static (bool IsValid, string ErrorMessage) ValidateRtn(string rtn)
    {
        if (string.IsNullOrWhiteSpace(rtn))
        {
            return (false, "El RTN es obligatorio");
        }

        // Eliminar espacios y guiones
        var cleanRtn = rtn.Replace(" ", "").Replace("-", "");

        // Verificar longitud exacta de 14 dígitos
        if (cleanRtn.Length != 14)
        {
            return (false, "El RTN debe contener exactamente 14 dígitos");
        }

        // Verificar que sean todos dígitos
        if (!cleanRtn.All(char.IsDigit))
        {
            return (false, "El RTN debe contener solo dígitos");
        }

        // Validar dígito verificador
        if (!ValidateCheckDigit(cleanRtn))
        {
            return (false, "El RTN no es válido - dígito verificador incorrecto");
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Valida el dígito verificador del RTN hondureño
    /// Algoritmo Módulo 11
    /// </summary>
    private static bool ValidateCheckDigit(string rtn)
    {
        // Los primeros 13 dígitos se usan para calcular el dígito verificador
        var digits = rtn.Substring(0, 13).Select(c => int.Parse(c.ToString())).ToArray();
        var weights = new[] { 4, 3, 2, 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
        
        int sum = 0;
        for (int i = 0; i < digits.Length; i++)
        {
            sum += digits[i] * weights[i];
        }

        int remainder = sum % 11;
        int checkDigit = 11 - remainder;
        
        if (checkDigit == 11)
            checkDigit = 0;

        return checkDigit == int.Parse(rtn.Substring(13));
    }

    /// <summary>
    /// Formatea el RTN en el formato estándar hondureño: 0801-1972-00001XX
    /// </summary>
    public static string FormatRtn(string rtn)
    {
        var cleanRtn = rtn.Replace(" ", "").Replace("-", "");
        
        if (cleanRtn.Length != 14)
            return rtn;

        return $"{cleanRtn.Substring(0, 4)}-{cleanRtn.Substring(4, 4)}-{cleanRtn.Substring(8, 6)}";
    }
}
