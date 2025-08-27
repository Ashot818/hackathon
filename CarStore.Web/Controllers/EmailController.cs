using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Mail;

namespace WarnDetect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    [HttpPost("send")]
    public IActionResult SendEmail([FromBody] EmailRequest request)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("My App", "steven.shelbycargollc@gmail.com")); 
            message.To.Add(new MailboxAddress("", request.To));
            message.Subject = request.Subject;
            message.Body = new TextPart("plain") { Text = request.Body };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            client.Authenticate("steven.shelbycargollc@gmail.com", "kimt bnnq cezo seqi"); 
            client.Send(message);
            client.Disconnect(true);

            return Ok(new { success = true, message = "Email sent successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}

public class EmailRequest
{
    public string To { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
}