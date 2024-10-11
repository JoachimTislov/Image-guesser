using System.ComponentModel.DataAnnotations;

namespace Image_guesser.Core.Domain.OracleContext;

public enum AI_Type
{
    [Display(Name = "Tiles in random order")]
    Random,

    [Display(Name = "Tiles in incremental order")]
    Incremental,

    [Display(Name = "Tiles in decremental order")]
    Decremental,

    [Display(Name = "Tiles in odd-even order")]
    OddEvenOrder,

    [Display(Name = "Tiles in even-odd order")]
    EvenOddOrder

}