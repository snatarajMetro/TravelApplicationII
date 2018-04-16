using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace TravelApplication.Services
{
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Sends an email message. It simply sends an email message that doen't have attachement
        /// </summary>
        /// <param name="sendFrom">from email address</param>
        /// <param name="sendTo">to email address. This can contains mutliple email addresses with comma or semicolon delimiter.</param>
        /// <param name="subject">subject</param>
        /// <param name="body">email message</param>
        /// <returns></returns>
        public async Task<bool> SendEmail(string sendFrom, string sendTo, string subject, string body)
        {
            return await SendEmail(sendFrom, sendTo, subject, body, "", "", "", "");
        }

        /// <summary>
        /// Sends an email message.
        /// </summary>
        /// <param name="sendFrom">From email address</param>
        /// <param name="sendTo">To email address. This can contains mutliple email addresses with comma or semicolon delimiter.</param>
        /// <param name="subject">subject</param>
        /// <param name="body">email message</param>
        /// <param name="attachmentFile">attached file's full path</param>
        /// <param name="cc">CC email address. This can contains mutliple email addresses with comma or semicolon delimiter.</param>
        /// <param name="bcc">Bcc email address. This can contains mutliple email addresses with comma or semicolon delimiter.</param>
        /// <param name="smtpServer">SMTP server</param>
        /// <returns></returns>
        public async Task<bool> SendEmail(string sendFrom, string sendTo, string subject, string body,
                                            string attachmentFile, string cc, string bcc, string smtpServer)
        {
            MailMessage emailMessage = default(MailMessage);

            try
            {
                emailMessage = new MailMessage();

                // To add sendTo emails
                char[] delimiters = new[] { ',', ';' };  // List of your delimiters
                string[] sendtolist = sendTo.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (string emailTo in sendtolist)
                {
                    emailMessage.To.Add(new MailAddress(emailTo.Trim()));
                }

                emailMessage.From = new MailAddress(sendFrom);
                emailMessage.Subject = subject;
                emailMessage.Body = body;
                emailMessage.IsBodyHtml = true;

               /* // To add cc email address if exists
                if (!string.IsNullOrEmpty(cc))
                {
                    string[] sendCClist = cc.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string emailCC in sendCClist)
                    {
                        emailMessage.CC.Add(new MailAddress(emailCC.Trim()));
                    }
                }

                // To add bcc email address if exists
                if (!string.IsNullOrEmpty(bcc))
                {
                    string[] sendBcclist = cc.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string emailBcc in sendBcclist)
                    {
                        emailMessage.Bcc.Add(new MailAddress(emailBcc.Trim()));
                    }
                }
                */
                // To attach file
                if (!string.IsNullOrEmpty(attachmentFile) && FileExists(attachmentFile))
                {
                    Attachment emailAttachment = new Attachment(attachmentFile);
                    emailMessage.Attachments.Add(emailAttachment);
                }

                // To create SmtpClient object
                SmtpClient smtpClient = default(SmtpClient);
                if (!string.IsNullOrEmpty(smtpServer))
                {
                    smtpClient = new SmtpClient(smtpServer);
                }
                else
                {                    
                    smtpClient = new SmtpClient(ConfigurationManager.AppSettings["emailServer"]);

                }


                smtpClient.Send(emailMessage);
                return true;
            }
            catch (Exception e)
            {
                String errorMessage = e.Message;
                return false;
            }
        }


        /// <summary>
        /// Checks wether a file exists or not
        /// </summary>
        /// <param name="fullFilePath">full file path</param>
        /// <returns>Return true if a file exists; otherwise, return false</returns>
        private bool FileExists(string fullFilePath)
        {
            if (fullFilePath.Trim() == "") return false;
            FileInfo fileInfo = new FileInfo(fullFilePath);
            return fileInfo.Exists;
        }
    }
}