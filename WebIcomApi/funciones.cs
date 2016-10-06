using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace WebIcomApi
{
    public static class funciones
    {
        public static void sendMail(MailMessage msg, string strsmtp, string emisor, string pass, int puerto, Boolean blnssl) 
        {                        

            SmtpClient smtp = new SmtpClient(strsmtp);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(emisor, pass);
            smtp.Port = puerto;
            smtp.Host = strsmtp;
            smtp.EnableSsl = blnssl;

            smtp.Send(msg);
        }       
    }
}