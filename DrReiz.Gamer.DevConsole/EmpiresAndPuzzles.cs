using NitroBolt.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Graph;
using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;

namespace DrReiz.Gamer.DevConsole
{
    public class EmpiresAndPuzzles
    {
        public static readonly string AppId = "56aef542-3607-4811-afb8-9726490171f8";
        public static readonly string Tenant = "common";
        public static string authority = $"https://login.microsoftonline.com/{Tenant}";

        public static string Initial_Secret_Name = "Initial";
        public static string Initial_Secret = System.IO.File.ReadAllText("../../microsoft.secret");

        //public static PublicClientApplication PublicClientApp = new PublicClientApplication(ClientId, authority, TokenCacheHelper.GetUserCache());
        public static PublicClientApplication PublicClientApp = new PublicClientApplication(AppId, authority);
        [CommandLine("empires-download-excel")]
        public static async Task DownloadExcel()
        {
            //var tokenPath = "microsoft.tokens.dat";

            //var tokenCache = new TokenCache();

            //if (System.IO.File.Exists(tokenPath))
            //{
            //    tokenCache.Deserialize(System.IO.File.ReadAllBytes(tokenPath));
            //}

            //// this is the OAUTH 2.0 TOKEN ENDPOINT from https://portal.azure.com/ -> Azure Active Directory -> App Registratuons -> End Points
            //var authenticationContext = new AuthenticationContext("https://login.windows.net/your-url-here/", tokenCache);

            //// only prompt when needed, you'll get a UI the first time you run
            //var platformParametes = new PlatformParameters(PromptBehavior.Auto);

            //var authenticationResult = await authenticationContext.AcquireTokenAsync("https://graph.microsoft.com/",
            //    "your-app-id",     // Application ID from https://portal.azure.com/
            //    new Uri("http://some.redirect.thing/"),         // Made up redirect URL, also from https://portal.azure.com/
            //    platformParametes);

            //string token = authenticationResult.AccessToken;

            //// save token so we don't need to re-authorize
            //System.IO.File.WriteAllBytes(tokenPath, tokenCache.Serialize());

            //var scopes = new[] { "user.read" };
            var scopes = new[] { "user.read", "Files.Read.All" };

            var tokenPath = "microsoft.token";
            var token = System.IO.File.Exists(tokenPath) ? System.IO.File.ReadAllText(tokenPath) : null;

            if (token == null)
            {
                var authResult = await PublicClientApp.AcquireTokenAsync(scopes);
                token = authResult.AccessToken;

                System.IO.File.WriteAllText(tokenPath, token);
            }

            Console.WriteLine(token);

            //var appKey = Initial_Secret;

            //var clientApp = new ConfidentialClientApplication(
            //            AppId,
            //            $"https://login.microsoftonline.com/{Tenant}/v2.0",
            //            "http://localhost",
            //            new ClientCredential(appKey),
            //            null,
            //            new TokenCache());


            ////var authResult = await clientApp.AcquireTokenForClientAsync(scopes);
            //var authResult = await clientApp.AcquireTokenForClientAsync(new string[] { "https://graph.microsoft.com/.default" });

            //var token = authResult.AccessToken;

            //Console.WriteLine(token);


            // use the token with Microsoft.Graph calls
            var client = new GraphServiceClient(new DelegateAuthenticationProvider(
                (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                    return Task.FromResult(0);
                }));



            // test reading from a sheet - in this case I have a test worksheet with a two column table for name/value pairs
            var readSheet = client.Me.Drive.Items["B9C21508096767EC!345"].Workbook.Worksheets["Sheet1"];

            var range = readSheet.UsedRange();
            var rangeData = await range.Request().GetAsync();

            Console.WriteLine(rangeData.Formulas);



            //var readTables = await readSheet.Tables.Request().GetAsync();
            //string readTableId = readTables[0].Name;
            //var table = await readSheet.Tables[readTableId].Rows.Request().GetAsync();



            //// convert page to a dictionary... this doesn't handle pagination
            //var tableValues = table.CurrentPage.ToDictionary(r => r.Values.First.First.ToString(),  r => Convert.ToDecimal(r.Values.First.Last, CultureInfo.InvariantCulture));

            //foreach (var row in tableValues)
            //    Console.WriteLine($"{row.Key}: {row.Value}");


            //// test adding a row to a table with four columns
            //// sadly it seems you need this exact format, a regular JArray or JObject fails
            //WorkbookTableRow newRow = new WorkbookTableRow
            //{
            //    Values = JArray.Parse("[[\"1\",\"2\",\"3\",\"4\"]]")
            //};



            //var outputSheet = client.Me.Drive.Items["your-workbook-id"].Workbook.Worksheets["data"];

            //var outputTables = outputSheet.Tables.Request().GetAsync().Result;

            //string outputTableId = outputTables[0].Name;

            //var outputResult = outputSheet.Tables[outputTableId].Rows.Request().AddAsync(newRow).Result;



            //// the excel unit tests seem to be the most useful documentation right now:

            //// https://github.com/microsoftgraph/msgraph-sdk-dotnet/blob/dev/tests/Microsoft.Graph.Test/Requests/Functional/ExcelTests.cs


        }

        //// If modifying these scopes, delete your previously saved credentials
        //// at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        //static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        //static readonly string ApplicationName = "Google Sheets API .NET Quickstart";

        //[CommandLine("empires-download-gdoc")]
        //public static async Task DownloadGdoc()
        //{
        //    var credentialsJson = File.ReadAllBytes("google.credentials.json");

        //    // The file token.json stores the user's access and refresh tokens, and is created
        //    // automatically when the authorization flow completes for the first time.
        //    var credPath = "google.token.json";
        //    var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        GoogleClientSecrets.Load(new MemoryStream(credentialsJson)).Secrets,
        //        Scopes,
        //        "user",
        //        CancellationToken.None,
        //        new FileDataStore(credPath, true));
        //    Console.WriteLine("Credential file saved to: " + credPath);

        //    // Create Google Sheets API service.
        //    var service = new SheetsService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName,
        //    });

        //    // Define request parameters.
        //    var spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
        //    var range = "Class Data!A2:E";
        //    var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

        //    // Prints the names and majors of students in a sample spreadsheet:
        //    // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
        //    var response = request.Execute();
        //    var values = response.Values;
        //    if (values != null && values.Count > 0)
        //    {
        //        Console.WriteLine("Name, Major");
        //        foreach (var row in values)
        //        {
        //            // Print columns A and E, which correspond to indices 0 and 4.
        //            Console.WriteLine("{0}, {1}", row[0], row[4]);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("No data found.");
        //    }
        //}

    }
}
