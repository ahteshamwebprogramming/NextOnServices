using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("QuestionOptions")]
public class QuestionOption
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? Qid { get; set; }

    public int? OptionNumber { get; set; }

    public string? OptionLabel { get; set; }

    public string? OptionCode { get; set; }

    public bool? IsChecked { get; set; }

    public DateTime? CrDate { get; set; }
}
