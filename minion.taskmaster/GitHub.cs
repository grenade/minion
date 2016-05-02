using Octokit;
using System.Configuration;

namespace minion.taskmaster
{
    internal class GitHub
    {
        public GitHub()
        {
            var githubProductHeader = ConfigurationManager.AppSettings.Get("githubProductHeader");
            var githubUsername = ConfigurationManager.AppSettings.Get("githubUsername");
            var githubPassword = ConfigurationManager.AppSettings.Get("githubPassword");
            var client = new GitHubClient(new ProductHeaderValue(githubProductHeader)) { Credentials = new Credentials(githubUsername, githubPassword) };
        }
    }
}
