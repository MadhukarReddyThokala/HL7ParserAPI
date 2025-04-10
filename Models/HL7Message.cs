using System;
using System.ComponentModel.DataAnnotations;

public class HL7Message
{
    [Key]
    public Guid Id { get; set; }
    public string? Data { get; set; }
}
