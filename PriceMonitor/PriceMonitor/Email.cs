using System.Net;
using System.Net.Mail;
using System.Text;

namespace PriceMonitor
{
    class Email
    {
        private readonly string _email;

        public Email(string emailTo)
        {
            _email = emailTo;
        }

        public void Send(string subject, string title, string price, string shipping, string total, string website, string category, string url, bool isRemoving)
        {
            string messageToSend = (isRemoving == false)
                ? CreateHTML("Monitored New", title, price, shipping, total, website, category, url)
                : CreateHTML("Removed", title, price, shipping, total, website, category, url);

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("notifyproductmonitor@gmail.com", "pricemonitor-project"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress("notifyproductmonitor@gmail.com"),
                Subject = subject,
                Body = messageToSend,
                IsBodyHtml = true
            };

            mailMessage.To.Add(_email);
            smtpClient.Send(mailMessage);
        }

        private string CreateHTML(string addOrRemove, string title, string price, string shipping, string total, string website, string category, string url)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div>");
            sb.AppendFormat("<link href='https://fonts.googleapis.com/css2?family=Ubuntu:wght@300&display=swap' rel='stylesheet'>");
            sb.AppendFormat("<div style=\"font-family: 'Ubuntu', sans-serif;\">");
            sb.AppendFormat("<header style='background-color: rgb(42, 130, 208); padding: 15px; color: white;'>");
            sb.AppendFormat("<h1>Product Monitor</h1>");
            sb.AppendFormat("</header>");

            sb.AppendFormat("<div style='padding: 15px;'>");
            sb.AppendFormat($"<h2><b>{addOrRemove} Product</b></h2>");

            sb.AppendFormat("<div style='font-weight: lighter; margin-top: 10px; font-style: italic;'>");
            sb.AppendFormat($"<h3><b style='font-style: italic;'>Title: </b>{title}<h3>");
            sb.AppendFormat($"<h3><b style='font-style: italic;'>Price: </b>€{price}</h3>");
            sb.AppendFormat($"<h3><b style='font-style: italic;'>Shipping: </b>€{shipping}</h3>");
            sb.AppendFormat($"<h3><b style='font-style: italic;'>Total: </b>€{total}</h3>");
            sb.AppendFormat($"<h3><b style='font-style: italic;'>Website: </b>{website}</h3>");
            if (category != null) sb.AppendFormat($"<h3><b style='font-style: italic;'>Category: </b>{category}</h3>");
            sb.AppendFormat($"<h3><b style='font-style: italic;'><a href='{url}'>Click Here</a></b></h3>");
            sb.AppendFormat("</div>");
            sb.AppendFormat("</div>");
            sb.AppendFormat("</div>");
            sb.AppendFormat("</div>");

            return sb.ToString();
        }
    }
}