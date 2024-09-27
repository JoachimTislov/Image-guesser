using System.ComponentModel.DataAnnotations;

namespace Image_guesser.Core.Domain.OracleContext;

public enum AI_Type
{
    [Display(Name = "Tiles in random order")]
    Random,
}