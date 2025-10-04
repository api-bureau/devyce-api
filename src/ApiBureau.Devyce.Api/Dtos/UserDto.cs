namespace ApiBureau.Devyce.Api.Dtos;

public class UserDto
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public required bool ActiveState { get; set; }
    public required string EmailAddress { get; set; }
}