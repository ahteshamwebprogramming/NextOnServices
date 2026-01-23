using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("QuestionsMaster")]
public class QuestionsMaster
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? QuestionId { get; set; }

    public string? QuestionLabel { get; set; }

    public int? QuestionType { get; set; }

    public DateTime? CreationDate { get; set; }
}
