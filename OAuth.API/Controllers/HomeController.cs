using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DoublyLinked;
using System.IO;

namespace OAuth.API.Controllers
{
    public class HomeController : Controller
    {
        const string clientId = "15004a6aa7ba81e8aa63";
        private const string clientSecret = "17e78978d97e0095dda99181a345f766fdc72eca";
        readonly GitHubClient client =
            new GitHubClient(new ProductHeaderValue("InterviewTask"), new Uri("https://github.com/"));

        public async Task<ActionResult> Index()
        {
            var accessToken = Session["OAuthToken"] as string;
            if (accessToken != null)
            {
                // This allows the client to make requests to the GitHub API on the user's behalf
                // without ever having the user's OAuth credentials.
                client.Credentials = new Credentials(accessToken);
            }

            try
            {
                // The following requests retrieves all of the user's repositories and
                // requires that the user be logged in to work.
                var repositories = await client.Repository.GetAllForCurrent();
                var commits = await client.Repository.Commit.GetAll(repositories[0].Id);

                DoubleLinkedList list = new DoubleLinkedList();
                foreach (GitHubCommit commit in commits)
                {
                    string[] broken_str = commit.Commit.Message.Split(' ');
                    foreach (var sub_str in broken_str)
                    {
                        list.Push(sub_str);
                    }
                }
                Dictionary<string, int> CommitDict = list.OccurancesOfElement(list.Head);
                ViewBag.CommitDict = CommitDict;
                using (var writer = new StreamWriter(Path.Combine(Path.GetTempPath(), "SaveFile.csv")))
                {
                    foreach (var pair in CommitDict)
                    {
                        writer.WriteLine("{0};{1};", pair.Key, pair.Value);
                    }
                }
                return View();
            }
            catch (Exception EX)
            {
                // Either the accessToken is null or it's invalid. This redirects
                // to the GitHub OAuth login page. That page will redirect back to the
                // Authorize action.
                return Redirect(GetOauthLoginUrl());
            }
        }

        public async Task<ActionResult> Authorize(string code, string state)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var expectedState = Session["CSRF:State"] as string;
                if (state != expectedState) throw new InvalidOperationException("SECURITY FAIL!");
                Session["CSRF:State"] = null;

                var token = await client.Oauth.CreateAccessToken(
                    new OauthTokenRequest(clientId, clientSecret, code)
                    {
                        RedirectUri = new Uri("http://localhost:8081/Home/Authorize")
                    });
                Session["OAuthToken"] = token.AccessToken;
            }

            return RedirectToAction("Index");
        }

        private string GetOauthLoginUrl()
        {
            string csrf = Membership.GeneratePassword(24, 1);
            Session["CSRF:State"] = csrf;

            // 1. Redirect users to request GitHub access
            var request = new OauthLoginRequest(clientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf
            };
            var oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginUrl.ToString();
        }

    }
}
