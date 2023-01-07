using System.ComponentModel.DataAnnotations;

namespace Cafet_Backend.Models;

public class KeyedEntity
{
    [Key] public int Id { get; set; }
}