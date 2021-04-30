//using SendGrid.Helpers.Mail;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace GymApplication.Models.GymTracking
//{
//    public class AppointmentConfirmEmail
//    {
//        //overload with what you want to show in the email.
//        public void SendConfirmation(string Email, string FullName, DateTime Date,DateTime Time, string Slot, string Teacher)
//        {
//            try
//            {
//                var myMessage = new SendGridMessage
//                {
//                    From = new EmailAddress("Appointments@InnovtechAcademy.co.za", "Innovtech Academy")
//                };
//                myMessage.AddTo(Email);
//                string subject = "Appointment outcome";
//                string body = (
//                "Dear " + FullName + "" + "<br/>"
//                + "<br/>"
//                + "Your appointment has been successfully scheduled, your appointment details are as follows: "
//                + "<br/>" + "" + "" + "Appointment with"+"" + Teacher + ""
//                + "<br/>" + "Date:" + "" + Date.ToString("dd-MM-yyyy-dddd") + ""
//                + "<br/>" + "" + "" + " Session:" + "" + Slot + ""
//                 + "<br/>" + "" + "" + " Which starts at:" + "" + Time.ToString("HH:mm") + ""
//                 + "<br/>" + "Please note the duration of meetings is 15 minutes." +""
//                + "<br/>" + "Ensure to keep this information as your proof of appointment" +
//                "<br/>" +
//                "Sincerely Yours, " +
//                "<br/>" +
//                "Innovtech Academy");
//                myMessage.Subject = subject;
//                myMessage.HtmlContent = body;
//                var transportWeb = new
//               SendGrid.SendGridClient("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");

//                transportWeb.SendEmailAsync(myMessage);
//            }
//            catch (Exception x)
//            {
//                Console.WriteLine(x);
//            }
//        }
//    }
//}