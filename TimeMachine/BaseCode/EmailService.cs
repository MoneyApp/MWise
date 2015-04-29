using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeMachine.BaseCode
{
    public class EmailService
    {
        public static void SendSimpleMessage()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                   new HttpBasicAuthenticator("api",
                                              "key-6de7390c50abada1b301c00207a3be35");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                "sandboxa39a1e82a3b14174a1eb4c8ccd6b3082.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxa39a1e82a3b14174a1eb4c8ccd6b3082.mailgun.org>");
            request.AddParameter("to", "Ambuj Sharma <ambujsharma23@gmail.com>");
            request.AddParameter("subject", "Hello Ambuj Sharma");
            request.AddParameter("text", "Congratulations Ambuj Sharma, you just sent an email with Mailgun!  You are truly awesome!  You can see a record of this email in your logs: https://mailgun.com/cp/log .  You can send up to 300 emails/day from this sandbox server.  Next, you should add your own domain so you can send 10,000 emails/month for free.");
            request.Method = Method.POST;
            client.Execute(request);
        }
    
    }
}