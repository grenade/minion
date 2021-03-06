﻿using Newtonsoft.Json;
using NLog;
using Octokit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace minion.taskmaster
{
    public class GitHub
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string productHeader { get { return ConfigurationManager.AppSettings.Get("githubProductHeader"); } }
        private static string username { get { return ConfigurationManager.AppSettings.Get("githubUsername"); } }
        private static string password { get { return ConfigurationManager.AppSettings.Get("githubPassword"); } }
        private static string taskRepoOwner { get { return ConfigurationManager.AppSettings.Get("githubTaskRepoOwner"); } }
        private static string taskRepoName { get { return ConfigurationManager.AppSettings.Get("githubTaskRepoName"); } }
        private static string taskRepoCommitterName { get { return ConfigurationManager.AppSettings.Get("githubTaskRepoCommitterName"); } }
        private static string taskRepoCommitterEmail { get { return ConfigurationManager.AppSettings.Get("githubTaskRepoCommitterEmail"); } }
        private GitHubClient client { get; set; }

        public static Payload GetPayload ()
        {
            WebClient webClient = new WebClient();
            var json = webClient.DownloadString(string.Format("https://raw.githubusercontent.com/{0}/{1}/master/in/mozharness-firefox-windows.json?{2}", taskRepoOwner, taskRepoName, Guid.NewGuid()));
            return JsonConvert.DeserializeObject<Payload>(json);
        }

        private static Dictionary<string, DateTimeOffset> githubCommitCache = new Dictionary<string, DateTimeOffset>();

        public static bool RecentCommitsExist(string task, TimeSpan timespan)
        {
            // don't hammer github api, check if we already know about recent commits
            if (githubCommitCache.ContainsKey(task) && githubCommitCache[task] > DateTimeOffset.Now.Add(-timespan))
            {
                logger.Trace("found commit in cache for {0}, dated {1}.", task, githubCommitCache[task].ToString("s", System.Globalization.CultureInfo.InvariantCulture));
                return true;
            };
            var client = new GitHubClient(new ProductHeaderValue(productHeader)) { Credentials = new Credentials(username, password) };
            var x = client.Repository.Commit.GetAll(taskRepoOwner, taskRepoName, new CommitRequest {
                Author = taskRepoCommitterEmail,
                Path = string.Format("out/{0}.json", task),
                Since = DateTimeOffset.Now.Add(-timespan)
            });
            var commitsExist = false;
            while (!x.IsCompleted)
            {
                x.Wait();
                commitsExist = x.Result != null && x.Result.Count > 0;
                if (commitsExist)
                {
                    if (githubCommitCache.ContainsKey(task))
                        githubCommitCache[task] = x.Result.OrderBy(c => c.Commit.Committer.Date).Last().Commit.Committer.Date;
                    else
                        githubCommitCache.Add(task, x.Result.OrderBy(c => c.Commit.Committer.Date).Last().Commit.Committer.Date);
                    logger.Debug("found commit in github for {0}, dated {1}.", task, githubCommitCache[task].ToString("s", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            return commitsExist;
        }

        public static void Update(string task, object report)
        {
            string sha;
            using (WebClient client = new WebClient())
            {
                var url = string.Format("https://api.github.com/repos/{0}/{1}/contents/out/{2}.json", taskRepoOwner, taskRepoName, task);
                client.Headers.Add(HttpRequestHeader.Authorization, string.Concat("Basic ", Convert.ToBase64String(new UTF8Encoding().GetBytes(string.Concat(username, ':', password)))));
                client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.github.v3+json");
                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)");
                var githubFile = JsonConvert.DeserializeObject<githubFile>(client.DownloadString(url));
                sha = githubFile.sha;
            }
            if (!string.IsNullOrWhiteSpace(sha))
            {
                using (WebClient client = new WebClient())
                {
                    var url = string.Format("https://api.github.com/repos/{0}/{1}/contents/out/{2}.json", taskRepoOwner, taskRepoName, task);
                    client.Headers.Add(HttpRequestHeader.Authorization, string.Concat("Basic ", Convert.ToBase64String(new UTF8Encoding().GetBytes(string.Concat(username, ':', password)))));
                    client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.github.v3+json");
                    client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)");
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    string responsebody = Encoding.UTF8.GetString(client.UploadData(url, "PUT", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new commit { content = JsonConvert.SerializeObject(report, Formatting.Indented), message = "payload execution report", sha = sha }))));
                }
            }
        }

        class githubFile
        {
            public string type { get; set; }
            public string encoding { get; set; }
            public int size { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            private string _content;
            public string content
            {
                get { return Encoding.UTF8.GetString(Convert.FromBase64String(_content)); }
                set { _content = value; }
            }
            public string sha { get; set; }
            public string url { get; set; }
            public string git_url { get; set; }
            public string html_url { get; set; }
            public string download_url { get; set; }
            public githubLinks _links { get; set; }
        }

        class githubLinks
        {
            public string git { get; set; }
            public string self { get; set; }
            public string html { get; set; }
        }

        class commit
        {
            public string message { get; set; }
            public committer committer { get { return new committer(); } }
            private string _content;
            public string content
            {
                get { return Convert.ToBase64String(new UTF8Encoding().GetBytes(_content)); }
                set { _content = value; }
            }
            public string sha { get; set; }
        }

        class committer
        {
            public string name { get { return taskRepoCommitterName; } }
            public string email { get { return taskRepoCommitterEmail; } }
        }
    }
}
