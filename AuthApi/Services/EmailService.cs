using AuthApi.Exceptions;
using AuthApi.Interfaces;
using AuthApi.Models;
using Azure;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorLight;
using System.Threading.Tasks;

namespace AuthApiCoreIdentity.Services
{
    public class EmailService(IOptions<SmtpSettings> smtpSettings, IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger) : IEmailService
    {
        private readonly ILogger _logger = logger;
        private readonly SmtpSettings _smtpSettings = smtpSettings.Value;
        private readonly EmailSettings _emailSettings = emailSettings.Value;
        private readonly RazorLightEngine _razor = new RazorLightEngineBuilder()
               .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "Templates"))
                .UseMemoryCachingProvider()
                .Build();

        public async Task SendEmailAsync(EmailModel emailData)
        {

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_emailSettings.Name, _emailSettings.Email));

            foreach (var toAddress in emailData.ToAddress)
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));

            if (emailData.Ccs is not null)
                foreach (var cc in emailData.Ccs)
                    emailMessage.Cc.Add(MailboxAddress.Parse(cc));

            if (emailData.Ccos is not null)
                foreach (var cco in emailData.Ccos)
                    emailMessage.Bcc.Add(MailboxAddress.Parse(cco));

            emailMessage.Subject = emailData.Subject;

            var bodyBuilder = new BodyBuilder();

            if (!string.IsNullOrWhiteSpace(emailData.Template))
            {
                GetTemplatePath(emailData.Template);
                bodyBuilder.HtmlBody = await _razor.CompileRenderAsync(
                    $"{emailData.Template}.cshtml",
                    emailData.TemplateModel);
            }


            if (emailData.Attachment is not null)
            {
                foreach (var anexo in emailData.Attachment)
                {
                    if (anexo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await anexo.CopyToAsync(memoryStream);
                        bodyBuilder.Attachments.Add(anexo.FileName, memoryStream.ToArray(), ContentType.Parse(anexo.ContentType));
                    }
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();

            string? response = null;
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                       _smtpSettings.Host,
                         int.Parse(_smtpSettings.Port),
                         SecureSocketOptions.StartTls);

                    var oauth2 = new SaslMechanismOAuth2(_smtpSettings.ClientId, _smtpSettings.SmtpToken);

                    //await client.AuthenticateAsync(oauth2);

                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

                    response = await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);


                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Erro ao enviar e-mail para {Destinatarios}. Assunto: {Assunto}.",
                string.Join(", ", emailData.ToAddress), emailData.Subject);
                throw new SendEmailException("Erro ao enviar email");
            }

        }

        private string GetTemplatePath(string templateName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{templateName}.cshtml");

            if (!File.Exists(path))
                throw new NotFoundException($"Template '{templateName}.cshtml' não encontrado em {path}");

            return path;
        }
    }
}
