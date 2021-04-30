using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GymApplication.Models.GymTracking
{
    public class DeclineAppointmentEmail
    {
        //overload with what you want to show in the email.
        public void SendConfirmation(string Email, string FullName, DateTime Date, string Slot, string Teacher)
        {
            try
            {
                var myMessage = new SendGridMessage
                {
                    From = new EmailAddress("Appointments@InnovtechAcademy.co.za", "Innovtech Academy")
                };
                myMessage.AddTo(Email);
                string subject = "Appointment outcome";
                string body = (
                "Dear " + FullName + "" + "<br/>"
                + "<br/>"
                + "Unfortunately your appoint has been declined: "
                + "<br/>" + "" + "" + "Appointment with" + "" + "" + Teacher + ""
                + "<br/>" + "Please try to reschedule for another day. We appologize for the inconvience." +
                "<br/>" +
                "Sincerely Yours, " +
                "<br/>" +
                "Innovtech Academy");
                myMessage.Subject = subject;
                myMessage.HtmlContent = body;
                var transportWeb = new
               SendGrid.SendGridClient("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");

                transportWeb.SendEmailAsync(myMessage);
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }
        }
    }
}