using System;

namespace GymApplication.Controllers
{
    internal class SendGridClient
    {
        private string v;

        public SendGridClient(string v)
        {
            this.v = v;
        }

        internal object SendEmailAsync(object msg)
        {
            throw new NotImplementedException();
        }
    }
}