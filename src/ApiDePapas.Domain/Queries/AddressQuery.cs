using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.Queries;

public record AddressQuery(
    string Street = "",
    int Number = 0,
    string PostalCode = "",
    string LocalityName = ""
);