using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.ValueObjects;

public record DateRange(
    [property: Required]
    DateTime Start,

    DateTime End
)
{
    public bool IsValid() => Start <= End;

    public TimeSpan Duration() => End - Start;

    public bool Contains(DateTime date) =>
        Start <= date && date <= End;

    public bool Overlaps(DateRange other) =>
        Start <= other.End && other.Start <= End;
}