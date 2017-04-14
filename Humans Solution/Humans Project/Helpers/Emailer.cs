using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Humans.Web.Models;
using System.Net.Mail;

namespace Humans.Web.Helpers
{
    public static class Emailer
    {
        public static void SendMail3()
        {/*
            try
            {
                //Assign the smtp credentials for gmail
                SmtpClient smtp = new SmtpClient();
                if (true)
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("from emil address", "password");
                    smtp.Timeout = 20000;
                }

                MailAddress fromAddress = new MailAddress("frommail@gmail.com", "From Name");
                MailAddress toAddress = new MailAddress("tomail@gmail.com");
                MailAddress ccAddress = new MailAddress("ccmail@gmail.com");

                //Passing values to smtp object
                dynamic message = new MailMessage(fromAddress, toAddress);
                message.CC.Add(ccAddress);
                message.Subject = "Mail Subject";
                message.Body = "Mail Body";
                message.IsBodyHtml = true;

                //Send email
                smtp.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }*/
        }

        public static bool SendMail2(string toAddress, string subject, string body, out Result result)
        {
            result = new Result();

            try
            {
                MailAddress mailfrom = new MailAddress(Properties.Settings.Default.SMTP_Username, "HUMANS Admin");
                MailAddress mailto = new MailAddress(toAddress);
                MailMessage newmsg = new MailMessage(mailfrom, mailto);

                newmsg.IsBodyHtml = true;
                newmsg.Subject = subject;
                newmsg.Body = body;
                SmtpClient smtp = new SmtpClient(Properties.Settings.Default.SMTP_Host, 25);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.SMTP_Username, Properties.Settings.Default.SMTP_Password);

                smtp.Send(newmsg);
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.MessageForUser = ex.Message;
                result.MessageForLog = ex.InnerException.Message;

                Helpers.Logger.LogError(string.Format("{0}\r\n\r\n{1}\r\n\r\n{2}", ex.Message, ex.InnerException.Message, ex.InnerException.Source));
            }

            return result.Success;
        }
        
        public static bool SendMail(string subject, string body, string toAddress, out Result scr)
        {
            scr = new Result();

            string smtpServer = "smtpout.secureserver.net";

            try
            {
                SmtpClient mySmtpClient = new SmtpClient(smtpServer);

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential basicAuthenticationInfo = new
                   System.Net.NetworkCredential(Properties.Settings.Default.SMTP_Username, Properties.Settings.Default.SMTP_Password);
                mySmtpClient.Credentials = basicAuthenticationInfo;

                // add from,to mailaddresses
                MailAddress from = new MailAddress(Properties.Settings.Default.SMTP_Username, "WinnerInTheMaking Admin");
                MailAddress to = new MailAddress(toAddress, toAddress);
                MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

                // add ReplyTo
                MailAddress replyto = new MailAddress(Properties.Settings.Default.SMTP_Username);
                myMail.ReplyToList.Add(new MailAddress(Properties.Settings.Default.SMTP_Username, "WinnerInTheMaking Admin"));

                // set subject and encoding
                myMail.Subject = subject;
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                myMail.Body = body;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;

                // text or html
                myMail.IsBodyHtml = true;

                mySmtpClient.Send(myMail);

                scr.Success = true;
            }
            catch (Exception ex)
            {
                scr.MessageForLog = string.Format("MESSAGE:{0} --- INNER-EXCEPTION:{1} --- SOURCE:{2} --- STACK-TRACE:{3}", ex.Message, ex.InnerException, ex.Source, ex.StackTrace);
                scr.MessageForUser = "There was a problem sending email.";
            }

            /*

            //Prepare email
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromAddress, fromName);
            message.To.Add(toAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            message.Priority = System.Net.Mail.MailPriority.Normal;

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = Properties.Settings.Default.SMTP_Host;

            //THIS NEXT FOUR LINES ARE IN CASE YOU NEED MORE CONTROL OVER SPECIFICS AND CANNOT USE THE web.config FILE OPTION
            //PASSWORD SHOULD BE SEEN AS A SECURITY RISK SO USING ENCRIPTION WOULD BE IDEAL - THESE ARE ALL HANDLED BY web.config INCLUDING THE client.Credentials LINE. SWEET STUFF IMHO.
            System.Net.NetworkCredential netwrkCrd = new System.Net.NetworkCredential();
            netwrkCrd.UserName = Properties.Settings.Default.SMTP_Username;
            netwrkCrd.Password = Properties.Settings.Default.SMTP_Password;
            client.Credentials = netwrkCrd;

            //Send email
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                scr.MessageForLog = string.Format("MESSAGE:{0} --- INNER-EXCEPTION:{1} --- SOURCE:{2} --- STACK-TRACE:{3}", ex.Message, ex.InnerException, ex.Source, ex.StackTrace);
                scr.MessageForUser = "There was a problem sending email.";
            }
            */

            //Log error
            if (!string.IsNullOrEmpty(scr.MessageForLog)) Logger.LogError(scr.MessageForLog);

            return scr.Success;
        }
    }
}