namespace WebApiForm.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmail(string toEmail, string subject,/* string plainTextContent,*/ string htmlContent);
        //El método es asíncrono(Task), lo cual es ideal para operaciones de red, como enviar correos, que pueden tardar un poco.
    }
}
