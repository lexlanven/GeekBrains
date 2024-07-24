using System.Text.Json;


namespace Client;

public class Message

{
    public string Date { get; set; }
    public string FromName { get; set; }
    public string Text { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
    
    public static Message FromJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<Message>(json);
        }
        catch (JsonException ex)
        {
            throw new JsonException("Error deserializing JSON into Message object.", ex);
        }
    }

}
