using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using TB_Stock.Api.ApiHandler;
using TB_Stock.Api.FTP;
using TB_Stock.DAL.Models;
using TBStock.DAL.Models;
using TBStock.DAL.Repositories;

namespace TB_Stock.Api.Controllers
{
    [RoutePrefix("api/Email")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmailController : ApiController
    {
        private const string EMAIL_RECIPIENT_1 = "tarek.aly.soltan@gmail.com";
        private const string EMAIL_RECIPIENT_2 = "walidalysultan@gmail.com";
        private const string SUBJECT_PREFIX= "TBOriginalBrands - Get it in touch - ";

        [HttpPost]
        [Route("Send")]
        public async Task Send(EmailSenderContent content)
        {
            using (var smtpClient = new SmtpClient()) {

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("TB@tboriginalbrands.com"),
                    Subject = string.Concat(SUBJECT_PREFIX, content.Name),
                    Body = string.Concat(content.Content, "<br/><br/>", string.Concat("From: ",content.Email)),
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(EMAIL_RECIPIENT_1);
                mailMessage.To.Add(EMAIL_RECIPIENT_2);

                smtpClient.Send(mailMessage);
            }
        }

        public class EmailSenderContent
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Content { get; set; }
        }



    }
}
