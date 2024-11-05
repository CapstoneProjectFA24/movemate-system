using System.Text.Json.Serialization;

namespace MoveMate.Service.ThirdPartyService.Firebase.Notification;

internal record FirebaseTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken, 
    [property: JsonPropertyName("token_type")] string TokenType, 
    [property: JsonPropertyName("expires_in")] int ExpiresIn);