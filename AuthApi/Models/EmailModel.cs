namespace AuthApi.Models;
public class EmailModel
{
    public required List<string> ToAddress { get; set; }
    public List<string>? Ccs { get; set; }
    public List<string>? Ccos { get; set; }
    public required string Subject { get; set; }
    public List<IFormFile>? Attachment { get; set; }
    public string Template { get; set; }
    public object TemplateModel { get; set; }

}